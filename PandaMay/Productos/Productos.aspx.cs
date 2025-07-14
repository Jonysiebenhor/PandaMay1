using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PandaMay;  // tu espacio de nombres donde está Conectar
namespace PandaMay.Productos
{
    public partial class Productos : System.Web.UI.Page
    {
        Conectar conectado = new Conectar();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1) Validar sesión siempre
            if (Session["usuario"] == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                // 2) Al cargar, ocultar el panel de la tabla
                pnlTabla.Visible = false;
            }
        }

        protected void btnCrearProducto_Click(object sender, EventArgs e)
        {
            // 3) Cargar la lista de productos
            conectado.conectar();
            GridView1.DataSource = conectado.productos();
            GridView1.DataBind();
            conectado.desconectar();

            // 4) Mostrar la tabla y ocultar el botón inicial
            pnlTabla.Visible = true;
            btnCrearProducto.Visible = false;
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            // 5) Volver a la vista inicial
            pnlTabla.Visible = false;
            btnCrearProducto.Visible = true;
        }

        protected void Select1(object sender, GridViewSelectEventArgs e)
        {
            // 1) Recuperar el idproducto de la fila seleccionada
            int idProducto = Convert.ToInt32(GridView1.DataKeys[e.NewSelectedIndex].Value);

            // 2) Mostrar el panel de detalles
            pnlDetalles.Visible = true;

            // 3) Cargar cada GridView llamando a tu método genérico
            conectado.conectar();

            gvCombosProductos.DataSource = conectado.GetByProducto("COMBOSPRODUCTOS", idProducto);
            gvDetallesCompras.DataSource = conectado.GetByProducto("DETALLESCOMPRAS", idProducto);
            gvDetallesTraslados.DataSource = conectado.GetByProducto("DETALLESTRASLADOS", idProducto);
            gvDetallesVentas.DataSource = conectado.GetByProducto("DETALLESVENTAS", idProducto);
            gvPrecios.DataSource = conectado.GetByProducto("PRECIOS", idProducto);
            gvPreciosCompras.DataSource = conectado.GetByProducto("PRECIOSCOMPRAS", idProducto);
            gvAtributos.DataSource = conectado.GetByProducto("ATRIBUTOS", idProducto);
            gvExistencias.DataSource = conectado.GetByProducto("EXISTENCIAS", idProducto);

            // 4) Hacer DataBind de cada uno
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

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            // Si el usuario borra todo, volvemos a mostrar la lista completa
            string criterio = txtBuscar.Text.Trim();
            conectado.conectar();

            if (criterio == "")
                GridView1.DataSource = conectado.productos();
            else
                GridView1.DataSource = conectado.buscarproducto(criterio);

            GridView1.DataBind();
            conectado.desconectar();

            // Aseguramos que, si venía oculto, muestre la tabla
            pnlTabla.Visible = true;
            btnCrearProducto.Visible = false;
        }

    }
}