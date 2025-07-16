using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PandaMay.Productos
{
    public partial class CrearProducto : Page
    {
        private readonly string _connString =
            ConfigurationManager.ConnectionStrings["ConnectionString4"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarListas();
        }

        private void CargarListas()
        {
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

                // Subcategorías
                ddlSubcategoria.Items.Clear();
                ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
                using (var cmd = new SqlCommand(
                    "SELECT idsubcategoria,nombre FROM dbo.SUBCATEGORIAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlSubcategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idsubcategoria"].ToString()));

                // Tiendas
                ddlTienda.Items.Clear();
                ddlTienda.Items.Add(new ListItem("-- Tienda --", ""));
                using (var cmd2 = new SqlCommand(
                    "SELECT idtienda,nombre FROM dbo.TIENDAS WHERE activo=1", cn))
                using (var dr2 = cmd2.ExecuteReader())
                    while (dr2.Read())
                        ddlTienda.Items.Add(new ListItem(
                            dr2["nombre"].ToString(),
                            dr2["idtienda"].ToString()));

                // Unidades
                ddlUnidad.Items.Clear();
                ddlUnidad.Items.Add(new ListItem("-- Unidad --", ""));
                using (var cmd3 = new SqlCommand(
                    "SELECT idunidaddemedida,nombre FROM dbo.UNIDADDEMEDIDAS WHERE activo=1", cn))
                using (var dr3 = cmd3.ExecuteReader())
                    while (dr3.Read())
                        ddlUnidad.Items.Add(new ListItem(
                            dr3["nombre"].ToString(),
                            dr3["idunidaddemedida"].ToString()));
                ddlUnidad.Items.Add(new ListItem("-- Agregar nueva unidad --", "0"));

                // Marcas
                ddlMarca.Items.Clear();
                ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
                using (var cmd4 = new SqlCommand(
                    "SELECT idmarca,nombre FROM dbo.MARCAS WHERE activo=1", cn))
                using (var dr4 = cmd4.ExecuteReader())
                    while (dr4.Read())
                        ddlMarca.Items.Add(new ListItem(
                            dr4["nombre"].ToString(),
                            dr4["idmarca"].ToString()));
                ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));

                // Colores para la imagen (sin filtrar por columna inexistente)
                ddlImgColor.Items.Clear();
                ddlImgColor.Items.Add(new ListItem("-- Color imagen --", ""));
                using (var cmd5 = new SqlCommand(
                    "SELECT idcolor,nombre FROM dbo.COLORES", cn))
                using (var dr5 = cmd5.ExecuteReader())
                    while (dr5.Read())
                        ddlImgColor.Items.Add(new ListItem(
                            dr5["nombre"].ToString(),
                            dr5["idcolor"].ToString()));
            }
        }

        protected void ddlUnidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddUnidad.Visible = ddlUnidad.SelectedValue == "0";
        }

        protected void btnGuardarUnidad_Click(object sender, EventArgs e)
        {
            var nueva = txtNewUnidad.Text.Trim();
            if (string.IsNullOrEmpty(nueva)) return;

            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.UNIDADDEMEDIDAS(nombre,activo,fecha) " +
                "OUTPUT INSERTED.idunidaddemedida " +
                "VALUES(@n,1,GETDATE())", cn))
            {
                cmd.Parameters.AddWithValue("@n", nueva);
                cn.Open();
                idNew = (int)cmd.ExecuteScalar();
            }

            CargarListas();
            ddlUnidad.SelectedValue = idNew.ToString();
            pnlAddUnidad.Visible = false;
            txtNewUnidad.Text = "";
        }

        protected void ddlMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddMarca.Visible = ddlMarca.SelectedValue == "0";
        }

        protected void btnGuardarMarca_Click(object sender, EventArgs e)
        {
            var nueva = txtNewMarca.Text.Trim();
            if (string.IsNullOrEmpty(nueva)) return;

            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.MARCAS(nombre,activo,fecha) " +
                "OUTPUT INSERTED.idmarca " +
                "VALUES(@n,1,GETDATE())", cn))
            {
                cmd.Parameters.AddWithValue("@n", nueva);
                cn.Open();
                idNew = (int)cmd.ExecuteScalar();
            }

            CargarListas();
            ddlMarca.SelectedValue = idNew.ToString();
            pnlAddMarca.Visible = false;
            txtNewMarca.Text = "";
        }

        protected void btnGuardarProducto_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            lblError.Visible = false;

            // Lectura de campos
            int subcat = int.TryParse(ddlSubcategoria.SelectedValue, out var sc) ? sc : 0;
            if (!int.TryParse(ddlUnidad.SelectedValue, out int unidad) || unidad == 0)
            { MostrarError("Seleccione o cree una unidad válida."); return; }
            if (!int.TryParse(ddlMarca.SelectedValue, out int marca) || marca == 0)
            { MostrarError("Seleccione o cree una marca válida."); return; }

            var nombre = txtNombre.Text.Trim();
            var referencia = txtReferencia.Text.Trim();
            var cb = long.TryParse(txtCodigoBarras.Text.Trim(), out var tmpcb) ? tmpcb : 0L;
            var activo = chkActivo.Checked;
            var tipoProd = txtTipo.Text.Trim();
            var descuento = float.TryParse(txtDescuento.Text.Trim(), out var d) ? d : 0f;

            // Dinámicos
            var tarifas = Request.Form.GetValues("tarifa") ?? new string[0];
            var aplicaEn = Request.Form.GetValues("aplicaEn") ?? new string[0];
            var cantMin = Request.Form.GetValues("cantMin") ?? new string[0];
            var precioVal = Request.Form.GetValues("precioVal") ?? new string[0];

            var exColor = Request.Form.GetValues("exColor") ?? new string[0];
            var exMedida = Request.Form.GetValues("exMedida") ?? new string[0];
            var exCant = Request.Form.GetValues("exCant") ?? new string[0];

            // Imagen
            byte[] imgBytes = null;
            if (fuImagen.HasFile)
            {
                using (var ms = new MemoryStream())
                {
                    fuImagen.PostedFile.InputStream.CopyTo(ms);
                    imgBytes = ms.ToArray();
                }
            }

            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) PRODUCTOS
                        int newId;
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.PRODUCTOS
 (idsubcategoria,idunidaddemedida,idmarca,nombre,fecha,
  referencia,codigodebarras,activo,tipodeproducto,descuento)
 OUTPUT INSERTED.idproducto
 VALUES
 ( @sub,@uni,@mar,@nom,GETDATE(),@ref,@cb,@act,@tip,@des )",
                            cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@sub", sc > 0 ? (object)sc : DBNull.Value);
                            cmd.Parameters.AddWithValue("@uni", unidad);
                            cmd.Parameters.AddWithValue("@mar", marca);
                            cmd.Parameters.AddWithValue("@nom", nombre);
                            cmd.Parameters.AddWithValue("@ref", referencia);
                            cmd.Parameters.AddWithValue("@cb", cb);
                            cmd.Parameters.AddWithValue("@act", activo);
                            cmd.Parameters.AddWithValue("@tip", tipoProd);
                            cmd.Parameters.AddWithValue("@des", descuento);
                            newId = (int)cmd.ExecuteScalar();
                        }

                        // 2) PRECIOS
                        for (int i = 0; i < tarifas.Length; i++)
                        {
                            using (var cmd2 = new SqlCommand(@"
INSERT INTO dbo.PRECIOS
 (idproducto,nombre,descripcion,precio,cantidad,activo,fecha)
 VALUES
 (@id,@nom,@desc,@pre,@cant,1,GETDATE())",
                                cn, tx))
                            {
                                cmd2.Parameters.AddWithValue("@id", newId);
                                cmd2.Parameters.AddWithValue("@nom", tarifas[i]);
                                cmd2.Parameters.AddWithValue("@desc", aplicaEn[i]);
                                cmd2.Parameters.AddWithValue("@pre", decimal.Parse(precioVal[i]));
                                cmd2.Parameters.AddWithValue("@cant", int.Parse(cantMin[i]));
                                cmd2.ExecuteNonQuery();
                            }
                        }

                        // 3) EXISTENCIAS
                        for (int i = 0; i < exColor.Length; i++)
                        {
                            using (var cmd3 = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIAS
 (idtienda,idproducto,idcolor,idmedida,cantidad,fechaingreso)
 VALUES
 (@tid,@id,@col,@med,@can,GETDATE())",
                                cn, tx))
                            {
                                cmd3.Parameters.AddWithValue("@tid", int.Parse(ddlTienda.SelectedValue));
                                cmd3.Parameters.AddWithValue("@id", newId);
                                cmd3.Parameters.AddWithValue("@col", exColor[i]);
                                cmd3.Parameters.AddWithValue("@med", exMedida[i]);
                                cmd3.Parameters.AddWithValue("@can", int.Parse(exCant[i]));
                                cmd3.ExecuteNonQuery();
                            }
                        }

                        // 4) IMAGEN
                        if (imgBytes != null)
                        {
                            using (var cmd4 = new SqlCommand(@"
INSERT INTO dbo.IMAGENES
 (idcolor,foto,descripcion,activo,fecha)
 VALUES
 (@col,@bin,@desc,1,GETDATE())",
                                cn, tx))
                            {
                                if (int.TryParse(ddlImgColor.SelectedValue, out var imgCid) && imgCid > 0)
                                    cmd4.Parameters.AddWithValue("@col", imgCid);
                                else
                                    cmd4.Parameters.AddWithValue("@col", DBNull.Value);

                                cmd4.Parameters.AddWithValue("@bin", imgBytes);
                                cmd4.Parameters.AddWithValue("@desc", txtImgDesc.Text.Trim());
                                cmd4.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MostrarError("Ocurrió un error: " + ex.Message);
                        return;
                    }
                }
            }

            Response.Redirect("Productos.aspx");
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Productos.aspx");
        }

        private void MostrarError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
        }
    }
}
