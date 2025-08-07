using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;           // para Min()
using System.Security.Cryptography;
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
            // ——— Traer tarifas para dropdown dinámico (se ejecuta SIEMPRE) ———
            var listaTarifas = new List<object>();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idnombreprecio, nombre, cantidad FROM dbo.nombresPrecios WHERE activo = 1", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        listaTarifas.Add(new
                        {
                            id = dr.GetInt32(0),
                            nombre = dr.GetString(1),
                            cantidad = dr.GetInt32(2)
                        });
            }
            var jsonTarifas = new System.Web.Script.Serialization.JavaScriptSerializer()
                                   .Serialize(listaTarifas);
            var script = $@"
<script type=""text/javascript"">
  var tarifaData = {jsonTarifas};
</script>";
            ClientScript.RegisterClientScriptBlock(
                this.GetType(),
                "tarifaData",
                script,
                false
            );

            RegistrarOpcionesMedidas();

            // ——— Inyectar tiendas y fecha de hoy en el cliente ———
            var sbT = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("SELECT idtienda,nombre FROM TIENDAS WHERE activo=1", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        sbT.AppendFormat("<option value=\"{0}\">{1}</option>",
                                         dr.GetInt32(0),
                                         dr.GetString(1));
            }
            string fechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
            var scriptTiendas = $@"
<script>
  var tiendaOptions = `{sbT}`;
  var hoy = '{fechaHoy}';
