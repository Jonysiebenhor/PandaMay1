using System;
using System.Data.SqlClient;
using System.Web;

namespace PandaMay
{
    public class VerImagen : IHttpHandler
    {
        // Ruta al placeholder (imagen por defecto)
        private const string PLACEHOLDER_VIRTUAL = "~/uploads/productos/aretes.png";

        // Cadena de conexión idéntica a la de tu Conectar.cs
        private const string CONN_STR =
            "workstation id = PandaMay.mssql.somee.com; packet size = 4096; " +
            "user id = Jonysiebenhor_SQLLogin_1; pwd = 9btgzhlyqy; " +
            "data source = PandaMay.mssql.somee.com; persist security info=False; " +
            "initial catalog = PandaMay; TrustServerCertificate=True";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // 1) Si recibimos imgid, devolvemos esa imagen
                if (int.TryParse(context.Request.QueryString["imgid"], out int imgId) && imgId > 0)
                {
                    var foto = ObtenerFotoPorIdImagen(imgId);
                    if (foto?.Length > 0)
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "image/jpeg";
                        context.Response.BinaryWrite(foto);
                        SetNoCache(context);
                        return;
                    }
                }

                // 2) Si recibimos id (producto) y detalle=1, buscamos la última imagen de ese producto
                if (int.TryParse(context.Request.QueryString["id"], out int prodId)
                    && context.Request.QueryString["detalle"] == "1")
                {
                    var lastImg = ObtenerUltimoIdImagenPorProducto(prodId);
                    if (lastImg.HasValue)
                    {
                        var foto = ObtenerFotoPorIdImagen(lastImg.Value);
                        if (foto?.Length > 0)
                        {
                            context.Response.Clear();
                            context.Response.ContentType = "image/jpeg";
                            context.Response.BinaryWrite(foto);
                            SetNoCache(context);
                            return;
                        }
                    }
                }

                // 3) Si todo lo anterior falla, enviamos el placeholder
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

        /// <summary>
        /// Saca el blob de la tabla IMAGENES por idimagen
        /// </summary>
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

        /// <summary>
        /// Obtiene el id de la última imagen asociada a un producto (si existe)
        /// </summary>
        private static int? ObtenerUltimoIdImagenPorProducto(int idProd)
        {
            const string sql = @"
WITH C AS (
  SELECT idimagen,
         ROW_NUMBER() OVER(
           PARTITION BY idproducto
           ORDER BY fechaingreso DESC, idexistencia DESC
         ) AS rn
  FROM dbo.EXISTENCIAS
  WHERE idproducto = @pid
    AND idimagen   IS NOT NULL
)
SELECT TOP(1) idimagen
  FROM C
 WHERE rn = 1;";
            using (var cn = new SqlConnection(CONN_STR))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@pid", idProd);
                cn.Open();
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? (int?)null : Convert.ToInt32(o);
            }
        }

        /// <summary>
        /// Evita caché en el navegador
        /// </summary>
        private static void SetNoCache(HttpContext ctx)
        {
            ctx.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ctx.Response.Cache.SetNoStore();
            ctx.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        }

        /// <summary>
        /// Envía la imagen placeholder
        /// </summary>
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
