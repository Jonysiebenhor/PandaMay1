using System;
using System.Data.SqlClient;
using System.Web;

namespace PandaMay
{
    public class VerImagen : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!int.TryParse(context.Request.QueryString["id"], out int idProd))
            {
                context.Response.Redirect("~/images/no-image.png");
                return;
            }

            byte[] foto = null;
            string connStr = "workstation id=PandaMay.mssql.somee.com;packet size=4096;user id=Jonysiebenhor_SQLLogin_1;pwd=9btgzhlyqy;data source=PandaMay.mssql.somee.com;persist security info=False;initial catalog=PandaMay;TrustServerCertificate=True";

            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 foto
                FROM Imagenes
                WHERE idproducto = @id AND activo = 1
                ORDER BY fecha DESC;", cn))
            {
                cmd.Parameters.AddWithValue("@id", idProd);
                cn.Open();
                foto = cmd.ExecuteScalar() as byte[];
            }

            if (foto != null && foto.Length > 0)
            {
                context.Response.ContentType = "image/jpeg";
                context.Response.BinaryWrite(foto);
            }
            else
            {
                context.Response.Redirect("~/productos/aretes.png");
            }
        }

        public bool IsReusable => false;
    }
}