</script>";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "tiendaOpts", scriptTiendas, false);

            if (!IsPostBack)
            {
                CargarListas();
                CargarSubcategorias();
                CargarPublicos();

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
                // — Placeholder principal
                ddlUnidad.Items.Add(new ListItem("-- Unidad --", ""));
                // — Opción “Agregar nueva unidad” en segundo lugar
                ddlUnidad.Items.Add(new ListItem("-- Agregar nueva unidad --", "0"));

                // — Carga las unidades existentes —
                using (var cmd3 = new SqlCommand(
                    "SELECT idunidaddemedida,nombre FROM dbo.UNIDADDEMEDIDAS WHERE activo=1 ORDER BY nombre", cn))
                using (var dr3 = cmd3.ExecuteReader())
                    while (dr3.Read())
                        ddlUnidad.Items.Add(new ListItem(
                            dr3["nombre"].ToString(),
                            dr3["idunidaddemedida"].ToString()));


                // 4) MARCAS
                // — Limpiar y poner placeholder —
                ddlMarca.Items.Clear();
                ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
                // — Insertar “Agregar nueva” como segundo ítem —
                ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));

                // — Cargar todas las marcas de la BD —
                using (var cmd4 = new SqlCommand(
                    "SELECT idmarca,nombre FROM dbo.MARCAS WHERE activo=1 ORDER BY nombre", cn))
                using (var dr4 = cmd4.ExecuteReader())
                    while (dr4.Read())
                        ddlMarca.Items.Add(new ListItem(
                            dr4["nombre"].ToString(),
                            dr4["idmarca"].ToString()));



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


        /// <summary>
        /// Carga los públicos desde la tabla PUBLICOS en el dropdown ddlPublico.
        /// </summary>
        private void CargarPublicos()
        {
            ddlPublico.Items.Clear();
            ddlPublico.Items.Add(new ListItem("-- Seleccione público --", ""));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
    "SELECT idpublico, nombre FROM dbo.PUBLICOS ORDER BY nombre", cn))

            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlPublico.Items.Add(new ListItem(
                            dr.GetString(1),
                            dr.GetInt32(0).ToString()
                        ));
            }
        }


        // Recarga SOLO la lista de unidades y deja seleccionada la recién creada
        private void RecargarUnidadesSeleccionando(int idNew)
        {
            // 1) Limpiar y placeholder + nueva
            ddlUnidad.Items.Clear();
            ddlUnidad.Items.Add(new ListItem("-- Unidad --", ""));
            ddlUnidad.Items.Add(new ListItem("-- Agregar nueva unidad --", "0"));

            // 2) Abrir conexión y cargar unidades
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT idunidaddemedida,nombre FROM dbo.UNIDADDEMEDIDAS WHERE activo=1 ORDER BY nombre", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlUnidad.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idunidaddemedida"].ToString()));
            }

            // 3) Seleccionar la recién creada
            ddlUnidad.SelectedValue = idNew.ToString();
        }


        // Recarga SOLO la lista de marcas y deja seleccionada la recién creada
        private void RecargarMarcasSeleccionando(int idNew)
        {
            // 1) Limpiar y placeholder + nueva
            ddlMarca.Items.Clear();
            ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
            ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));

            // 2) Abrir conexión para recarga
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var cmd = new SqlCommand(
                    "SELECT idmarca,nombre FROM dbo.MARCAS WHERE activo=1 ORDER BY nombre", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlMarca.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idmarca"].ToString()));
            }

            // 3) Seleccionar la marca recién agregada
            ddlMarca.SelectedValue = idNew.ToString();
        }



        /// <summary>
        /// Carga todas las subcategorías activas en ddlSubcategoria.
        /// </summary>
        private void CargarSubcategorias()
        {
            ddlSubcategoria.Items.Clear();
            // 1) Placeholder al inicio
            ddlSubcategoria.Items.Add(new ListItem("-- Seleccione subcategoría --", ""));
            // 2) Opción para abrir el panel de agregar
            ddlSubcategoria.Items.Add(new ListItem("Agregar subcategoría", "new"));

            const string sql = @"
SELECT idsubcategoria, nombre
  FROM dbo.SUBCATEGORIAS
 WHERE activo = 1
 ORDER BY nombre;";

            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ddlSubcategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idsubcategoria"].ToString()));
                    }
                }
            }
        }

        // Detecta “new” y muestra el panel, o carga la jerarquía existente
        protected void ddlSubcategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSubcategoria.SelectedValue == "new")
            {
                // Mostrar formulario extendido
                pnlAddFullSubcat.Visible = true;
                return;
            }
            pnlAddFullSubcat.Visible = false;

            // Rellenar Categoría Maestra y Categoría según la subcategoría elegida
            if (int.TryParse(ddlSubcategoria.SelectedValue, out var idSub) && idSub > 0)
            {
                const string sql = @"
SELECT 
  cm.idcategoriamaestra, cm.nombre AS catMaestra,
  c.idcategoria,          c.nombre       AS categoria
FROM dbo.SUBCATEGORIAS s
JOIN dbo.CATEGORIASSUBCATEGORIAS cs ON cs.idsubcategoria = s.idsubcategoria
JOIN dbo.CATEGORIAS c                ON c.idcategoria   = cs.idcategoria
JOIN dbo.CATEGORIASMAESTRASCATEGORIAS mc ON mc.idcategoria = c.idcategoria
JOIN dbo.CATEGORIASMAESTRAS cm       ON cm.idcategoriamaestra = mc.idcategoriamaestra
WHERE s.idsubcategoria = @idSub;";

                using (var cn = new SqlConnection(_connString))
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@idSub", idSub);
                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                        if (dr.Read())
                        {
                            ddlCatMaestra.Items.Clear();
                            ddlCatMaestra.Items.Add(new ListItem(
                                dr["catMaestra"].ToString(),
                                dr["idcategoriamaestra"].ToString()));
                            ddlCatMaestra.SelectedIndex = 0;

                            ddlCategoria.Items.Clear();
                            ddlCategoria.Items.Add(new ListItem(
                                dr["categoria"].ToString(),
                                dr["idcategoria"].ToString()));
                            ddlCategoria.SelectedIndex = 0;
                        }
                }
            }
        }
        // >>> PASO 5: Guardar nueva subcategoría
        // Crea Cat. Maestra, Cat. y Subcat en una transacción y recarga el dropdown
        protected void btnAgregarSubFull_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            // 1) Sólo leemos los nombres:
            var nombreM = txtNewCatMFull.Text.Trim();
            var nombreC = txtNewCatFull.Text.Trim();
            var nombreS = txtNewSubFull.Text.Trim();

            if (string.IsNullOrEmpty(nombreM) ||
                string.IsNullOrEmpty(nombreC) ||
                string.IsNullOrEmpty(nombreS))
            {
                MostrarError("Complete los tres nombres antes de agregar.");
                return;
            }

            int idCatM, idCat, idSub;
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 2) Insertar Categoría Maestra (sin descripción)
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.CATEGORIASMAESTRAS (nombre, activo, fecha)
OUTPUT INSERTED.idcategoriamaestra
VALUES (@n, 1, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", nombreM);
                            idCatM = (int)cmd.ExecuteScalar();
                        }

                        // 3) Insertar Categoría (sin descripción)
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.CATEGORIAS (nombre, activo, fecha)
OUTPUT INSERTED.idcategoria
VALUES (@n, 1, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", nombreC);
                            idCat = (int)cmd.ExecuteScalar();
                        }

                        // 4) Enlazar Maestra→Categoría
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.CATEGORIASMAESTRASCATEGORIAS
  (idcategoriamaestra, idcategoria, activo)
