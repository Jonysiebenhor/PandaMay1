using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PandaMay
{
    public partial class IngresoPedidos1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] is null)
            {
                Response.Redirect("/Login.aspx");
            }
        }
        protected void ddldepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlmunicipio.Items.Clear();
            ddlzona.Items.Clear();
            ddlzona0.Items.Clear();
            montoenvio.Text = "";

            ddlmunicipio.Items.Insert(0, new ListItem("--Municipio", "0"));
            ddlzona.Items.Insert(0, new ListItem("--Zona, Aldea, Lugar", "0"));
          
        }
        protected void ddlmunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlzona.Items.Clear();
            ddlzona0.Items.Clear();
            montoenvio.Text = "";
            ddlzona.Items.Insert(0, new ListItem("--Zona, Aldea, Lugar", "0"));
         
        }
        protected void ddlzona_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlzona0.Items.Clear();
        }
        protected void ddlzona0_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }

        protected void Button3_Click(object sender, EventArgs e)
        {

        }
    }
}