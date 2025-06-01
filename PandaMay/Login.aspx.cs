using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PandaMay
{
    public partial class Login : System.Web.UI.Page
    {
        Conectar conectado = new Conectar();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button2_Click(object sender, EventArgs e)
        {

            String usuario = txtusuario.Text;
            string contraseña = txtcontraseña.Text;


            if (usuario == "" || contraseña == "")

            {
                estado.Text = "Faltan Datos";
                estado.ForeColor = Color.Red;
            }
            else
            {
                conectado.conectar();

                DataTable ingresousuario = new DataTable();
                ingresousuario = conectado.consultaUsuarioloign(usuario); //validacion con usuario
                DataTable ingresocontraseña = new DataTable();
                ingresocontraseña = conectado.consultaUsuarioloign1( contraseña); //validacion con contraseña

                if (ingresousuario.Rows.Count > 0 & ingresocontraseña.Rows.Count > 0)
                {
                    DataRow rows = conectado.consultaUsuarioloign(usuario).Rows[0];
                    String idusuario = Convert.ToString(Convert.ToString(rows["idusuario"]));
                    String activo = Convert.ToString(Convert.ToString(rows["activo"]));

                    if (activo == "False")
                    {
                        estado.Text = "Usuario Inactivo " + usuario;
                        estado.ForeColor = Color.Red;
                    }
                    DataTable cliente = new DataTable();
                    cliente = conectado.consultaUsuarioloign2(usuario); //validacion DE CLIENTE
                    DataTable revendedor = new DataTable();
                    revendedor = conectado.consultaUsuarioloign3(usuario); //validacion DE revendedor
                    DataTable empleado = new DataTable();
                    empleado = conectado.consultaUsuarioloign4(usuario); //validacion DE empleado
                    conectado.desconectar();
                    
                    if (cliente.Rows.Count > 0)
                    {
                        Response.Redirect("/Login.aspx");
                        Session["usuario"] = txtusuario.Text;
                        Response.Redirect("/empleados/confirmacionpedidos.aspx");

                    }
                    else if (revendedor.Rows.Count > 0)
                    {
                        Response.Redirect("/Login.aspx");
                        Response.Redirect("/empleados/empacarpedidos.aspx");
                    }
                    else if (empleado.Rows.Count > 0)
                    {
                        Session["usuario"] = txtusuario.Text;
                        Response.Redirect("/empleados/menuempl.aspx");
                    }
                }
                else if (ingresousuario.Rows.Count == 0)
                    {
                    estado.Text = "Usuario Incorrecto: " + usuario;
                    estado.ForeColor = Color.Red;
                }
                else if (ingresocontraseña.Rows.Count == 0)
                {
                    estado.Text = "Contraseña Incorrecta";
                    estado.ForeColor = Color.Red;
                }

            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrearCuenta.aspx");
        }
    }
}