VALUES (@idm, @ic, 1);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@idm", idCatM);
                            cmd.Parameters.AddWithValue("@ic", idCat);
                            cmd.ExecuteNonQuery();
                        }

                        // 5) Insertar Subcategoría (sin descripción)
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.SUBCATEGORIAS (nombre, activo, fecha)
OUTPUT INSERTED.idsubcategoria
VALUES (@n, 1, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", nombreS);
                            idSub = (int)cmd.ExecuteScalar();
                        }

                        // 6) Enlazar Categoría→Subcategoría
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.CATEGORIASSUBCATEGORIAS
  (idcategoria, idsubcategoria, activo)
VALUES (@ic, @is, 1);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ic", idCat);
                            cmd.Parameters.AddWithValue("@is", idSub);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MostrarError("No se pudo agregar: " + ex.Message);
                        return;
                    }
                }
            }

            // 7) Recarga el dropdown y selecciona la subcategoría recién creada
            CargarSubcategorias();
            ddlSubcategoria.SelectedValue = idSub.ToString();
            pnlAddFullSubcat.Visible = false;
        }


        private void RegistrarOpcionesMedidas()
        {
            // …generas tu sb con los <option>…
            var sb = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idmedida, tipomedida FROM dbo.MEDIDAS WHERE activo = 1 ORDER BY tipomedida",
                cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        sb.AppendFormat(
                          "<option value=\"{0}\">{1}</option>",
                          dr["idmedida"],
                          dr["tipomedida"]
                        );
            }

            // Inyecta en JS sin volver a generar <script> tags
            var script = $@"
<script type=""text/javascript"">
  var medidasOptions = `{sb}`;
