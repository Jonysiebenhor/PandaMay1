using PandaMay;  // Ajusta al namespace de tu clase Conectar
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;




namespace PandaMay.Productos
{
    public partial class Productos : Page
    {
        // Instancia a tu helper de datos
        private readonly Conectar conectado = new Conectar();

        private readonly string _connString =
           ConfigurationManager.ConnectionStrings["ConnectionString4"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                pnlTabla.Visible = true;
                CargarProductos();
            }
        }

        /// <summary>
        /// Carga el grid principal invocando al nuevo método que retorna productos + tarifas pivotadas.
        /// </summary>
        private void CargarProductos(string filtro = "")
        {
            conectado.conectar();

            DataTable dt = string.IsNullOrEmpty(filtro)
                ? conectado.GetProductosConTarifas()
                : conectado.BuscarProductosConTarifas(filtro);

            GridView1.DataSource = dt;
            GridView1.DataBind();

            conectado.desconectar();
        }




        protected void txtBuscar_TextChanged(object sender, EventArgs e)
            => CargarProductos(txtBuscar.Text.Trim());

        protected void btnCrearProducto_Click(object sender, EventArgs e)
            => Response.Redirect("CrearProducto.aspx");


        /// <summary>
        /// Cuando el usuario selecciona una fila, cargamos todos los detalles del producto.
        /// </summary>
        protected void Select1(object sender, GridViewSelectEventArgs e)
        {
            int idProducto = Convert.ToInt32(GridView1.DataKeys[e.NewSelectedIndex].Value);

            var datos = conectado.ObtenerProductoCompleto(idProducto);
            if (datos != null)
            {
                lblNombre.Text = datos["nombre"] != DBNull.Value ? datos["nombre"].ToString() : "";
                lblReferencia.Text = datos["referencia"] != DBNull.Value ? datos["referencia"].ToString() : "";
                lblCodigoBarras.Text = datos["codigodebarras"] != DBNull.Value ? datos["codigodebarras"].ToString() : "";
                lblUnidad.Text = datos["unidad"] != DBNull.Value ? datos["unidad"].ToString() : "";
                lblMarca.Text = datos["marca"] != DBNull.Value ? datos["marca"].ToString() : "";
                lblSubcategoria.Text = datos["subcategoria"] != DBNull.Value ? datos["subcategoria"].ToString() : "";
                lblCategoria.Text = datos["categoria"] != DBNull.Value ? datos["categoria"].ToString() : "";
                lblCatMaestra.Text = datos["categoriamaestra"] != DBNull.Value ? datos["categoriamaestra"].ToString() : "";
                lblDescuento.Text = datos["descuento"] != DBNull.Value ? datos["descuento"].ToString() : "";
                lblTipo.Text = datos["tipodeproducto"] != DBNull.Value ? datos["tipodeproducto"].ToString() : "";
                chkActivo.Checked = datos["activo"] != DBNull.Value && Convert.ToBoolean(datos["activo"]);

                imgFotoDetalle.ImageUrl = $"~/VerImagen.ashx?id={idProducto}&detalle=1&t={DateTime.Now.Ticks}";
                imgFotoDetalle.AlternateText = lblNombre.Text;
            }

            pnlDetalles.Visible = true;

            conectado.conectar();

            gvCombosProductos.DataSource = conectado.GetByProducto("COMBOSPRODUCTOS", idProducto);
            gvDetallesCompras.DataSource = conectado.GetByProducto("DETALLESCOMPRAS", idProducto);
            gvDetallesTraslados.DataSource = conectado.GetByProducto("DETALLESTRASLADOS", idProducto);
            gvDetallesVentas.DataSource = conectado.GetByProducto("DETALLESVENTAS", idProducto);
            gvPrecios.DataSource = conectado.GetByProducto("PRECIOS", idProducto);

            // ---------- PRECIOS DE COMPRA: Proveedor | Precio | Fecha, con fallback ----------
            var dtPcRaw = conectado.GetByProducto("PRECIOSCOMPRAS", idProducto);

            var dtView = new DataTable();
            dtView.Columns.Add("Proveedor", typeof(string));
            dtView.Columns.Add("Precio", typeof(decimal));
            dtView.Columns.Add("Fecha", typeof(DateTime));

            // 1) ids de precio-compra
            var idsPrecioCompra = new List<int>();
            if (dtPcRaw.Columns.Contains("idpreciocompra"))
                foreach (DataRow r in dtPcRaw.Rows)
                    if (r["idpreciocompra"] != DBNull.Value)
                        idsPrecioCompra.Add(Convert.ToInt32(r["idpreciocompra"]));

            // 2) proveedor por join (si existe en DETALLESCOMPRAS→COMPRAS)
            var mapProveedorPorPC = GetProveedorPorPrecioCompraMap(idProducto, idsPrecioCompra);

            // 3) fallback: leer ProveedorId desde "descripcion" y resolver nombres reales
            var proveedorIds = new HashSet<int>();
            foreach (DataRow r in dtPcRaw.Rows)
            {
                if (r.Table.Columns.Contains("descripcion"))
                {
                    int? pid = ExtraerProveedorIdDesdeDescripcion(r["descripcion"]?.ToString());
                    if (pid.HasValue) proveedorIds.Add(pid.Value);
                }
            }
            var mapNombreProveedorPorId = GetProveedorNombresPorIds(proveedorIds.ToList());

            // 4) poblar vista final
            foreach (DataRow r in dtPcRaw.Rows)
            {
                string proveedor = "";

                // a) intento por idpreciocompra
                if (dtPcRaw.Columns.Contains("idpreciocompra") && r["idpreciocompra"] != DBNull.Value)
                {
                    int idpc = Convert.ToInt32(r["idpreciocompra"]);
                    mapProveedorPorPC.TryGetValue(idpc, out proveedor);
                }

                // b) fallback por ProveedorId en descripcion
                if (string.IsNullOrWhiteSpace(proveedor) && r.Table.Columns.Contains("descripcion"))
                {
                    int? pid = ExtraerProveedorIdDesdeDescripcion(r["descripcion"]?.ToString());
                    if (pid.HasValue)
                    {
                        if (!mapNombreProveedorPorId.TryGetValue(pid.Value, out proveedor) || string.IsNullOrWhiteSpace(proveedor))
                            proveedor = "Proveedor #" + pid.Value;
                    }
                }

                decimal precio = (dtPcRaw.Columns.Contains("precio") && r["precio"] != DBNull.Value)
                                   ? Convert.ToDecimal(r["precio"]) : 0m;

                object fechaObj = (dtPcRaw.Columns.Contains("fecha") && r["fecha"] != DBNull.Value)
                                    ? (object)Convert.ToDateTime(r["fecha"])
                                    : DBNull.Value;

                dtView.Rows.Add(proveedor ?? "", precio, fechaObj);
            }

            gvPreciosCompras.DataSource = dtView;
            // ---------------------------------------------------------------------

            gvAtributos.DataSource = conectado.GetByProducto("ATRIBUTOS", idProducto);
            gvExistencias.DataSource = conectado.GetExistenciasConColorYPublico(idProducto);

            gvCombosProductos.DataBind();
            gvDetallesCompras.DataBind();
            gvDetallesTraslados.DataBind();
            gvDetallesVentas.DataBind();
            gvPrecios.DataBind();
            gvPreciosCompras.DataBind();
            gvAtributos.DataBind();
            gvExistencias.DataBind();

            conectado.desconectar();
        }



