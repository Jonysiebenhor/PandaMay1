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
        public DataTable consultaUsuarioloign(string usuario)
        {
            const string sql = "SELECT * FROM dbo.USUARIOS WHERE usuario = @usuario;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value =
                    (object)usuario ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable consultaUsuarioloign1(string contraseña)
        {
            const string sql = "SELECT * FROM dbo.USUARIOS WHERE [contraseña] = @pass;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@pass", SqlDbType.NVarChar, 256).Value =
                    (object)contraseña ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable consultaUsuarioloign2(string usuario)
        {
            const string sql = @"
        SELECT b.idusuario
        FROM dbo.USUARIOS a
        RIGHT JOIN dbo.CLIENTES b ON a.idusuario = b.idusuario
        WHERE a.usuario = @usuario;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value =
                    (object)usuario ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable consultaUsuarioloign3(string usuario)
        {
            const string sql = @"
        SELECT b.idusuario AS proveedor
        FROM dbo.USUARIOS a
        RIGHT JOIN dbo.REVENDEDORES b ON a.idusuario = b.idusuario
        WHERE a.usuario = @usuario;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value =
                    (object)usuario ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable consultaUsuarioloign4(string usuario)
        {
            const string sql = @"
        SELECT b.idusuario AS proveedor
        FROM dbo.USUARIOS a
        RIGHT JOIN dbo.EMPLEADOS b ON a.idusuario = b.idusuario
        WHERE a.usuario = @usuario;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value =
                    (object)usuario ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable empleadospuestos(string usuario)
        {
            const string sql = @"
        SELECT z.idpuesto
        FROM dbo.PUESTOS z
        LEFT JOIN dbo.EMPLEADOSPUESTOS a ON z.idpuesto = a.idpuesto
        LEFT JOIN dbo.EMPLEADOS b        ON a.idempleado = b.idempleado
        LEFT JOIN dbo.USUARIOS c         ON b.idusuario = c.idusuario
        WHERE c.usuario = @usuario;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value =
                    (object)usuario ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable Productos()
        {
            const string query = @"
WITH UltimaExistencia AS (
  SELECT
    idproducto, idexistencia,
    ROW_NUMBER() OVER(PARTITION BY idproducto ORDER BY fechaingreso DESC, idexistencia DESC) AS rn
  FROM dbo.EXISTENCIAS
),
FotoYPublico AS (
  SELECT 
    ue.idproducto,
    ue.idexistencia,
    (SELECT TOP (1) ei.idimagen
       FROM dbo.EXISTENCIASIMAGENES ei
      WHERE ei.idexistencia = ue.idexistencia
      ORDER BY ei.idimagen DESC) AS idimagen,
    (SELECT TOP (1) p.nombre
       FROM dbo.EXISTENCIASPUBLICOS ep
       JOIN dbo.PUBLICOS p ON p.idpublico = ep.idpublico
      WHERE ep.idexistencia = ue.idexistencia AND ep.activo = 1
      ORDER BY p.nombre) AS Publico
  FROM UltimaExistencia ue
  WHERE ue.rn = 1
),
PreciosPorProducto AS (
  SELECT e.idproducto, np.nombre AS tarifa, pr.precio
  FROM dbo.PRECIOS pr
  JOIN dbo.EXISTENCIAS e     ON pr.idexistencia   = e.idexistencia
  JOIN dbo.NOMBRESPRECIOS np ON pr.idnombreprecio = np.idnombreprecio
  WHERE pr.activo = 1 AND np.activo = 1
)
SELECT
  p.idproducto,
  fyp.idimagen,
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN pr.tarifa = 'unidad'  THEN pr.precio END) AS unidad,
  MAX(CASE WHEN pr.tarifa = '3 o más' THEN pr.precio END) AS tresomas,
  MAX(CASE WHEN pr.tarifa = 'docena'  THEN pr.precio END) AS docena,
  MAX(CASE WHEN pr.tarifa = 'fardo'   THEN pr.precio END) AS fardo,
  p.nombre,
  fyp.Publico
FROM dbo.PRODUCTOS p
LEFT JOIN FotoYPublico     fyp ON fyp.idproducto = p.idproducto
LEFT JOIN PreciosPorProducto pr ON pr.idproducto  = p.idproducto
GROUP BY p.idproducto, fyp.idimagen, p.codigodebarras, p.referencia, p.nombre, fyp.Publico
ORDER BY p.nombre;";
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
WITH UltimaExistencia AS (
  SELECT
    idproducto,
    idexistencia,
    ROW_NUMBER() OVER(
      PARTITION BY idproducto
      ORDER BY fechaingreso DESC, idexistencia DESC
    ) AS rn
  FROM dbo.EXISTENCIAS
),
FotoPorProducto AS (
  SELECT 
    ue.idproducto,
    ue.idexistencia,
    (SELECT TOP (1) ei.idimagen
       FROM dbo.EXISTENCIASIMAGENES ei
      WHERE ei.idexistencia = ue.idexistencia
      ORDER BY ei.idimagen DESC) AS idimagen
  FROM UltimaExistencia ue
  WHERE ue.rn = 1
),
PreciosPorProducto AS (
  SELECT 
    e.idproducto,
    np.nombre AS tarifa,
    pr.precio
  FROM dbo.PRECIOS pr
  JOIN dbo.EXISTENCIAS e     ON pr.idexistencia   = e.idexistencia
  JOIN dbo.NOMBRESPRECIOS np ON pr.idnombreprecio = np.idnombreprecio
  WHERE pr.activo = 1 AND np.activo = 1
)
SELECT
  p.idproducto,
  fpp.idimagen,
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN ppp.tarifa = 'unidad'  THEN ppp.precio END) AS unidad,
  MAX(CASE WHEN ppp.tarifa = '3 o más' THEN ppp.precio END) AS tresomas,
  MAX(CASE WHEN ppp.tarifa = 'docena'  THEN ppp.precio END) AS docena,
  MAX(CASE WHEN ppp.tarifa = 'fardo'   THEN ppp.precio END) AS fardo,
  p.nombre
FROM dbo.PRODUCTOS p
LEFT JOIN FotoPorProducto    fpp ON fpp.idproducto = p.idproducto
LEFT JOIN PreciosPorProducto ppp ON ppp.idproducto = p.idproducto
GROUP BY
  p.idproducto, fpp.idimagen, p.codigodebarras, p.referencia, p.nombre
ORDER BY p.nombre;
";
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
WITH UltimaExistencia AS (
  SELECT
    idproducto,
    idexistencia,
    ROW_NUMBER() OVER(
      PARTITION BY idproducto
      ORDER BY fechaingreso DESC, idexistencia DESC
    ) AS rn
  FROM dbo.EXISTENCIAS
),
FotoPorProducto AS (
  SELECT 
    ue.idproducto,
    ue.idexistencia,
    (SELECT TOP (1) ei.idimagen
       FROM dbo.EXISTENCIASIMAGENES ei
      WHERE ei.idexistencia = ue.idexistencia
      ORDER BY ei.idimagen DESC) AS idimagen
  FROM UltimaExistencia ue
  WHERE ue.rn = 1
),
PreciosPorProducto AS (
  SELECT 
    e.idproducto,
    np.nombre AS tarifa,
    pr.precio
  FROM dbo.PRECIOS pr
  JOIN dbo.EXISTENCIAS e     ON pr.idexistencia   = e.idexistencia
  JOIN dbo.NOMBRESPRECIOS np ON pr.idnombreprecio = np.idnombreprecio
  WHERE pr.activo = 1 AND np.activo = 1
)
SELECT
  p.idproducto,
  fpp.idimagen,
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN ppp.tarifa = 'unidad'  THEN ppp.precio END) AS unidad,
  MAX(CASE WHEN ppp.tarifa = '3 o más' THEN ppp.precio END) AS tresomas,
  MAX(CASE WHEN ppp.tarifa = 'docena'  THEN ppp.precio END) AS docena,
  MAX(CASE WHEN ppp.tarifa = 'fardo'   THEN ppp.precio END) AS fardo,
  p.nombre
FROM dbo.PRODUCTOS p
LEFT JOIN FotoPorProducto    fpp ON fpp.idproducto = p.idproducto
LEFT JOIN PreciosPorProducto ppp ON ppp.idproducto = p.idproducto
WHERE p.nombre         LIKE @busq
   OR p.referencia     LIKE @busq
   OR p.codigodebarras LIKE @busq
GROUP BY
  p.idproducto, fpp.idimagen, p.codigodebarras, p.referencia, p.nombre
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

        public DataTable tiendasempleados(string usuario)
        {
            const string sql = @"
        SELECT a.nombre
        FROM dbo.TIENDAS a
        LEFT JOIN dbo.EMPLEADOSTIENDAS b ON a.idtienda  = b.idtienda
        LEFT JOIN dbo.EMPLEADOS c        ON b.idempleado = c.idempleado
        LEFT JOIN dbo.USUARIOS d         ON c.idusuario  = d.idusuario
        WHERE d.usuario = @usuario
        ORDER BY a.nombre ASC;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@usuario", SqlDbType.NVarChar, 100).Value =
                    (object)usuario ?? DBNull.Value;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable buscarproducto(string buscar)
        {
            const string query = @"
WITH PreciosPorProducto AS (
  SELECT 
    e.idproducto,
    np.nombre AS tarifa,
    pr.precio
  FROM dbo.PRECIOS pr
  JOIN dbo.EXISTENCIAS    e  ON pr.idexistencia   = e.idexistencia
  JOIN dbo.NOMBRESPRECIOS np ON pr.idnombreprecio = np.idnombreprecio
  WHERE pr.activo = 1 AND np.activo = 1
)
SELECT
  p.idproducto,
  p.codigodebarras,
  p.referencia,
  MAX(CASE WHEN ppp.tarifa = 'unidad'  THEN ppp.precio END) AS unidad,
  MAX(CASE WHEN ppp.tarifa = '3 o más' THEN ppp.precio END) AS tresomas,
  MAX(CASE WHEN ppp.tarifa = 'docena'  THEN ppp.precio END) AS docena,
  MAX(CASE WHEN ppp.tarifa = 'fardo'   THEN ppp.precio END) AS fardo,
  p.nombre
FROM dbo.PRODUCTOS p
LEFT JOIN PreciosPorProducto ppp ON ppp.idproducto = p.idproducto
WHERE p.nombre LIKE @busc OR p.referencia LIKE @busc OR p.codigodebarras LIKE @busc
GROUP BY p.idproducto, p.codigodebarras, p.referencia, p.nombre
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


        public DataTable buscarproducto1(string nombre)
        {
            const string sql = @"
SELECT p.nombre, pr.precio
FROM dbo.PRODUCTOS p
JOIN dbo.EXISTENCIAS e ON e.idproducto = p.idproducto
JOIN dbo.PRECIOS pr     ON pr.idexistencia = e.idexistencia AND pr.activo = 1
WHERE p.nombre = @nombre;";

            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = nombre;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
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
        public DataTable InsertarProductos(string nombre)   // (el nombre del método es confuso; esto NO inserta)
        {
            const string sql = @"
SELECT p.nombre, pr.precio
FROM dbo.PRODUCTOS p
JOIN dbo.EXISTENCIAS e ON e.idproducto = p.idproducto
JOIN dbo.PRECIOS pr    ON pr.idexistencia = e.idexistencia AND pr.activo = 1
WHERE p.nombre = @nombre;";

            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@nombre", SqlDbType.NVarChar, 150).Value = nombre;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable ventas(string idcliente, string idempleado, string idtienda,
                        string tipocomprobante, string estado, string referencia)
        {
            const string sql = @"
INSERT INTO dbo.VENTAS (idcliente, idempleado, idtienda, fecha, tipocomprobante, estado, referencia)
OUTPUT INSERTED.idventa
VALUES (@idcliente, @idempleado, @idtienda, GETDATE(), @tipocomprobante, @estado, @referencia);";

            int idCli = int.TryParse(idcliente, out var _idc) ? _idc : 0;
            int idEmp = int.TryParse(idempleado, out var _ide) ? _ide : 0;
            int idTi = int.TryParse(idtienda, out var _idt) ? _idt : 0;

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@idcliente", SqlDbType.Int).Value = idCli;
                cmd.Parameters.Add("@idempleado", SqlDbType.Int).Value = idEmp;
                cmd.Parameters.Add("@idtienda", SqlDbType.Int).Value = idTi;
                cmd.Parameters.Add("@tipocomprobante", SqlDbType.NVarChar, 40).Value = (object)tipocomprobante ?? DBNull.Value;
                cmd.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = (object)estado ?? DBNull.Value;
                cmd.Parameters.Add("@referencia", SqlDbType.NVarChar, 60).Value = (object)referencia ?? DBNull.Value;

                cn.Open();
                var idVenta = (int)cmd.ExecuteScalar();

                var dt = new DataTable();
                dt.Columns.Add("idventa", typeof(int));
                dt.Rows.Add(idVenta);
                return dt;
            }
        }

        public DataTable usuarios(string usuario, string dpi, string primerNombre, string segundoNombre, string tercerNombre,
                          string primerApellido, string segundoApellido, string apellidoCasada, string nombrenegocio,
                          string producto, string idioma, string correo, string nit, string telefono, string nacimiento,
                          string genero, string activo, string nombrebanco, string tipocuenta, string cuentabancaria,
                          string contraseña)
        {
            const string sql = @"
INSERT INTO dbo.USUARIOS
(usuario, dpi, primerNombre, segundoNombre, tercerNombre, primerApellido, segundoApellido, apellidoCasada,
 nombrenegocio, producto, idioma, correo, nit, telefono, nacimiento, genero, activo, fechaingreso,
 nombrebanco, tipocuenta, cuentabancaria, [contraseña])
OUTPUT INSERTED.idusuario
VALUES
(@usuario, @dpi, @p1, @p2, @p3, @a1, @a2, @ac, @neg, @prod, @idioma, @correo, @nit, @tel, @nac, @gen, @act, GETDATE(),
 @nbanco, @tc, @cta, @pass);";

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@usuario", usuario ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@dpi", dpi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p1", primerNombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p2", segundoNombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p3", tercerNombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@a1", primerApellido ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@a2", segundoApellido ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ac", apellidoCasada ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@neg", nombrenegocio ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@prod", producto ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@idioma", idioma ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@correo", correo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@nit", nit ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tel", telefono ?? (object)DBNull.Value);

                if (DateTime.TryParse(nacimiento, out var nac))
                    cmd.Parameters.Add("@nac", SqlDbType.DateTime).Value = nac;
                else
                    cmd.Parameters.Add("@nac", SqlDbType.DateTime).Value = DBNull.Value;

                // si tu columna 'activo' es bit, convierte:
                bool act = activo == "1" || activo?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
                cmd.Parameters.Add("@act", SqlDbType.Bit).Value = act;

                cmd.Parameters.AddWithValue("@nbanco", nombrebanco ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tc", tipocuenta ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cta", cuentabancaria ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pass", contraseña ?? (object)DBNull.Value);

                cn.Open();
                int idusuario = (int)cmd.ExecuteScalar();

                var dt = new DataTable();
                dt.Columns.Add("idusuario", typeof(int));
                dt.Rows.Add(idusuario);
                return dt;
            }
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
        public DataTable clientes(string idusuario, string descuento, string tarifa, string codigodebarras)
        {
            const string sql = @"
INSERT INTO dbo.CLIENTES (idusuario, fechaprimeraventa, fechaultimaventa, descuento, tarifa, codigodebarras)
OUTPUT INSERTED.idcliente
VALUES (@idusuario, GETDATE(), GETDATE(), @descuento, @tarifa, @cb);";

            int idu = int.TryParse(idusuario, out var _idu) ? _idu : 0;

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@idusuario", SqlDbType.Int).Value = idu;

                if (decimal.TryParse(descuento, out var dcto))
                {
                    cmd.Parameters.Add("@descuento", SqlDbType.Decimal).Precision = 18;
                    cmd.Parameters["@descuento"].Scale = 2;
                    cmd.Parameters["@descuento"].Value = dcto;
                }
                else
                {
                    cmd.Parameters.Add("@descuento", SqlDbType.Decimal).Value = DBNull.Value;
                }

                cmd.Parameters.AddWithValue("@tarifa", tarifa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cb", codigodebarras ?? (object)DBNull.Value);

                cn.Open();
                int idcliente = (int)cmd.ExecuteScalar();

                var dt = new DataTable();
                dt.Columns.Add("idcliente", typeof(int));
                dt.Rows.Add(idcliente);
                return dt;
            }
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
        public DataTable direcciones(string idcliente, string idpais, string iddepartamento, string idmunicipio,
                             string idzona, string idtarifa, string idempleado, string idtienda,
                             string idmensajeria, string direccion)
        {
            const string sql = @"
INSERT INTO dbo.DIRECCIONES
(idcliente, idpais, iddepartamento, idmunicipio, idzona, idtarifa, idempleado, idtienda, idmensajeria, direccion, fechahora)
OUTPUT INSERTED.iddireccion
VALUES (@idcliente, @idpais, @iddepartamento, @idmunicipio, @idzona, @idtarifa, @idempleado, @idtienda, @idmensajeria, @direccion, GETDATE());";

            int ToInt(string s) => int.TryParse(s, out var n) ? n : 0;

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@idcliente", SqlDbType.Int).Value = ToInt(idcliente);
                cmd.Parameters.Add("@idpais", SqlDbType.Int).Value = ToInt(idpais);
                cmd.Parameters.Add("@iddepartamento", SqlDbType.Int).Value = ToInt(iddepartamento);
                cmd.Parameters.Add("@idmunicipio", SqlDbType.Int).Value = ToInt(idmunicipio);
                cmd.Parameters.Add("@idzona", SqlDbType.Int).Value = ToInt(idzona);
                cmd.Parameters.Add("@idtarifa", SqlDbType.Int).Value = ToInt(idtarifa);
                cmd.Parameters.Add("@idempleado", SqlDbType.Int).Value = ToInt(idempleado);
                cmd.Parameters.Add("@idtienda", SqlDbType.Int).Value = ToInt(idtienda);
                cmd.Parameters.Add("@idmensajeria", SqlDbType.Int).Value = ToInt(idmensajeria);
                cmd.Parameters.AddWithValue("@direccion", direccion ?? (object)DBNull.Value);

                cn.Open();
                int iddireccion = (int)cmd.ExecuteScalar();

                var dt = new DataTable();
                dt.Columns.Add("iddireccion", typeof(int));
                dt.Rows.Add(iddireccion);
                return dt;
            }
        }

        public DataTable tarifas(string idzona)
        {
            const string sql = @"
        SELECT *
        FROM dbo.TARIFAS a
        LEFT JOIN dbo.ZONAS b ON a.idtarifa = b.idtarifa
        WHERE b.idzona = @idzona;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@idzona", SqlDbType.Int).Value =
                    int.TryParse(idzona, out var v) ? v : 0;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
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

        public DataTable pais(string iddepartamento)
        {
            const string sql = "SELECT * FROM dbo.DEPARTAMENTOS WHERE iddepartamento = @iddep;";
            using (var da = new SqlDataAdapter(sql, conexion))
            {
                da.SelectCommand.Parameters.Add("@iddep", SqlDbType.Int).Value =
                    int.TryParse(iddepartamento, out var v) ? v : 0;
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
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
        public int InsertarPago(int idventa, int idtienda, string tipopago, decimal total)
        {
            const string sql = @"
INSERT INTO dbo.PAGOS (idventa,idtienda,tipopago,fechahora,total)
VALUES (@idventa,@idtienda,@tipopago,GETDATE(),@total);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var cn = new SqlConnection(conexionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@idventa", SqlDbType.Int).Value = idventa;
                cmd.Parameters.Add("@idtienda", SqlDbType.Int).Value = idtienda;
                cmd.Parameters.Add("@tipopago", SqlDbType.NVarChar, 50).Value = (object)tipopago ?? DBNull.Value;
                cmd.Parameters.Add("@total", SqlDbType.Decimal).Precision = 18;
                cmd.Parameters["@total"].Scale = 2;
                cmd.Parameters["@total"].Value = total;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
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
    p.cantidad,
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
  COALESCE(col.nombre, '')   AS Color,
  COALESCE(pub.nombre, '')   AS Publico,
  md.tipomedida              AS Medida,   -- o md.medidasu si prefieres
  e.cantidad
FROM dbo.EXISTENCIAS e
LEFT JOIN dbo.MEDIDAS md ON md.idmedida = e.idmedida
OUTER APPLY (
  SELECT TOP (1) ei.idimagen
  FROM dbo.EXISTENCIASIMAGENES ei
  WHERE ei.idexistencia = e.idexistencia
  ORDER BY ei.idimagen DESC
) x
LEFT JOIN dbo.IMAGENES img ON img.idimagen = x.idimagen
LEFT JOIN dbo.COLORES col  ON col.idcolor  = img.idcolor
LEFT JOIN dbo.EXISTENCIASPUBLICOS ep ON ep.idexistencia = e.idexistencia AND ep.activo = 1
LEFT JOIN dbo.PUBLICOS pub         ON pub.idpublico    = ep.idpublico
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
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(@"
WITH UltimaExistencia AS (
  SELECT 
    idproducto,
    idmarca,
    idmedida,
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

  -- unidad: tomamos algo representativo de MEDIDAS
  COALESCE(md.medidasu, md.tipomedida, '') AS unidad,

  m.nombre  AS marca,
  s.nombre  AS subcategoria,
  c.nombre  AS categoria,
  cm.nombre AS categoriamaestra
FROM dbo.PRODUCTOS p
LEFT JOIN UltimaExistencia ue ON ue.idproducto = p.idproducto AND ue.rn = 1
LEFT JOIN dbo.MARCAS  m  ON m.idmarca   = ue.idmarca
LEFT JOIN dbo.MEDIDAS md ON md.idmedida = ue.idmedida
LEFT JOIN dbo.SUBCATEGORIAS s           ON s.idsubcategoria = p.idsubcategoria
LEFT JOIN dbo.CATEGORIASSUBCATEGORIAS cs ON cs.idsubcategoria = s.idsubcategoria
LEFT JOIN dbo.CATEGORIAS c               ON c.idcategoria    = cs.idcategoria
LEFT JOIN dbo.CATEGORIASMAESTRASCATEGORIAS cmc ON cmc.idcategoria = c.idcategoria
LEFT JOIN dbo.CATEGORIASMAESTRAS cm           ON cm.idcategoriamaestra = cmc.idcategoriamaestra
WHERE p.idproducto = @id;", cn);

                cmd.Parameters.AddWithValue("@id", idProducto);

                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }




    }
}