</script>";

            ClientScript.RegisterStartupScript(
              this.GetType(),    // tipo de la página
              "medidasTpl",      // clave única
              script,            // tu bloque de script
              false              // <-- aquí
            );
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

            // —– Validaciones previas —–
            if (!int.TryParse(ddlUnidad.SelectedValue, out var unidad) || unidad == 0)
            { MostrarError("Seleccione o cree una unidad válida."); return; }
            if (!int.TryParse(ddlMarca.SelectedValue, out var marca) || marca == 0)
            { MostrarError("Seleccione o cree una marca válida."); return; }
            if (!int.TryParse(ddlTienda.SelectedValue, out var tienda) || tienda == 0)
            { MostrarError("Seleccione tienda."); return; }
            if (!int.TryParse(ddlSubcategoria.SelectedValue, out var sc) || sc == 0)
            { MostrarError("Seleccione subcategoría."); return; }
            if (!int.TryParse(ddlPublico.SelectedValue, out var idPublico) || idPublico == 0)
            { MostrarError("Seleccione un tipo de público válido."); return; }

            // —– Lectura de campos —–
            var nombre = txtNombre.Text.Trim();
            var referencia = txtReferencia.Text.Trim();
            var cb = long.TryParse(txtCodigoBarras.Text.Trim(), out var tmpCb) ? tmpCb : 0L;
            var activo = chkActivo.Checked;
            var tipo = txtTipo.Text.Trim();
            var descuento = float.TryParse(txtDescuento.Text.Trim(), out var tmpD) ? tmpD : 0f;

            // —– Tarifas —–
            var tarifas = Request.Form.GetValues("tarifa") ?? new string[0];
            var precioVal = Request.Form.GetValues("precioVal") ?? new string[0];

            // —– Historial de existencias —–
            var salidas = Request.Form.GetValues("histTiendaSalida") ?? new string[0];
            var recibos = Request.Form.GetValues("histTiendaRecibe") ?? new string[0];
            var fechas = Request.Form.GetValues("histFecha") ?? new string[0];
            var cantidades = Request.Form.GetValues("histCantidad") ?? new string[0];
            var referenciasHist = Request.Form.GetValues("histReferencia") ?? new string[0];
            var estados = Request.Form.GetValues("histEstado") ?? new string[0];

            int filas = new[]{
        salidas.Length,
        recibos.Length,
        fechas.Length,
        cantidades.Length,
        referenciasHist.Length,
        estados.Length
    }.Min();

            // —– Imagen —–
            byte[] imgBytes = null;
            if (fuImagen.HasFile)
            {
                using (var ms = new MemoryStream())
                {
                    fuImagen.PostedFile.InputStream.CopyTo(ms);
                    imgBytes = ms.ToArray();
                }
            }

            // —– Transacción única —–
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) INSERT en PRODUCTOS → newId
                        int newId;
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.PRODUCTOS
  (idsubcategoria, idunidaddemedida, nombre,
   referencia, codigodebarras, activo, tipodeproducto, descuento)
