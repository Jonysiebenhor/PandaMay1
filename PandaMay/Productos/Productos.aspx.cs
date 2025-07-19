using PandaMay;  // Ajusta al namespace donde esté tu clase Conectar
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PandaMay.Productos
{
    public partial class Productos : Page
    {
        Conectar conectado = new Conectar();

        protected void Page_Load(object sender, EventArgs e)
        {
            // ——— 1) Servicio de imagen inline ———
            string imgid = Request.QueryString["imgid"];
            if (!string.IsNullOrEmpty(imgid) && int.TryParse(imgid, out int idProd))
            {
                byte[] foto = null;
                // Usa aquí tu misma cadena de conexión de Conectar.cs
                string connStr = "workstation id=PandaMay.mssql.somee.com;packet size=4096;user id=Jonysiebenhor_SQLLogin_1;pwd=9btgzhlyqy;data source=PandaMay.mssql.somee.com;persist security info=False;initial catalog=PandaMay;TrustServerCertificate=True";

                using (var cn = new SqlConnection(connStr))
                using (var cmd = new SqlCommand(@"
            SELECT TOP 1 i.foto
              FROM Imagenes i
             WHERE i.idproducto = @id
               AND i.activo = 1
          ORDER BY i.fecha DESC", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idProd);
                    cn.Open();
                    foto = cmd.ExecuteScalar() as byte[];
                }

                if (foto != null && foto.Length > 0)
                {
                    Response.Clear();
                    Response.ContentType = "image/webp";    // o "image/png" si cambias el formato
                    Response.BinaryWrite(foto);
                }
                else
                {
                    // Si no hay imagen en BD, muestro tu placeholder
                    Response.Redirect("~/images/no-image.png");
                }

                Response.End();
                return;  // terminamos aquí la petición
            }
            // ——————————————————————————————

            // 2) Tu lógica normal de Page_Load
            if (Session["usuario"] == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                pnlTabla.Visible = true;
                CargarProductos();
            }
        }


        // Método auxiliar para bindear el grid principal
        private void CargarProductos(string filtro = "")
        {
            conectado.conectar();
            if (String.IsNullOrEmpty(filtro))
                GridView1.DataSource = conectado.productos();
            else
                GridView1.DataSource = conectado.buscarproducto(filtro);
            GridView1.DataBind();
            conectado.desconectar();
        }

        // Búsqueda en vivo
        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarProductos(txtBuscar.Text.Trim());
        }

        // Redirigir a CrearProducto.aspx
        protected void btnCrearProducto_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrearProducto.aspx");
        }

        // Al seleccionar un producto, cargamos los grids de detalle
        protected void Select1(object sender, GridViewSelectEventArgs e)
        {
            int idProducto = Convert.ToInt32(GridView1.DataKeys[e.NewSelectedIndex].Value);

            pnlDetalles.Visible = true;
            conectado.conectar();

            gvCombosProductos.DataSource      = conectado.GetByProducto("COMBOSPRODUCTOS",   idProducto);
            gvDetallesCompras.DataSource      = conectado.GetByProducto("DETALLESCOMPRAS",  idProducto);
            gvDetallesTraslados.DataSource    = conectado.GetByProducto("DETALLESTRASLADOS",idProducto);
            gvDetallesVentas.DataSource       = conectado.GetByProducto("DETALLESVENTAS",   idProducto);
            gvPrecios.DataSource              = conectado.GetByProducto("PRECIOS",          idProducto);
            gvPreciosCompras.DataSource       = conectado.GetByProducto("PRECIOSCOMPRAS",   idProducto);
            gvAtributos.DataSource            = conectado.GetByProducto("ATRIBUTOS",        idProducto);
            gvExistencias.DataSource          = conectado.GetByProducto("EXISTENCIAS",      idProducto);

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

        protected string GetFotoUrl(object foto)
        {
            if (foto == DBNull.Value)
                return ResolveUrl("~/images/no-image.png");

            // Si viene un byte[], lo convertimos a Base64 con mime WebP
            if (foto is byte[] bytes)
                return "data:image/webp;base64,"
                     + Convert.ToBase64String(bytes);

            // Si fuera string (ruta), lo resolvemos normalmente
            return ResolveUrl(foto.ToString());
        }

    }


}
