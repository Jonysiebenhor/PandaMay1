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
        public DataTable productos()
        {
            const string query = @"
SELECT 
  a.idproducto,
  a.codigodebarras,
  a.referencia,
  a.nombre,
  (
    SELECT b.precio 
    FROM precios b 
    WHERE b.nombre = 'unidad' 
      AND b.idproducto = a.idproducto
  ) AS unidad,
  (
    SELECT b.precio 
    FROM precios b 
    WHERE b.nombre = '3 o más' 
      AND b.idproducto = a.idproducto
  ) AS tresomas,
  (
    SELECT b.precio 
    FROM precios b 
    WHERE b.nombre = 'docena' 
      AND b.idproducto = a.idproducto
  ) AS docena,
  (
    SELECT b.precio 
    FROM precios b 
    WHERE b.nombre = 'fardo' 
      AND b.idproducto = a.idproducto
  ) AS fardo,
  -- Aquí traemos la foto directamente por idproducto
  i.foto
FROM productos a
LEFT JOIN imagenes i
  ON i.idproducto = a.idproducto
  AND i.activo = 1
ORDER BY a.nombre;
";

            using (var cmd = new SqlCommand(query, conexion))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
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
        public DataTable buscarproducto(String buscar)
        {
            String query = "select a.codigodebarras, a.referencia, a.nombre,(select b.precio from precios b left join productos c on b.idproducto=c.idproducto where b.nombre='unidad' and a.idproducto=c.idproducto) as unidad,(select b.precio from precios b left join productos c on b.idproducto=c.idproducto where b.nombre='3 o más' and a.idproducto=c.idproducto) as tresomas,(select b.precio from precios b left join productos c on b.idproducto=c.idproducto where b.nombre='docena' and a.idproducto=c.idproducto) as docena, (select b.precio from precios b left join productos c on b.idproducto=c.idproducto where b.nombre='fardo' and a.idproducto=c.idproducto) as fardo, c.foto from productos a  left join existencias j on a.idproducto = j.idproducto left join colores k on j.idcolor=k.idcolor left join imagenes c on k.idcolor = c.idcolor where a.nombre like '%" + buscar + "%' or a.referencia like '%" + buscar + "%' or a.codigodebarras like '%" + buscar + "%'";

            SqlCommand cmd = new SqlCommand(query, conexion);
            SqlDataAdapter returnVal = new SqlDataAdapter(query, conexion);
            DataTable dt = new DataTable();
            returnVal.Fill(dt);
            return dt;
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
        public DataTable direcciones(String idcliente, String idpais, String iddepartamento, String idmunicipio, String idzona, String idtarifa, String idempleado, String idtienda, String idmensajeria,  String direccion)
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
        public DataTable GetByProducto(string tableName, int idproducto)
        {
            string sql = $"SELECT * FROM {tableName} WHERE idproducto = @id";
            using (var cmd = new SqlCommand(sql, conexion))
            {
                cmd.Parameters.AddWithValue("@id", idproducto);
                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    } }