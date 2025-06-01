using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PandaMay.Empleados
{
    public partial class Ventas : System.Web.UI.Page
    {
        Conectar conectado = new Conectar();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["usuario"] is null)
                {
                    Response.Redirect("/Login.aspx");
                }
                String usuario = Session["usuario"].ToString();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("/empleados/detalleventas.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }
    }
}