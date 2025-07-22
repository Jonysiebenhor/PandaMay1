using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace PandaMay
{
    public class VerImagen : IHttpHandler
    {
        // Cambia esto si tu placeholder está en otro lado
        private const string PLACEHOLDER_VIRTUAL = "~/uploads/productos/aretes.png";

        // MISMO connection string que usas en el resto del proyecto
        private const string CONN_STR =
            "workstation id=PandaMay.mssql.somee.com;packet size=4096;user id=Jonysiebenhor_SQLLogin_1;pwd=9btgzhlyqy;data source=PandaMay.mssql.somee.com;persist security info=False;initial catalog=PandaMay;TrustServerCertificate=True";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (!int.TryParse(context.Request.QueryString["id"], out int idProd))
                {
                    EnviarPlaceholder(context);
                    return;
                }

                // Modo debug: /VerImagen.ashx?id=17&dbg=1
                if (context.Request.QueryString["dbg"] == "1")
                {
                    byte[] dummy = ObtenerFoto(idProd, out string dump);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(dump);
                    context.Response.Write("\r\nbytes=" + (dummy == null ? 0 : dummy.Length));
                    return;
                }

                byte[] foto = ObtenerFoto(idProd, out _);

                if (foto != null && foto.Length > 0)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "image/jpeg"; // ajusta si guardas png/webp
                    context.Response.BinaryWrite(foto);

                    // sin caché mientras pruebas
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    context.Response.Cache.SetNoStore();
                    context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                }
                else
                {
                    EnviarPlaceholder(context);
                }
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.Write("ERROR VerImagen.ashx\r\n\r\n" + ex);
            }
            finally
            {
                context.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Lee la imagen con DataReader para evitar que ExecuteScalar devuelva null.
        /// También arma un dump para el modo dbg.
        /// </summary>
        private static byte[] ObtenerFoto(int idProd, out string dump)
        {
            var sb = new StringBuilder();
            byte[] result = null;

            using (var cn = new SqlConnection(CONN_STR))
            using (var cmd = new SqlCommand(@"
SET TEXTSIZE 2147483647;

SELECT TOP 5 idimagen, activo, DATALENGTH(foto) AS bytes, fecha
FROM dbo.IMAGENES
WHERE idproducto = @id
ORDER BY ISNULL(fecha,'19000101') DESC, idimagen DESC;

-- La fila real que vamos a devolver
SELECT TOP 1 CAST(foto AS varbinary(max)) AS foto
FROM dbo.IMAGENES
WHERE idproducto = @id AND activo = 1
ORDER BY ISNULL(fecha,'19000101') DESC, idimagen DESC;", cn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = idProd;
                cn.Open();

                // 1er resultset: dump
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        sb.AppendLine($"row: idimg={rd["idimagen"]}, act={rd["activo"]}, bytes={rd["bytes"]}, fecha={rd["fecha"]}");
                    }

                    // Ir al siguiente resultset (la imagen)
                    if (rd.NextResult())
                    {
                        if (rd.Read() && !rd.IsDBNull(0))
                        {
                            // Tamaño
                            long len = rd.GetBytes(0, 0, null, 0, 0);
                            result = new byte[len];
                            long bytesRead = 0;
                            int offset = 0;
                            const int chunk = 8192;
                            while (bytesRead < len)
                            {
                                int read = (int)rd.GetBytes(0, bytesRead, result, offset, chunk);
                                bytesRead += read;
                                offset += read;
                            }
                        }
                    }
                }
            }

            var csb = new SqlConnectionStringBuilder(CONN_STR);
            dump = $"id={idProd}\r\ndb={csb.InitialCatalog}\r\nsrv={csb.DataSource}\r\n{sb}";
            return result;
        }

        private static void EnviarPlaceholder(HttpContext context)
        {
            string phys = context.Server.MapPath(PLACEHOLDER_VIRTUAL);
            context.Response.Clear();
            context.Response.ContentType = "image/png"; // tipo real del placeholder
            context.Response.TransmitFile(phys);
        }

        public bool IsReusable => false;
    }
}
