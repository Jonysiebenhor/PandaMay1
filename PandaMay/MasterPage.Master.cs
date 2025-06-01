using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PandaMay
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        Conectar conectado = new Conectar();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //String usuario = Request.QueryString["id"].ToString();
                if (Session["usuario"] is null)
                {
                    Response.Redirect("/Login.aspx");
                }
                String usuario = Session["usuario"].ToString();

                conectado.conectar();
                DataRow rows = conectado.consultaUsuarioloign(usuario).Rows[0];
                String nombre = Convert.ToString(Convert.ToString(rows["primerNombre"]));
                String apellido = Convert.ToString(Convert.ToString(rows["primerApellido"]));
                Label2.Text = nombre;
                Label3.Text = apellido;
                conectado.desconectar();
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Session.Remove("usuario");
            Session.RemoveAll();
            Response.Redirect("/Login.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}