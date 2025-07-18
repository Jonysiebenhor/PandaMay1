using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using PandaMay;  // Ajusta al namespace donde esté tu clase Conectar

namespace PandaMay.Productos
{
    public partial class Productos : Page
    {
        Conectar conectado = new Conectar();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1) Validar sesión
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
    }
}
