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
                CargarCategoriasMaestras();
                LimpiarCombosCategorias();
                RegistrarOpcionesMedidas();
            }
        }

        private void CargarListas()
        {
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

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

        // Recarga SOLO la lista de unidades y deja seleccionada la recién creada
        private void RecargarUnidadesSeleccionando(int idNew)
        {
            ddlUnidad.Items.Clear();
            ddlUnidad.Items.Add(new ListItem("-- Unidad --", ""));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idunidaddemedida,nombre FROM dbo.UNIDADDEMEDIDAS WHERE activo=1", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlUnidad.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idunidaddemedida"].ToString()));
            }
            ddlUnidad.Items.Add(new ListItem("-- Agregar nueva unidad --", "0"));
            ddlUnidad.SelectedValue = idNew.ToString();
        }

        // Recarga SOLO la lista de marcas y deja seleccionada la recién creada
        private void RecargarMarcasSeleccionando(int idNew)
        {
            ddlMarca.Items.Clear();
            ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idmarca,nombre FROM dbo.MARCAS WHERE activo=1", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlMarca.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idmarca"].ToString()));
            }
            ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));
            ddlMarca.SelectedValue = idNew.ToString();
        }



        // >>> PASO 3: Métodos para categorías/subcategorías

        private void CargarCategoriasMaestras()
        {
            ddlCatMaestra.Items.Clear();
            ddlCatMaestra.Items.Add(new ListItem("-- Cat. maestra --", ""));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idcategoriamaestra, nombre FROM CATEGORIASMAESTRAS WHERE activo = 1 ORDER BY nombre", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlCatMaestra.Items.Add(new ListItem(dr["nombre"].ToString(),
                                                             dr["idcategoriamaestra"].ToString()));
            }
            ddlCatMaestra.Items.Add(new ListItem("-- Agregar nueva cat. maestra --", "0")); // <-- NUEVO
        }


        private void CargarCategoriasPorMaestra(int idMaestra)
        {
            ddlCategoria.Items.Clear();
            ddlCategoria.Items.Add(new ListItem("-- Categoría --", ""));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT c.idcategoria, c.nombre
          FROM CATEGORIAS c
          JOIN CATEGORIASMAESTRASCATEGORIAS mc ON mc.idcategoria = c.idcategoria
         WHERE mc.idcategoriamaestra = @idm AND c.activo = 1
         ORDER BY c.nombre;", cn))
            {
                cmd.Parameters.AddWithValue("@idm", idMaestra);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlCategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idcategoria"].ToString()));
            }
            ddlCategoria.Items.Add(new ListItem("-- Agregar nueva categoría --", "0"));
        }

        private void CargarSubcategoriasPorCategoria(int idCat)
        {
            ddlSubcategoria.Items.Clear();
            ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT s.idsubcategoria, s.nombre
          FROM SUBCATEGORIAS s
          JOIN CATEGORIASSUBCATEGORIAS cs ON cs.idsubcategoria = s.idsubcategoria
         WHERE cs.idcategoria = @idc AND s.activo = 1
         ORDER BY s.nombre;", cn))
            {
                cmd.Parameters.AddWithValue("@idc", idCat);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlSubcategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idsubcategoria"].ToString()));
            }
            ddlSubcategoria.Items.Add(new ListItem("-- Agregar nueva subcategoría --", "0"));
        }

        private void LimpiarCombosCategorias()
        {
            ddlCategoria.Items.Clear();
            ddlCategoria.Items.Add(new ListItem("-- Categoría --", ""));
            ddlSubcategoria.Items.Clear();
            ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
        }


        // >>> PASO 4: Eventos de cascada

        protected void ddlCatMaestra_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddSubcat.Visible = false;
            pnlAddCat.Visible = false;

            if (ddlCatMaestra.SelectedValue == "0")
            {
                // Mostrar panel para nueva Cat. Maestra
                pnlAddCatM.Visible = true;

                // Limpiar combos de categoría y subcategoría
                LimpiarCombosCategorias();
                return;
            }

            pnlAddCatM.Visible = false;

            if (int.TryParse(ddlCatMaestra.SelectedValue, out int idM) && idM > 0)
            {
                CargarCategoriasPorMaestra(idM);
                ddlSubcategoria.Items.Clear();
                ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
            }
            else
            {
                LimpiarCombosCategorias();
            }
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddSubcat.Visible = false;

            if (ddlCategoria.SelectedValue == "0")
            {
                // Mostrar panel para nueva Categoría
                pnlAddCat.Visible = true;
                ddlSubcategoria.Items.Clear();
                ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
                return;
            }

            pnlAddCat.Visible = false;

            if (int.TryParse(ddlCategoria.SelectedValue, out int idC) && idC > 0)
            {
                CargarSubcategoriasPorCategoria(idC);
            }
        }



        protected void ddlSubcategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddSubcat.Visible = ddlSubcategoria.SelectedValue == "0";
        }


        // >>> PASO 5: Guardar nueva subcategoría
        protected void btnGuardarSubcat_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (!int.TryParse(ddlCategoria.SelectedValue, out int idCategoria) || idCategoria <= 0)
            {
                MostrarError("Seleccione una categoría antes de crear la subcategoría.");
                return;
            }

            var nombre = txtNewSubcat.Text.Trim();
            var desc = txtNewSubcatDesc.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarError("Ingrese el nombre de la subcategoría.");
                return;
            }

            int newSubId;
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) SUBCATEGORIAS
                        using (var cmd = new SqlCommand(@"
INSERT INTO SUBCATEGORIAS (nombre, descripcion, descuento, activo, fecha)
OUTPUT INSERTED.idsubcategoria
VALUES (@n, @d, 0, 1, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@d", string.IsNullOrEmpty(desc) ? (object)DBNull.Value : desc);
                            newSubId = (int)cmd.ExecuteScalar();
                        }

                        // 2) Relación CATEGORIASSUBCATEGORIAS
                        using (var cmd2 = new SqlCommand(@"
INSERT INTO CATEGORIASSUBCATEGORIAS (idcategoria, idsubcategoria, descripcion, activo)
VALUES (@c, @s, NULL, 1);", cn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@c", idCategoria);
                            cmd2.Parameters.AddWithValue("@s", newSubId);
                            cmd2.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MostrarError("No se pudo guardar la subcategoría: " + ex.Message);
                        return;
                    }
                }
            }

            // recargar y seleccionar
            CargarSubcategoriasPorCategoria(idCategoria);
            ddlSubcategoria.SelectedValue = newSubId.ToString();

            pnlAddSubcat.Visible = false;
            txtNewSubcat.Text = txtNewSubcatDesc.Text = "";
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
        VALUES(@n,1);", cn))
            {
                cmd.Parameters.AddWithValue("@n", nueva);
                cn.Open();
                idNew = (int)cmd.ExecuteScalar();
            }

            // Recargar listas y seleccionar la nueva
            RecargarUnidadesSeleccionando(idNew);


            // Ocultar panel y limpiar textbox
            pnlAddUnidad.Visible = false;
            txtNewUnidad.Text = "";
        }

        // ====== NUEVO: Guardar CATEGORÍA MAESTRA ======
        protected void btnGuardarCatM_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            var nombre = txtNewCatM.Text.Trim();
            var desc = txtNewCatMDesc.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarError("Ingrese el nombre de la categoría maestra.");
                return;
            }

            int newId;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        INSERT INTO CATEGORIASMAESTRAS (nombre, descripcion, descuento, activo, fecha)
        OUTPUT INSERTED.idcategoriamaestra
        VALUES (@n, @d, 0, 1, GETDATE());", cn))
            {
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@d", string.IsNullOrEmpty(desc) ? (object)DBNull.Value : desc);
                cn.Open();
                newId = (int)cmd.ExecuteScalar();
            }

            CargarCategoriasMaestras();
            ddlCatMaestra.SelectedValue = newId.ToString();

            pnlAddCatM.Visible = false;
            txtNewCatM.Text = txtNewCatMDesc.Text = "";

            LimpiarCombosCategorias();
        }

        protected void btnGuardarCat_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (!int.TryParse(ddlCatMaestra.SelectedValue, out int idM) || idM <= 0)
            {
                MostrarError("Seleccione una categoría maestra antes de crear la categoría.");
                return;
            }

            var nombre = txtNewCat.Text.Trim();
            var desc = txtNewCatDesc.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarError("Ingrese el nombre de la categoría.");
                return;
            }

            int newCatId;
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(@"
INSERT INTO CATEGORIAS (nombre, descripcion, descuento, activo, fecha)
OUTPUT INSERTED.idcategoria
VALUES (@n, @d, 0, 1, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@d", string.IsNullOrEmpty(desc) ? (object)DBNull.Value : desc);
                            newCatId = (int)cmd.ExecuteScalar();
                        }

                        using (var cmd2 = new SqlCommand(@"
INSERT INTO CATEGORIASMAESTRASCATEGORIAS (idcategoriamaestra, idcategoria, descripcion, activo)
VALUES (@m, @c, NULL, 1);", cn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@m", idM);
                            cmd2.Parameters.AddWithValue("@c", newCatId);
                            cmd2.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MostrarError("No se pudo guardar la categoría: " + ex.Message);
                        return;
                    }
                }
            }

            CargarCategoriasPorMaestra(idM);
            ddlCategoria.SelectedValue = newCatId.ToString();

            pnlAddCat.Visible = false;
            txtNewCat.Text = txtNewCatDesc.Text = "";

            ddlSubcategoria.Items.Clear();
            ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
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
        VALUES(@n,1);", cn))
            {
                cmd.Parameters.AddWithValue("@n", nueva);
                cn.Open();
                idNew = (int)cmd.ExecuteScalar();
            }

            RecargarMarcasSeleccionando(idNew);


            pnlAddMarca.Visible = false;
            txtNewMarca.Text = "";
        }

        protected void btnGuardarProducto_Click(object s, EventArgs e)
        {
            Page.Validate("prod");
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
            if (!int.TryParse(ddlTienda.SelectedValue, out var tienda) || tienda == 0)
            {
                MostrarError("Seleccione tienda."); return;
            }
            if (!int.TryParse(ddlSubcategoria.SelectedValue, out var sc) || sc == 0) 
            {
                MostrarError("Seleccione subcategoría."); return; 
            }


            // Leer campos
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
                        if (imgBytes != null && imgBytes.Length > 0)
                        {
                            using (var cmd4 = new SqlCommand(@"
INSERT INTO dbo.IMAGENES
    (idproducto, idcolor, foto, descripcion, activo, fecha)
VALUES
    (@pid,       @col,    @bin, @desc,       1,     GETDATE());", cn, tx))
                            {
                                // 1) id del producto recién creado
                                cmd4.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = newId;

                                // 2) Color seleccionado (o NULL)
                                if (int.TryParse(ddlImgColor.SelectedValue, out var ic) && ic > 0)
                                    cmd4.Parameters.Add("@col", System.Data.SqlDbType.Int).Value = ic;
                                else
                                    cmd4.Parameters.Add("@col", System.Data.SqlDbType.Int).Value = DBNull.Value;

                                // 3) Imagen binaria
                                cmd4.Parameters.Add("@bin", System.Data.SqlDbType.VarBinary, imgBytes.Length).Value = imgBytes;

                                // 4) Descripción (puede ser NULL)
                                var desc = txtImgDesc.Text?.Trim();
                                cmd4.Parameters.Add("@desc", System.Data.SqlDbType.NVarChar, 200).Value =
                                    string.IsNullOrEmpty(desc) ? (object)DBNull.Value : desc;

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