OUTPUT INSERTED.idproducto
VALUES
  (@sub, @uni, @nom, @ref, @cb, @act, @tip, @des);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@sub", sc > 0 ? (object)sc : DBNull.Value);
                            cmd.Parameters.AddWithValue("@uni", unidad);
                            cmd.Parameters.AddWithValue("@nom", nombre);
                            cmd.Parameters.AddWithValue("@ref", referencia);
                            cmd.Parameters.AddWithValue("@cb", cb);
                            cmd.Parameters.AddWithValue("@act", activo);
                            cmd.Parameters.AddWithValue("@tip", tipo);
                            cmd.Parameters.AddWithValue("@des", descuento);
                            newId = (int)cmd.ExecuteScalar();
                        }

                        // 2) INSERT en EXISTENCIAS + HISTORIAL
                        var existenciasIds = new List<int>();
                        for (int i = 0; i < filas; i++)
                        {
                            // 2.1) EXISTENCIAS
                            int idExist;
                            using (var cmdE = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIAS
  (idtienda, idproducto, idimagen, idmedida, idmarca, cantidad, fechaingreso)
OUTPUT INSERTED.idexistencia
VALUES (@tid, @pid, NULL, @med, @mar, @can, @f);", cn, tx))
                            {
                                cmdE.Parameters.AddWithValue("@tid", int.Parse(recibos[i]));
                                cmdE.Parameters.AddWithValue("@pid", newId);
                                cmdE.Parameters.AddWithValue("@med", unidad);
                                cmdE.Parameters.AddWithValue("@mar", marca);
                                cmdE.Parameters.AddWithValue("@can", int.Parse(cantidades[i]));
                                cmdE.Parameters.AddWithValue("@f", DateTime.Parse(fechas[i]));
                                idExist = (int)cmdE.ExecuteScalar();
                            }
                            existenciasIds.Add(idExist);

                            // 2.2) HISTORIALESEXISTENCIAS
                            using (var cmdH = new SqlCommand(@"
INSERT INTO dbo.HISTORIALESEXISTENCIAS
  (idexistencia, fecha, idtiendasalida, idtiendarecebe, cantidad, referencia, estado)
VALUES (@eid, @f, @sal, @rec, @can, @ref, @est);", cn, tx))
                            {
                                cmdH.Parameters.AddWithValue("@eid", idExist);
                                cmdH.Parameters.AddWithValue("@f", DateTime.Parse(fechas[i]));
                                cmdH.Parameters.AddWithValue("@sal", int.Parse(salidas[i]));
                                cmdH.Parameters.AddWithValue("@rec", int.Parse(recibos[i]));
                                cmdH.Parameters.AddWithValue("@can", int.Parse(cantidades[i]));
                                cmdH.Parameters.AddWithValue("@ref", referenciasHist[i]);
                                cmdH.Parameters.AddWithValue("@est", estados[i]);
                                cmdH.ExecuteNonQuery();
                            }
                        }

                        // 3) INSERT en PRECIOS (sobre la primera existencia)
                        if (existenciasIds.Count > 0)
                        {
                            int existenciaPrincipal = existenciasIds[0];
                            const string sqlPrecios = @"
INSERT INTO dbo.PRECIOS
  (idexistencia, idnombreprecio, precio, activo, fecha)
VALUES
  (@idexistencia, @idnombreprecio, @precio, 1, GETDATE());";
                            using (var cmdPrecio = new SqlCommand(sqlPrecios, cn, tx))
                            {
                                for (int i = 0; i < tarifas.Length; i++)
                                {
                                    if (!int.TryParse(tarifas[i], out var idNomPre) || idNomPre <= 0)
                                        continue;
                                    cmdPrecio.Parameters.Clear();
                                    cmdPrecio.Parameters.AddWithValue("@idexistencia", existenciaPrincipal);
                                    cmdPrecio.Parameters.AddWithValue("@idnombreprecio", idNomPre);
                                    cmdPrecio.Parameters.AddWithValue("@precio", decimal.Parse(precioVal[i]));
                                    cmdPrecio.ExecuteNonQuery();
                                }
                            }
                        }

                        // 4) INSERT en IMAGENES y UPDATE EXISTENCIAS (última)
                        if (imgBytes != null && imgBytes.Length > 0 && existenciasIds.Count > 0)
                        {
                            int newImageId;
                            using (var cmdImg = new SqlCommand(@"
INSERT INTO dbo.IMAGENES
  (idcolor, foto, descripcion, activo, fecha)
VALUES
  (@col, @bin, @desc, 1, GETDATE());
SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                            {
                                var col = int.TryParse(ddlImgColor.SelectedValue, out var tmpC) ? tmpC : 0;
                                cmdImg.Parameters.AddWithValue("@col", col > 0 ? (object)col : DBNull.Value);
                                cmdImg.Parameters.Add("@bin", SqlDbType.VarBinary, imgBytes.Length).Value = imgBytes;
                                cmdImg.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(txtImgDesc.Text)
                                    ? (object)DBNull.Value
                                    : txtImgDesc.Text.Trim());
                                newImageId = (int)cmdImg.ExecuteScalar();
                            }
                            using (var cmdUpd = new SqlCommand(@"
UPDATE dbo.EXISTENCIAS
  SET idimagen = @img
 WHERE idexistencia = @eid;", cn, tx))
                            {
                                cmdUpd.Parameters.AddWithValue("@img", newImageId);
                                cmdUpd.Parameters.AddWithValue("@eid", existenciasIds.Last());
                                cmdUpd.ExecuteNonQuery();
                            }
                        }

                        // Commit y salir
                        tx.Commit();
                        Response.Redirect("Productos.aspx");
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MostrarError("Ocurrió un error al guardar: " + ex.Message);
                    }
                }
            }
        }




        private void MostrarError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
        }


     

        // Guarda la nueva tarifa y recarga la página
        protected void btnGuardarTarifa_Click(object sender, EventArgs e)
        {
            var nombre = txtTarifaNombre.Text.Trim();
            if (!int.TryParse(txtTarifaCantidad.Text, out var cantidad))
                return;  // podrías validar y mostrar error aquí

            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        INSERT INTO dbo.nombresPrecios (nombre, cantidad, activo, fecha)
        VALUES (@n, @c, 1, GETDATE())", cn))
            {
                cmd.Parameters.AddWithValue("@n", nombre);
                cmd.Parameters.AddWithValue("@c", cantidad);
                cn.Open();
                cmd.ExecuteNonQuery();
            }

            // Refresca la página para recargar tarifaData
            Response.Redirect(Request.RawUrl);
        }

    }
}