        // Helper para extraer el proveedor desde "descripcion" si venía como "ProveedorId:3" o similar
        private static string ExtraerProveedorDesdeDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion)) return "";
            var m = Regex.Match(descripcion, @"Proveedor(Id)?\s*:\s*(\d+)", RegexOptions.IgnoreCase);
            if (m.Success) return "Proveedor #" + m.Groups[2].Value;
            return descripcion; // por si ya guardas el nombre del proveedor en texto
        }


        protected string GetPreciosHtml(object dataItem)
        {
            var row = (DataRowView)dataItem;
            var sb = new StringBuilder();

            // Recorro sólo las columnas que usaste en el pivot
            foreach (string tarifa in new[] { "unidad", "tresomas", "docena", "fardo" })
            {
                if (row.DataView.Table.Columns.Contains(tarifa))
                {
                    var val = row[tarifa];
                    if (val != DBNull.Value)
                        sb.AppendFormat("<div><strong>{0}:</strong> {1:C}</div>",
                                        tarifa, Convert.ToDecimal(val));
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Helper para mostrar la foto en el grid principal.
        /// </summary>

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // 1) Sólo filas de datos
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            // 2) Obtenemos el idimagen de la fila
            var drv = (DataRowView)e.Row.DataItem;
            int idImg = drv["idimagen"] != DBNull.Value
                ? Convert.ToInt32(drv["idimagen"])
                : 0;

            // 3) Localizamos el <asp:Image ID="imgFoto">
            var img = (Image)e.Row.FindControl("imgFoto");
            if (img == null) return;

            // 4) Le asignamos la URL al handler
            if (idImg > 0)
                img.ImageUrl = $"~/VerImagen.ashx?imgid={idImg}&t={DateTime.Now.Ticks}";
            else
                img.ImageUrl = ResolveUrl("~/uploads/productos/aretes.png");
        }

        /// <summary>
        /// Devuelve un mapa idpreciocompra -> Nombre del proveedor,
        /// uniendo DETALLESCOMPRAS → COMPRAS → PROVEEDORES → USUARIOS en UNA consulta.
        /// No requiere cambios en la BD.
        /// </summary>
        private Dictionary<int, string> GetProveedorPorPrecioCompraMap(int idProducto, IList<int> idsPrecioCompra)
        {
            var map = new Dictionary<int, string>();
            if (idsPrecioCompra == null || idsPrecioCompra.Count == 0) return map;

            // Construir @p0,@p1,... para el IN (...)
            var paramNames = new List<string>(idsPrecioCompra.Count);
            for (int i = 0; i < idsPrecioCompra.Count; i++)
                paramNames.Add("@p" + i);

            string sql = $@"
        SELECT 
            dc.idpreciocompra,
            -- Elegimos el nombre del proveedor:
            -- 1) nombreNegocio (si existe)
            -- 2) nombre completo (primer/segundo/tercer nombre + apellidos)
            -- 3) 'Proveedor #<idproveedor>' como último recurso
            COALESCE(
                NULLIF(u.nombreNegocio, ''),
                NULLIF(
                    LTRIM(RTRIM(CONCAT(
                        ISNULL(u.primerNombre,''), ' ',
                        ISNULL(u.segundoNombre,''), ' ',
                        ISNULL(u.tercerNombre,''), ' ',
                        ISNULL(u.primerApellido,''), ' ',
                        ISNULL(u.segundoApellido,''), ' ',
                        ISNULL(u.apellidoCasada,'')
                    ))),
                ''),
                'Proveedor #' + CAST(pr.idproveedor AS varchar(12))
            ) AS Proveedor
        FROM dbo.DETALLESCOMPRAS dc
        INNER JOIN dbo.COMPRAS c
            ON c.idcompra = dc.idcompra
        INNER JOIN dbo.PROVEEDORES pr
            ON pr.idproveedor = c.idproveedor
        LEFT JOIN dbo.USUARIOS u
            ON u.idusuario = pr.idusuario
        WHERE dc.idproducto = @IdProducto
          AND dc.idpreciocompra IN ({string.Join(",", paramNames)})
    ";

            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@IdProducto", SqlDbType.Int).Value = idProducto;
                for (int i = 0; i < idsPrecioCompra.Count; i++)
                    cmd.Parameters.Add(paramNames[i], SqlDbType.Int).Value = idsPrecioCompra[i];

                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int idpc = dr.GetInt32(0);
                        string proveedor = dr.IsDBNull(1) ? "" : dr.GetString(1);
                        map[idpc] = proveedor;
                    }
                }
            }
            return map;
        }

        // Lee "ProveedorId: 3" (o "Proveedor:3") desde descripcion
        private static int? ExtraerProveedorIdDesdeDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion)) return null;
            var m = Regex.Match(descripcion, @"Proveedor(Id)?\s*:\s*(\d+)", RegexOptions.IgnoreCase);
            if (m.Success && int.TryParse(m.Groups[2].Value, out int id)) return id;
            return null;
        }

        // Trae nombres reales para una lista de ids de proveedor (una sola consulta)
        private Dictionary<int, string> GetProveedorNombresPorIds(IList<int> idsProveedor)
        {
            var map = new Dictionary<int, string>();
            if (idsProveedor == null || idsProveedor.Count == 0) return map;

            var paramNames = new List<string>(idsProveedor.Count);
            for (int i = 0; i < idsProveedor.Count; i++) paramNames.Add("@p" + i);

            string sql = $@"
        SELECT 
            pr.idproveedor,
            COALESCE(
                NULLIF(u.nombreNegocio, ''),
                NULLIF(LTRIM(RTRIM(CONCAT(
                    ISNULL(u.primerNombre,''), ' ',
                    ISNULL(u.segundoNombre,''), ' ',
                    ISNULL(u.tercerNombre,''), ' ',
                    ISNULL(u.primerApellido,''), ' ',
                    ISNULL(u.segundoApellido,''), ' ',
                    ISNULL(u.apellidoCasada,'')
                ))),''),
                'Proveedor #' + CAST(pr.idproveedor AS varchar(12))
            ) AS Nombre
        FROM dbo.PROVEEDORES pr
        LEFT JOIN dbo.USUARIOS u ON u.idusuario = pr.idusuario
        WHERE pr.idproveedor IN ({string.Join(",", paramNames)})
    ";

            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                for (int i = 0; i < idsProveedor.Count; i++)
                    cmd.Parameters.Add(paramNames[i], SqlDbType.Int).Value = idsProveedor[i];

                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        map[dr.GetInt32(0)] = dr.IsDBNull(1) ? "" : dr.GetString(1);
            }

            return map;
        }



    }
}
