using PandaMay;  // Ajusta al namespace de tu clase Conectar
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PandaMay.Productos
{
    public partial class Productos : Page
    {
        // Instancia a tu helper de datos
        private readonly Conectar conectado = new Conectar();

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
                lblNombre.Text = datos["nombre"].ToString();
                lblReferencia.Text = datos["referencia"].ToString();
                lblCodigoBarras.Text = datos["codigodebarras"].ToString();
                lblUnidad.Text = datos["unidad"].ToString();
                lblMarca.Text = datos["marca"].ToString();
                lblSubcategoria.Text = datos["subcategoria"].ToString();
                lblCategoria.Text = datos["categoria"].ToString();
                lblCatMaestra.Text = datos["categoriamaestra"].ToString();
                lblDescuento.Text = datos["descuento"].ToString();
                lblTipo.Text = datos["tipodeproducto"].ToString();
                chkActivo.Checked = Convert.ToBoolean(datos["activo"]);

                imgFotoDetalle.ImageUrl = $"~/VerImagen.ashx?id={idProducto}&detalle=1&t={DateTime.Now.Ticks}";
            }

            pnlDetalles.Visible = true;
            conectado.conectar();

            gvCombosProductos.DataSource = conectado.GetByProducto("COMBOSPRODUCTOS", idProducto);
            gvDetallesCompras.DataSource = conectado.GetByProducto("DETALLESCOMPRAS", idProducto);
            gvDetallesTraslados.DataSource = conectado.GetByProducto("DETALLESTRASLADOS", idProducto);
            gvDetallesVentas.DataSource = conectado.GetByProducto("DETALLESVENTAS", idProducto);
            gvPrecios.DataSource = conectado.GetByProducto("PRECIOS", idProducto);
            gvPreciosCompras.DataSource = conectado.GetByProducto("PRECIOSCOMPRAS", idProducto);
            gvAtributos.DataSource = conectado.GetByProducto("ATRIBUTOS", idProducto);
            gvExistencias.DataSource = conectado.GetByProducto("EXISTENCIAS", idProducto);

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





    }
}
