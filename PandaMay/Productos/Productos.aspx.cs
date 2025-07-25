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

            // NUEVO BLOQUE INSERTADO
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

                imgFotoDetalle.ImageUrl = "~/VerImagen.ashx?id=" + idProducto + "&detalle=1&t=" + DateTime.Now.Ticks;
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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var drv = (DataRowView)e.Row.DataItem;
            int idProd = Convert.ToInt32(drv["idproducto"]);

            var img = (Image)e.Row.FindControl("imgFoto");
            if (img != null)
            {
                img.ImageUrl = ResolveUrl("~/VerImagen.ashx?id=" + idProd + "&t=" + DateTime.Now.Ticks);
            }
        }


    }


}
