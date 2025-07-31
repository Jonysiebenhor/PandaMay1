using System;
using System.Data.SqlClient;
using System.Web;

namespace PandaMay
{
    public class VerImagen : IHttpHandler
    {
        private const string PLACEHOLDER_VIRTUAL = "~/uploads/productos/aretes.png";
        private const string CONN_STR =
           "workstation id=PandaMay.mssql.somee.com;packet size=4096;user id=…;pwd=…;data source=PandaMay.mssql.somee.com;persist security info=False;initial catalog=PandaMay;TrustServerCertificate=True";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // 1) Si recibimos imgid, lo usamos para sacar el blob de IMAGENES
                if (int.TryParse(context.Request.QueryString["imgid"], out int idImg) && idImg > 0)
                {
                    var foto = ObtenerFotoPorIdImagen(idImg);
                    if (foto?.Length > 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "image/jpeg";
                        context.Response.BinaryWrite(foto);
                        SetNoCache(context);
                        return;
                    }
                }

                // 2) Si no vino imgid o falló, devolvemos el placeholder
                EnviarPlaceholder(context);
            }
            catch
            {
                EnviarPlaceholder(context);
            }
            finally
            {
                context.ApplicationInstance.CompleteRequest();
            }
        }

        // Saca el blob directamente de IMAGENES
        private static byte[] ObtenerFotoPorIdImagen(int idImg)
        {
            const string sql = @"
SELECT CAST(foto AS varbinary(max)) 
  FROM dbo.IMAGENES 
 WHERE idimagen = @iid
   AND activo   = 1;";
            using (var cn = new SqlConnection(CONN_STR))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@iid", idImg);
                cn.Open();
                return cmd.ExecuteScalar() as byte[];
            }
        }

        private static void SetNoCache(HttpContext ctx)
        {
            ctx.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ctx.Response.Cache.SetNoStore();
            ctx.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        }

        private static void EnviarPlaceholder(HttpContext context)
        {
            string phys = context.Server.MapPath(PLACEHOLDER_VIRTUAL);
            context.Response.Clear();
            context.Response.ContentType = "image/png";
            context.Response.TransmitFile(phys);
        }

        public bool IsReusable => false;
    }
}
