using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace PandaMay.Empleados
{
    public partial class MenuEmpl : System.Web.UI.Page
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
                conectado.conectar();
                String usuario = Session["usuario"].ToString();
                string connect = "Data Source=DESKTOP-KNTJ3BG\\SQLEXPRESS;DATABASE=PandaMay;Integrated security=true";
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    String query = "Select z.nombre from puestos z left join  empleadospuestos a on z.idpuesto=a.idpuesto left join empleados b on a.idempleado=b.idempleado left join usuarios c on b.idusuario=c.idusuario where c.usuario ='" + usuario + "'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    
                    try
                    {
                        conn.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())//se obtiene el tipo de empleado para validar que botones le aparecen
                        {
                            while (rdr.Read())
                            {
                                String var = (rdr[0].ToString());
                                if (var is "Administracion")
                                {
                                    Button1.Visible = true;
                                    Button2.Visible = true;
                                    Button3.Visible = true;
                                    Button4.Visible = true;
                                    Button5.Visible = true;
                                }
                                else if (var is "Ventas")
                                {
                                    Button1.Visible = true;
                                }
                                else if (var is "CallCenter")
                                {
                                    Button3.Visible = true;
                                    Button4.Visible = true;
                                }
                                else if (var is "Produccion")
                                {
                                    Button2.Visible = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                    }
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("/empleados/ventas.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }

        protected void Button3_Click(object sender, EventArgs e)
        {

        }

        protected void Button4_Click(object sender, EventArgs e)
        {

        }

        protected void Button5_Click(object sender, EventArgs e)
        {

        }
    }
}