﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;           // para Min()
using System.Text;
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
            {
                CargarListas();
                RegistrarOpcionesMedidas();
            }
        }

        private void CargarListas()
        {
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

                // 1) SUBCATEGORIAS
                ddlSubcategoria.Items.Clear();
                ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
                using (var cmd = new SqlCommand(
                    "SELECT idsubcategoria,nombre FROM dbo.SUBCATEGORIAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlSubcategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idsubcategoria"].ToString()));

                // 2) TIENDAS
                ddlTienda.Items.Clear();
                ddlTienda.Items.Add(new ListItem("-- Tienda --", ""));
                using (var cmd2 = new SqlCommand(
                    "SELECT idtienda,nombre FROM dbo.TIENDAS WHERE activo=1", cn))
                using (var dr2 = cmd2.ExecuteReader())
                    while (dr2.Read())
                        ddlTienda.Items.Add(new ListItem(
                            dr2["nombre"].ToString(),
                            dr2["idtienda"].ToString()));

                // 3) UNIDADES
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

                // 4) MARCAS
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

                // 5) COLORES IMAGEN
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

        private void RegistrarOpcionesMedidas()
        {
            // Genera <option> para MEDIDAS
            var sb = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("SELECT idmedida,nombre FROM dbo.MEDIDAS ORDER BY nombre", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        sb.AppendFormat("<option value=\"{0}\">{1}</option>",
                                        dr["idmedida"], dr["nombre"]);
            }

            // Inyecta en JS
            var script = $@"
<script type=""text/javascript"">
  var medidasOptions = `{sb}`;
</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "medidasTpl", script);
        }

        protected void ddlUnidad_SelectedIndexChanged(object s, EventArgs e)
        {
            pnlAddUnidad.Visible = ddlUnidad.SelectedValue == "0";
        }

        protected void btnGuardarUnidad_Click(object s, EventArgs e)
        {
            var nueva = txtNewUnidad.Text.Trim();
            if (string.IsNullOrEmpty(nueva)) return;

            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.UNIDADDEMEDIDAS(nombre,activo)
 OUTPUT INSERTED.idunidaddemedida
 VALUES(@n,1)", cn))
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

        protected void ddlMarca_SelectedIndexChanged(object s, EventArgs e)
        {
            pnlAddMarca.Visible = ddlMarca.SelectedValue == "0";
        }

        protected void btnGuardarMarca_Click(object s, EventArgs e)
        {
            var nueva = txtNewMarca.Text.Trim();
            if (string.IsNullOrEmpty(nueva)) return;

            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.MARCAS(nombre,activo)
 OUTPUT INSERTED.idmarca
 VALUES(@n,1)", cn))
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

        protected void btnGuardarProducto_Click(object s, EventArgs e)
        {
            if (!Page.IsValid) return;
            lblError.Visible = false;

            // Validar dropdowns
            if (!int.TryParse(ddlUnidad.SelectedValue, out var unidad) || unidad == 0)
            {
                MostrarError("Seleccione o cree una unidad válida.");
                return;
            }
            if (!int.TryParse(ddlMarca.SelectedValue, out var marca) || marca == 0)
            {
                MostrarError("Seleccione o cree una marca válida.");
                return;
            }

            // Leer campos
            var sc = int.TryParse(ddlSubcategoria.SelectedValue, out var tmpSc) ? tmpSc : 0;
            var nombre = txtNombre.Text.Trim();
            var referencia = txtReferencia.Text.Trim();
            var cb = long.TryParse(txtCodigoBarras.Text.Trim(), out var tmpCb) ? tmpCb : 0L;
            var activo = chkActivo.Checked;
            var tipo = txtTipo.Text.Trim();
            var descuento = float.TryParse(txtDescuento.Text.Trim(), out var tmpD) ? tmpD : 0f;

            // Arrays dinámicos
            var tarifas = Request.Form.GetValues("tarifa") ?? new string[0];
            var aplicaEn = Request.Form.GetValues("aplicaEn") ?? new string[0];
            var cantMin = Request.Form.GetValues("cantMin") ?? new string[0];
            var precioVal = Request.Form.GetValues("precioVal") ?? new string[0];
            var exColor = Request.Form.GetValues("exColor") ?? new string[0];
            var exMed = Request.Form.GetValues("exMedida") ?? new string[0];
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
                    bool committed = false;
                    try
                    {
                        // 1) PRODUCTOS
                        int newId;
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.PRODUCTOS
 (idsubcategoria,idunidaddemedida,idmarca,nombre,
  referencia,codigodebarras,activo,tipodeproducto,descuento)
 OUTPUT INSERTED.idproducto
 VALUES
 (@sub,@uni,@mar,@nom,
  @ref,@cb,@act,@tip,@des)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@sub", sc > 0 ? (object)sc : DBNull.Value);
                            cmd.Parameters.AddWithValue("@uni", unidad);
                            cmd.Parameters.AddWithValue("@mar", marca);
                            cmd.Parameters.AddWithValue("@nom", nombre);
                            cmd.Parameters.AddWithValue("@ref", referencia);
                            cmd.Parameters.AddWithValue("@cb", cb);
                            cmd.Parameters.AddWithValue("@act", activo);
                            cmd.Parameters.AddWithValue("@tip", tipo);
                            cmd.Parameters.AddWithValue("@des", descuento);
                            newId = (int)cmd.ExecuteScalar();
                        }

                        // 2) PRECIOS (Evita IndexOutOfRange)
                        int precioCount = new[] { tarifas.Length, aplicaEn.Length, cantMin.Length, precioVal.Length }.Min();
                        using (var cmd2 = new SqlCommand(@"
INSERT INTO dbo.PRECIOS
 (idproducto,nombre,descripcion,precio,cantidad,activo)
 VALUES
 (@id,@nom,@desc,@pre,@cant,1)", cn, tx))
                        {
                            for (int i = 0; i < precioCount; i++)
                            {
                                cmd2.Parameters.Clear();
                                cmd2.Parameters.AddWithValue("@id", newId);
                                cmd2.Parameters.AddWithValue("@nom", tarifas[i]);
                                cmd2.Parameters.AddWithValue("@desc", aplicaEn[i]);
                                cmd2.Parameters.AddWithValue("@pre", decimal.Parse(precioVal[i]));
                                cmd2.Parameters.AddWithValue("@cant", int.Parse(cantMin[i]));
                                cmd2.ExecuteNonQuery();
                            }
                        }

                        // 3) EXISTENCIAS (Evita IndexOutOfRange)
                        int existCount = new[] { exColor.Length, exMed.Length, exCant.Length }.Min();
                        for (int i = 0; i < existCount; i++)
                        {
                            // colorId lookup
                            int colorId;
                            if (!int.TryParse(exColor[i], out colorId))
                            {
                                using (var lc = new SqlCommand(
                                    "SELECT idcolor FROM dbo.COLORES WHERE nombre=@n", cn, tx))
                                {
                                    lc.Parameters.AddWithValue("@n", exColor[i]);
                                    var o = lc.ExecuteScalar();
                                    colorId = o != null ? Convert.ToInt32(o) : 0;
                                }
                            }

                            // medId (dropdown)
                            int medId = int.Parse(exMed[i]);

                            using (var cmd3 = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIAS
 (idtienda,idproducto,idcolor,idmedida,cantidad)
 VALUES
 (@tid,@id,@col,@med,@can)", cn, tx))
                            {
                                cmd3.Parameters.AddWithValue("@tid", int.Parse(ddlTienda.SelectedValue));
                                cmd3.Parameters.AddWithValue("@id", newId);
                                cmd3.Parameters.AddWithValue("@col", colorId > 0 ? (object)colorId : DBNull.Value);
                                cmd3.Parameters.AddWithValue("@med", medId);
                                cmd3.Parameters.AddWithValue("@can", int.Parse(exCant[i]));
                                cmd3.ExecuteNonQuery();
                            }
                        }

                        
                        // 4) IMÁGENES
                        if (imgBytes != null)
                        {
                            using (var cmd4 = new SqlCommand(@"
INSERT INTO dbo.IMAGENES
 (idproducto,idcolor,foto,descripcion,activo)
 VALUES
 (@pid,      @col,    @bin,  @desc,      1)", cn, tx))
                            {
                                // 1) Vincular la imagen al producto recién creado
                                cmd4.Parameters.AddWithValue("@pid", newId);

                                // 2) Color de imagen
                                if (int.TryParse(ddlImgColor.SelectedValue, out var ic) && ic > 0)
                                    cmd4.Parameters.AddWithValue("@col", ic);
                                else
                                    cmd4.Parameters.AddWithValue("@col", DBNull.Value);

                                // 3) El contenido binario de la imagen
                                cmd4.Parameters.AddWithValue("@bin", imgBytes);

                                // 4) Descripción de la imagen
                                cmd4.Parameters.AddWithValue("@desc", txtImgDesc.Text.Trim());

                                cmd4.ExecuteNonQuery();
                            }
                        }


                        tx.Commit();
                        committed = true;
                        Response.Redirect("Productos.aspx");
                    }
                    catch (Exception ex)
                    {
                        if (!committed)
                            try { tx.Rollback(); } catch { }
                        MostrarError("Ocurrió un error: " + ex.Message);
                    }
                }
            }
        }

        private void MostrarError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
        }
    }
}

