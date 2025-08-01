//using iTextSharp.text.pdf.codec.wmf;
//using NPOI.SS.Formula.Functions;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Security.Policy;
using System.Web.UI.WebControls;
//using System.Windows.Controls.Primitives;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//using static QRCoder.PayloadGenerator.SwissQrCode;

namespace PandaMay
{
    public class Conectar
    {
        SqlConnection conexion = new SqlConnection();
        //conexion para la pc de JONY//String conexionString = "Data Source=DESKTOP-KNTJ3BG\\SQLEXPRESS;DATABASE=PandaMay;Integrated security=true";

        //conexion para el servidor
        String conexionString = "workstation id = PandaMay.mssql.somee.com; packet size = 4096; user id = Jonysiebenhor_SQLLogin_1; pwd=9btgzhlyqy;data source = PandaMay.mssql.somee.com; persist security info=False;initial catalog = PandaMay; TrustServerCertificate=True";


        public void conectar()
        {
            try
            {
                conexion.ConnectionString = conexionString;
                conexion.Open();
            }
            catch
            {
                //MessageBox.Show("Error en Conexion");
            }
        }
        internal void Open()
        {
            throw new NotImplementedException();
        }

        public void desconectar()
        {
            try
            {
                conexion.ConnectionString = conexionString;
                conexion.Close();
            }
            catch
            {
                //MessageBox.Show("Error en Conexion");
            }
        }
        public DataTable consultaUsuarioloign(String usuario)
        {
            String query = "Select *  from usuarios where usuario ='" + usuario + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable consultaUsuarioloign1(String contraseña)
        {
            String query = "Select *  from usuarios where contraseña ='" + contraseña + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable consultaUsuarioloign2(String usuario)
        {
            String query = "Select b.idusuario from usuarios a right join CLIENTES b on a.idusuario=b.idusuario where a.usuario ='" + usuario + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable consultaUsuarioloign3(String usuario)
        {
            String query = "Select b.idusuario as proveedor from usuarios a right join REVENDEDORES b on a.idusuario=b.idusuario where a.usuario ='" + usuario + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable consultaUsuarioloign4(String usuario)
        {
            String query = "Select b.idusuario as proveedor from usuarios a right join EMPLEADOS b on a.idusuario=b.idusuario where a.usuario ='" + usuario + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable empleadospuestos(String usuario)
        {
            String query = "Select z.idpuesto from puestos z left join  empleadospuestos a on z.idpuesto=a.idpuesto left join empleados b on a.idempleado=b.idempleado left join usuarios c on b.idusuario=c.idusuario where c.usuario ='" + usuario + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable Productos()
        {
            const string query = @"
WITH UltimaExistencia AS (
    -- Para cada producto, la existencia más reciente
    SELECT
      idproducto,
      idexistencia,
      ROW_NUMBER() OVER(
        PARTITION BY idproducto
        ORDER BY fechaingreso DESC, idexistencia DESC
      ) AS rn
    FROM dbo.EXISTENCIAS
),
UltimaImagen AS (
    -- Igual que antes, pero filtrada a la existencia más reciente
    SELECT
      e.idproducto,
      e.idexistencia,
      ex.idimagen,
      ROW_NUMBER() OVER(
        PARTITION BY e.idproducto
        ORDER BY ex.fechaingreso DESC, ex.idexistencia DESC
      ) AS rn
    FROM dbo.EXISTENCIAS ex
    JOIN UltimaExistencia e
      ON ex.idproducto = e.idproducto AND ex.idexistencia = e.idexistencia
    WHERE ex.idimagen IS NOT NULL
),
PreciosPorProducto AS (
    SELECT 
      e.idproducto,
      np.nombre      AS tarifa,
      pr.precio
    FROM dbo.PRECIOS pr
    JOIN dbo.EXISTENCIAS e     ON pr.idexistencia   = e.idexistencia
    JOIN dbo.nombresPrecios np ON pr.idnombreprecio = np.idnombreprecio
    WHERE pr.activo = 1
),
ExistenciaPublico AS (
    SELECT
      ep.idexistencia,
      p.nombre AS Publico
    FROM dbo.EXISTENCIASPUBLICOS ep
    JOIN dbo.PUBLICOS p
      ON ep.idpublico = p.idpublico
    WHERE ep.activo = 1
)
SELECT
  p.idproducto,
  ui.idimagen,
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN pr.tarifa = 'unidad'  THEN pr.precio END)   AS unidad,
  MAX(CASE WHEN pr.tarifa = '3 o más' THEN pr.precio END)   AS tresomas,
  MAX(CASE WHEN pr.tarifa = 'docena'  THEN pr.precio END)   AS docena,
  MAX(CASE WHEN pr.tarifa = 'fardo'   THEN pr.precio END)   AS fardo,
  p.nombre,
  pub.Publico
FROM dbo.PRODUCTOS p
LEFT JOIN UltimaImagen ui
  ON ui.idproducto = p.idproducto AND ui.rn = 1
LEFT JOIN PreciosPorProducto pr
  ON pr.idproducto = p.idproducto
LEFT JOIN ExistenciaPublico pub
  ON pub.idexistencia = ui.idexistencia
GROUP BY
  p.idproducto,
  ui.idimagen,
  p.codigodebarras,
  p.referencia,
  p.nombre,
  pub.Publico
ORDER BY p.nombre;
";

            var dt = new DataTable();
            using (var conn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(query, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }




        public DataTable productos() => Productos();

        /// <summary>
        /// Devuelve el listado completo de productos con todas sus tarifas
        /// (unidad, 3+, docena, fardo y cualquier columna extra).
        /// </summary>
        public DataTable GetProductosConTarifas()
        {
            const string sql = @"
WITH UltimaImagen AS (
    -- Para cada producto, la existencia más reciente (fechaingreso DESC, idexistencia DESC)
    SELECT
      idproducto,
      idimagen,
      ROW_NUMBER() OVER(
        PARTITION BY idproducto
        ORDER BY fechaingreso DESC, idexistencia DESC
      ) AS rn
    FROM dbo.EXISTENCIAS
    WHERE idimagen IS NOT NULL
),
PreciosPorProducto AS (
    SELECT 
      e.idproducto,
      np.nombre      AS tarifa,
      pr.precio
    FROM dbo.PRECIOS pr
    JOIN dbo.EXISTENCIAS e     ON pr.idexistencia   = e.idexistencia
    JOIN dbo.nombresPrecios np ON pr.idnombreprecio = np.idnombreprecio
    WHERE pr.activo = 1
)
SELECT
  p.idproducto,
  ui.idimagen,                    -- 1) TRAEMOS AQUÍ EL IDDEIMAGEN
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN ppp.tarifa = 'unidad'  THEN ppp.precio END)   AS unidad,
  MAX(CASE WHEN ppp.tarifa = '3 o más' THEN ppp.precio END)   AS tresomas,
  MAX(CASE WHEN ppp.tarifa = 'docena'  THEN ppp.precio END)   AS docena,
  MAX(CASE WHEN ppp.tarifa = 'fardo'   THEN ppp.precio END)   AS fardo,
  p.nombre
FROM dbo.PRODUCTOS p
LEFT JOIN UltimaImagen ui
  ON ui.idproducto = p.idproducto
 AND ui.rn = 1
LEFT JOIN PreciosPorProducto ppp
  ON ppp.idproducto = p.idproducto
GROUP BY
  p.idproducto,
  ui.idimagen,                    -- 2) Y TAMBIÉN EN EL GROUP BY
  p.codigodebarras,
  p.referencia,
  p.nombre
ORDER BY p.nombre;";

            var dt = new DataTable();
            using (var conn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Busca productos cuyo nombre, referencia o código de barras contenga el filtro,
        /// devolviendo también los mismos cuatro precios.
        /// </summary>
        public DataTable BuscarProductosConTarifas(string filtro)
        {
            const string sql = @"
WITH UltimaImagen AS (
    -- Seleccionamos la existencia más reciente con imagen para cada producto
    SELECT
      idproducto,
      idimagen,
      ROW_NUMBER() OVER(
        PARTITION BY idproducto
        ORDER BY fechaingreso DESC, idexistencia DESC
      ) AS rn
    FROM dbo.EXISTENCIAS
    WHERE idimagen IS NOT NULL
),
PreciosPorProducto AS (
    SELECT 
      e.idproducto,
      np.nombre      AS tarifa,
      pr.precio
    FROM dbo.PRECIOS pr
    JOIN dbo.EXISTENCIAS e     ON pr.idexistencia   = e.idexistencia
    JOIN dbo.nombresPrecios np ON pr.idnombreprecio = np.idnombreprecio
    WHERE pr.activo = 1
)
SELECT
  p.idproducto,
  ui.idimagen,                   -- <-- Ahora traemos el id de la imagen
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN ppp.tarifa = 'unidad'  THEN ppp.precio END)   AS unidad,
  MAX(CASE WHEN ppp.tarifa = '3 o más' THEN ppp.precio END)   AS tresomas,
  MAX(CASE WHEN ppp.tarifa = 'docena'  THEN ppp.precio END)   AS docena,
  MAX(CASE WHEN ppp.tarifa = 'fardo'   THEN ppp.precio END)   AS fardo,
  p.nombre
FROM dbo.PRODUCTOS p
LEFT JOIN UltimaImagen ui
  ON ui.idproducto = p.idproducto AND ui.rn = 1
LEFT JOIN PreciosPorProducto ppp
  ON ppp.idproducto = p.idproducto
WHERE
  p.nombre         LIKE @busq OR
  p.referencia     LIKE @busq OR
  p.codigodebarras LIKE @busq
GROUP BY
  p.idproducto,
  ui.idimagen,                   -- <-- y aquí también para el GROUP BY
  p.codigodebarras,
  p.referencia,
  p.nombre
ORDER BY p.nombre;";

            var dt = new DataTable();
            using (var conn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@busq", $"%{filtro}%");
                conn.Open();
                da.Fill(dt);
            }

            return dt;
        }


        public DataTable tiendasempleados(String usuario)
        {
            String query = "select a.nombre from tiendas a left join empleadostiendas b on a.idtienda=b.idtienda left join empleados c on b.idempleado=c.idempleado left join usuarios d on c.idusuario=d.idusuario where d.usuario='" + usuario + "' order by a.nombre asc";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable buscarproducto(string buscar)
        {
            const string query = @"
WITH PreciosPorProducto AS (
  SELECT 
    e.idproducto,
    np.nombre      AS tarifa,
    pr.precio
  FROM precios pr
  JOIN existencias e     ON pr.idexistencia    = e.idexistencia
  JOIN nombresPrecios np ON pr.idnombreprecio  = np.idnombreprecio
  WHERE pr.activo = 1
)
SELECT
  p.idproducto,
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN ppp.tarifa = 'unidad'  THEN ppp.precio END)   AS unidad,
  MAX(CASE WHEN ppp.tarifa = '3 o más' THEN ppp.precio END)   AS tresomas,
  MAX(CASE WHEN ppp.tarifa = 'docena'  THEN ppp.precio END)   AS docena,
  MAX(CASE WHEN ppp.tarifa = 'fardo'   THEN ppp.precio END)   AS fardo,
  p.nombre
FROM productos p
LEFT JOIN PreciosPorProducto ppp ON ppp.idproducto = p.idproducto
WHERE
  p.nombre        LIKE @busc OR
  p.referencia    LIKE @busc OR
  p.codigodebarras LIKE @busc
GROUP BY
  p.idproducto,
  p.codigodebarras,
  p.referencia,
  p.nombre
ORDER BY p.nombre;";


            using (var cmd = new SqlCommand(query, conexion))
            {
                cmd.Parameters.AddWithValue("@busc", $"%{buscar}%");
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }




        public DataTable buscarproducto1(String nombre)
        {
            String query = "select a.nombre, b.precio from productos a left join precios b on a.idproducto=b.idproducto where a.nombre= '" + nombre + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable cuentabanco()
        {
            String query = "select*from bancos where idusuario=4";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable insertarproductos(String nombre)
        {
            String query = "select a.nombre, b.precio from productos a left join precios b on a.idproducto=b.idproducto where a.nombre= '" + nombre + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable ventas(String idcliente, String idempleado, String idtienda, String tipocomprobante, String estado, String referencia)
        {
            string query = "INSERT INTO VENTAS" +
                            "(" +
                            "idcliente," +
                            "idempleado," +
                            "idtienda," +
                            "fecha," +
                            "tipocomprobante," +
                            "estado," +
                            "referencia)" +
                            "VALUES" +
                            "('" + idcliente + "'," +
                            "'" + idempleado + "'," +
                            "'" + idtienda + "'," +
                            "GETDATE()," +
                            "'" + tipocomprobante + "'," +
                            "'" + estado + "'," +
                            "'" + referencia + "')";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable usuarios(String usuario, String dpi, String primerNombre, String segundoNombre, String tercerNombre, String primerApellido, String segundoApellido, String apellidoCasada, String nombrenegocio, String producto, String idioma, String correo, String nit, String telefono, String nacimiento, String genero, String activo, String nombrebanco, String tipocuenta, String cuentabancaria, String contraseña)
        {
            string query = "INSERT INTO USUARIOS" +

                            "(" +
                            "usuario," +
                            "dpi," +
                            "primerNombre," +
                            "segundoNombre," +
                            "tercerNombre," +
                            "primerApellido," +
                            "segundoApellido," +
                            "apellidoCasada," +
                            "nombrenegocio," +
                            "producto," +
                            "idioma," +
                             "correo," +
                            "nit," +
                            "telefono," +
                            "nacimiento," +
                            "genero," +
                            "activo," +
                            "fechaingreso," +
                            "nombrebanco," +
                            "tipocuenta," +
                            "cuentabancaria," +
                            "contraseña)" +
                            "VALUES" +
                            "('" + usuario + "'," +
                            "'" + dpi + "'," +
                            "'" + primerNombre + "'," +
                             "'" + segundoNombre + "'," +
                            "'" + tercerNombre + "'," +
                             "'" + primerApellido + "'," +
                            "'" + segundoApellido + "'," +
                             "'" + apellidoCasada + "'," +
                            "'" + nombrenegocio + "'," +
                             "'" + producto + "'," +
                            "'" + idioma + "'," +
                            "'" + correo + "'," +
                            "'" + nit + "'," +
                            "'" + telefono + "'," +
                            "'" + nacimiento + "'," +
                            "'" + genero + "'," +
                            "'" + activo + "'," +
                            "GETDATE()," +
                            "'" + nombrebanco + "'," +
                            "'" + tipocuenta + "'," +
                            "'" + cuentabancaria + "'," +
                            "'" + contraseña + "')";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable ultimousuario()
        {
            String query = "Select*  from usuarios where idusuario =SCOPE_IDENTITY()";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable clientes(String idusuario, String descuento, String tarifa, String codigodebarras)
        {
            string query = "INSERT INTO CLIENTES" +

                            "(" +
                            "idusuario," +
                            "fechaprimeraventa," +
                            "fechaultimaventa," +
                            "descuento," +
                            "tarifa," +
                            "codigodebarras)" +
                            "VALUES" +
                            "('" + idusuario + "'," +
                            "GETDATE()," +
                            "GETDATE()," +
                            "'" + descuento + "'," +
                            "'" + tarifa + "'," +
                            "'" + codigodebarras + "')";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable ultimocliente()
        {
            String query = "Select*  from clientes where idcliente =SCOPE_IDENTITY()";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable direcciones(String idcliente, String idpais, String iddepartamento, String idmunicipio, String idzona, String idtarifa, String idempleado, String idtienda, String idmensajeria, String direccion)
        {
            string query = "INSERT INTO DIRECCIONES" +

                            "(" +
                            "idcliente," +
                            "idpais," +
                            "iddepartamento," +
                            "idmunicipio," +
                            "idzona," +
                            "idtarifa," +
                            "idempleado," +
                            "idtienda," +
                            "idmensajeria," +
                             "direccion," +
                            "fechahora)" +
                            "VALUES" +
                            "('" + idcliente + "'," +
                            "'" + idpais + "'," +
                            "'" + iddepartamento + "'," +
                             "'" + idmunicipio + "'," +
                            "'" + idzona + "'," +
                             "'" + idtarifa + "'," +
                            "'" + idempleado + "'," +
                             "'" + idtienda + "'," +
                             "'" + idmensajeria + "'," +
                            "'" + direccion + "'," +
                            "GETDATE())";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable tarifas(String idzona)
        {
            String query = "select* from tarifas a left join zonas b on a.idtarifa = b.idtarifa where b.idzona  ='" + idzona + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }

        /// <summary>
        /// Devuelve todas las marcas activas ordenadas por nombre
        /// </summary>
        public DataTable GetMarcas()
        {
            const string sql = @"
SELECT idmarca, nombre
  FROM dbo.MARCAS
 WHERE activo = 1   -- o quita esta línea si no usas 'activo'
 ORDER BY nombre;
";
            var dt = new DataTable();
            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cn.Open();
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable pais(String iddepartamento)
        {
            String query = "Select *  from departamentos where iddepartamento ='" + iddepartamento + "'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable ultimaventa()
        {
            String query = "Select*  from ventas where idventa =SCOPE_IDENTITY()";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable pagos(String idventa, String idtienda, String tipopago, String total)
        {
            string query = "INSERT INTO DIRECCIONES" +

                            "(" +
                            "idventa," +
                            "idtienda," +
                            "tipopago," +
                            "fechahora," +
                            "total)" +
                            "VALUES" +
                            "('" + idventa + "'," +
                            "'" + idtienda + "'," +
                            "'" + tipopago + "'," +
                            "GETDATE()," +
                            "'" + total + "')";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
        }
        public DataTable GetByProducto(string tableName, int idProducto)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var t = tableName.Trim().ToUpperInvariant();

            switch (t)
            {
                case "ATRIBUTOS":
                case "COMBOSPRODUCTOS":
                case "DETALLESCOMPRAS":
                case "DETALLESTRASLADOS":
                case "DETALLESVENTAS":
                case "EXISTENCIAS":
                case "PRECIOS":
                case "PRECIOSCOMPRAS":
                case "PRODUCTOS":
                    break;
                default:
                    throw new ArgumentException($"Tabla no soportada o sin columna idproducto: {tableName}");
            }

            string sql;

            if (t == "PRECIOS")
            {
                sql = @"
SELECT 
    p.idprecio,
    e.idproducto,
    np.nombre AS descripcion,
    p.precio,
    np.cantidad,
    p.activo
FROM dbo.PRECIOS p
LEFT JOIN dbo.EXISTENCIAS e
  ON p.idexistencia = e.idexistencia
LEFT JOIN dbo.nombresPrecios np
  ON p.idnombreprecio = np.idnombreprecio
WHERE e.idproducto = @id;";
            }
            else if (t == "PRECIOSCOMPRAS")
            {
                sql = @"
SELECT
    p.idpreciocompra,
    e.idproducto,
    p.descripcion,
    p.precio,
    p.activo,
    p.fecha
FROM dbo.PRECIOSCOMPRAS p
LEFT JOIN dbo.EXISTENCIAS e
  ON p.idexistencia = e.idexistencia
WHERE e.idproducto = @id;";
            }
            else if (t == "ATRIBUTOS")
            {
                // Opción B: devolvemos vacio porque no existe esa tabla
                return new DataTable();
            }
            else
            {
                // COMBOSPRODUCTOS, DETALLESCOMPRAS, DETALLESTRASLADOS, DETALLESVENTAS,
                // EXISTENCIAS o PRODUCTOS
                sql = $"SELECT * FROM dbo.[{t}] WHERE idproducto = @id;";
            }

            try
            {
                using (var cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", idProducto);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error en tabla '{t}'. SQL: {sql}. Mensaje SQL: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Trae las existencias de un producto con su color e público asociado.
        /// </summary>
        public DataTable GetExistenciasConColorYPublico(int idProducto)
        {
            const string sql = @"
SELECT 
  e.idexistencia,
  COALESCE(c.nombre, '')   AS Color,
  COALESCE(pub.nombre, '') AS Publico,
  m.tipomedida             AS Medida,
  e.cantidad
FROM dbo.EXISTENCIAS e
LEFT JOIN dbo.MEDIDAS m 
  ON m.idmedida = e.idmedida
LEFT JOIN dbo.IMAGENES img 
  ON img.idimagen = e.idimagen
LEFT JOIN dbo.COLORES c 
  ON c.idcolor = img.idcolor
LEFT JOIN dbo.EXISTENCIASPUBLICOS ep 
  ON ep.idexistencia = e.idexistencia AND ep.activo = 1
LEFT JOIN dbo.PUBLICOS pub 
  ON pub.idpublico = ep.idpublico
WHERE e.idproducto = @idProducto
ORDER BY e.idexistencia;";

            var dt = new DataTable();
            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@idProducto", idProducto);
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataRow ObtenerProductoCompleto(int idProducto)
        {
            using (SqlConnection cn = new SqlConnection(conexionString)) 
                                                                          // ✅ correcto

            {
                cn.Open();

                SqlCommand cmd = new SqlCommand(@"
WITH UltimaExistencia AS (
  SELECT 
    idproducto,
    idmarca,
    ROW_NUMBER() OVER(
      PARTITION BY idproducto
      ORDER BY fechaingreso DESC, idexistencia DESC
    ) AS rn
  FROM dbo.EXISTENCIAS
)
SELECT 
  p.idproducto,
  p.nombre,
  p.referencia,
  p.codigodebarras,
  p.tipodeproducto,
  p.descuento,
  p.activo,
  u.nombre  AS unidad,
  m.nombre  AS marca,              -- ahora sí viene de la existencia
  s.nombre  AS subcategoria,
  c.nombre  AS categoria,
  cm.nombre AS categoriamaestra
FROM dbo.PRODUCTOS p
LEFT JOIN dbo.UNIDADDEMEDIDAS u 
  ON u.idunidaddemedida = p.idunidaddemedida

LEFT JOIN UltimaExistencia ue
  ON ue.idproducto = p.idproducto 
 AND ue.rn = 1

LEFT JOIN dbo.MARCAS m
  ON m.idmarca = ue.idmarca        -- reemplaza p.idmarca

LEFT JOIN dbo.SUBCATEGORIAS s 
  ON s.idsubcategoria = p.idsubcategoria
LEFT JOIN dbo.CATEGORIASSUBCATEGORIAS cs 
  ON cs.idsubcategoria = s.idsubcategoria
LEFT JOIN dbo.CATEGORIAS c 
  ON c.idcategoria = cs.idcategoria
LEFT JOIN dbo.CATEGORIASMAESTRASCATEGORIAS cmc 
  ON cmc.idcategoria = c.idcategoria
LEFT JOIN dbo.CATEGORIASMAESTRAS cm 
  ON cm.idcategoriamaestra = cmc.idcategoriamaestra

WHERE p.idproducto = @id;
", cn);


                cmd.Parameters.AddWithValue("@id", idProducto);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }




    }
}