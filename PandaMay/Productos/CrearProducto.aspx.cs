using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;           // para Min()
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
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
                CargarCategoriasListBox();   // llena lstCategorias
                CargarCatMaestra2();         // llena ddlCatMaestra2

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

        private void CargarCategoriasListBox(int? idMaestra = null)
        {
            lstCategorias.Items.Clear();
            lstCategorias.Items.Add(new ListItem("+ Agregar categoría", "new"));

            string sql = idMaestra.HasValue
                ? @"SELECT c.idcategoria, c.nombre
              FROM dbo.CATEGORIAS c
              JOIN dbo.CATEGORIASMAESTRASCATEGORIAS mc ON mc.idcategoria = c.idcategoria
             WHERE c.activo=1 AND mc.idcategoriamaestra=@idm
             ORDER BY c.nombre"
                : @"SELECT idcategoria, nombre
              FROM dbo.CATEGORIAS
             WHERE activo=1
             ORDER BY nombre";

            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                if (idMaestra.HasValue) cmd.Parameters.AddWithValue("@idm", idMaestra.Value);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        lstCategorias.Items.Add(new ListItem(
                            dr.GetString(1),
                            dr.GetInt32(0).ToString()
                        ));
            }
        }



        private void CargarCatMaestra2()
        {
            ddlCatMaestra2.Items.Clear();
            ddlCatMaestra2.Items.Add(new ListItem("-- Seleccione maestra --", ""));
            ddlCatMaestra2.Items.Add(new ListItem("+ Agregar maestra", "new"));
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idcategoriamaestra,nombre FROM dbo.CATEGORIASMAESTRAS WHERE activo=1 ORDER BY nombre", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlCatMaestra2.Items.Add(new ListItem(
                            dr.GetString(1),
                            dr.GetInt32(0).ToString()));
            }
        }

        /// <summary>
        /// Muestra en pantalla (solo lectura) las categorías y la maestra enlazadas
        /// a la subcategoría seleccionada.
        /// </summary>
        private void MostrarCategoriasYMaestra(int idSub)
        {
            // 1) Carga la o las categorías vinculadas a esta subcategoría
            var categorias = new List<string>();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT c.nombre 
          FROM dbo.CATEGORIAS c
          JOIN dbo.CATEGORIASSUBCATEGORIAS cs 
            ON c.idcategoria = cs.idcategoria
         WHERE cs.idsubcategoria = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", idSub);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        categorias.Add(dr.GetString(0));
            }
            // 2) Mostrar esas categorías en lstCategorias (solo lectura)
            lstCategorias.Items.Clear();
            foreach (var nom in categorias)
                lstCategorias.Items.Add(new ListItem(nom, ""));

            // 3) Carga la categoría maestra (asumiendo que cada categoría enlazada
            //    apunta a la misma maestra; si hay varias, podrías listar todas)
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT cm.nombre
          FROM dbo.CATEGORIASMAESTRASCATEGORIAS mc
          JOIN dbo.CATEGORIASMAESTRAS cm 
            ON mc.idcategoriamaestra = cm.idcategoriamaestra
         WHERE mc.idcategoria = (
            SELECT TOP 1 idcategoria
              FROM dbo.CATEGORIASSUBCATEGORIAS
             WHERE idsubcategoria = @id
          )", cn))
            {
                cmd.Parameters.AddWithValue("@id", idSub);
                cn.Open();
                var nombreMaestra = cmd.ExecuteScalar() as string ?? "";
                // Mostrarla en ddlCatMaestra2 (pero deshabilitada)
                ddlCatMaestra2.Items.Clear();
                ddlCatMaestra2.Items.Add(new ListItem(nombreMaestra, ""));
                ddlCatMaestra2.Enabled = false;
            }

            // Finalmente, ocultamos cualquier panel de “nuevo”
            pnlNewSub.Visible = pnlNewCat.Visible = pnlNewCatM.Visible = false;
        }

        /// <summary>
        /// Carga en ddlCategoria y ddlCatMaestra (ambos readonly) los valores
        /// asociados a la subcategoría idSub.
        /// </summary>
        private void MostrarCategoriaYMaestraReadOnly(int idSub)
        {
            // 1) Cargar categorías ligadas a la subcategoría
            var categorias = new List<Tuple<int, string>>();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT c.idcategoria, c.nombre
          FROM dbo.CATEGORIAS c
          JOIN dbo.CATEGORIASSUBCATEGORIAS cs 
            ON c.idcategoria = cs.idcategoria
         WHERE cs.idsubcategoria = @idSub
         ORDER BY c.nombre;", cn))
            {
                cmd.Parameters.AddWithValue("@idSub", idSub);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        categorias.Add(Tuple.Create(dr.GetInt32(0), dr.GetString(1)));
            }

            // Bind a la lista de categorías (solo lectura, multiselección)
            lstCategoriasRO.Items.Clear();
            foreach (var cat in categorias)
                lstCategoriasRO.Items.Add(new ListItem(cat.Item2, cat.Item1.ToString()) { Selected = true });
            lstCategoriasRO.Enabled = false;

            // 2) Cargar maestras (distintas) de esas categorías
            var maestras = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT cm.nombre
          FROM dbo.CATEGORIASMAESTRASCATEGORIAS mc
          JOIN dbo.CATEGORIASMAESTRAS cm 
            ON mc.idcategoriamaestra = cm.idcategoriamaestra
         WHERE mc.idcategoria = @idCat;", cn))
            {
                cmd.Parameters.Add("@idCat", SqlDbType.Int);
                cn.Open();
                foreach (var cat in categorias)
                {
                    cmd.Parameters["@idCat"].Value = cat.Item1;
                    var nombre = cmd.ExecuteScalar() as string;
                    if (!string.IsNullOrWhiteSpace(nombre))
                        maestras.Add(nombre);
                }
            }

            // Bind a la lista de maestras
            lstMaestrasRO.Items.Clear();
            foreach (var m in maestras.OrderBy(x => x))
                lstMaestrasRO.Items.Add(new ListItem(m, m) { Selected = true });
            lstMaestrasRO.Enabled = false;

            // Ocultar paneles de “nuevo”
            pnlNewSub.Visible = pnlNewCat.Visible = pnlNewCatM.Visible = false;
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
            ddlSubcategoria.Items.Add(new ListItem("+ Agregar subcategoría", "new"));


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
            bool nueva = ddlSubcategoria.SelectedValue == "new";
            pnlNewSub.Visible = nueva;

            if (nueva)
            {
                // Creación de subcategoría: recargo listados
                CargarCategoriasListBox();
                CargarCatMaestra2();
            }
            else
            {
                pnlNewSub.Visible = false;
                MostrarCategoriaYMaestraReadOnly(int.Parse(ddlSubcategoria.SelectedValue));
            }

        }


        protected void ddlCatMaestra2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Mantener visible el panel de "Nueva Categoría" tras el postback
            pnlNewCat.Visible = true;

            // Si el usuario eligió "+ Agregar maestra", mostrar ese panel
            if (ddlCatMaestra2.SelectedValue == "new")
            {
                txtNewCatM.Text = "";
                pnlNewCatM.Visible = true;   // abrir panel de nueva maestra
                                             // NO ocultes pnlNewCat: el usuario ya escribió la categoría
                                             // Opcional: foco en la nueva maestra
                txtNewCatM.Focus();
                return;
            }

            // Cualquier otra maestra seleccionada: ocultar el panel de nueva maestra
            pnlNewCatM.Visible = false;

            // (Opcional) si quieres filtrar el ListBox de categorías por la maestra elegida en el panel de subcategoría
            if (int.TryParse(ddlCatMaestra2.SelectedValue, out var idM) && idM > 0)
            {
                CargarCategoriasListBox(idM);  // usa la sobrecarga que filtra por maestra
                                               // NO toques pnlNewSub aquí: no queremos cerrar el flujo actual
            }
        }




        protected void btnAddCatM_Click(object sender, EventArgs e)
        {
            var nom = txtNewCatM.Text.Trim();
            if (nom == "") return;
            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        INSERT INTO dbo.CATEGORIASMAESTRAS (nombre,activo,fecha)
        OUTPUT INSERTED.idcategoriamaestra
        VALUES(@n,1,GETDATE())", cn))
            {
                cmd.Parameters.AddWithValue("@n", nom);
                cn.Open();
                idNew = (int)cmd.ExecuteScalar();
            }
            CargarCatMaestra2();
            ddlCatMaestra2.SelectedValue = idNew.ToString();
            pnlNewCatM.Visible = false;
            txtNewCatM.Text = "";
            CargarCategoriasListBox(idNew);   // refresca lista (vacía por ahora) asociada a la nueva maestra

        }

        protected void btnAddCat_Click(object sender, EventArgs e)
        {
            var nom = txtNewCat.Text.Trim();
            if (string.IsNullOrWhiteSpace(nom))
            {
                MostrarError("Escribe el nombre de la categoría.");
                return;
            }

            if (!int.TryParse(ddlCatMaestra2.SelectedValue, out var idMaestra) || idMaestra <= 0)
            {
                MostrarError("Selecciona una categoría maestra válida.");
                return;
            }

            int idCat;

            // Insert categoría + relación con la maestra en una sola transacción
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.CATEGORIAS (nombre, activo, fecha)
OUTPUT INSERTED.idcategoria
VALUES (@n, 1, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", nom);
                            idCat = (int)cmd.ExecuteScalar();
                        }

                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.CATEGORIASMAESTRASCATEGORIAS
  (idcategoriamaestra, idcategoria, activo)
VALUES
  (@idm, @ic, 1);", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@idm", idMaestra);
                            cmd.Parameters.AddWithValue("@ic", idCat);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MostrarError("No se pudo crear la categoría: " + ex.Message);
                        return;
                    }
                }
            }

            // Recarga el ListBox de categorías filtrado por la maestra seleccionada
            CargarCategoriasListBox(idMaestra);   

            // Preselecciona la categoría recién creada
            var nuevo = lstCategorias.Items.FindByValue(idCat.ToString());
            if (nuevo != null) nuevo.Selected = true;

            
            pnlNewCat.Visible = false;
            pnlNewSub.Visible = true;
            txtNewCat.Text = "";

            // (opcional) foco en la lista
            lstCategorias.Focus();
        }


        protected void btnAddSub_Click(object sender, EventArgs e)
        {
            

            var nom = txtNewSub.Text.Trim();
            if (nom == "") return;
            int idSub;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        INSERT INTO dbo.SUBCATEGORIAS (nombre,activo,fecha)
        OUTPUT INSERTED.idsubcategoria
        VALUES(@n,1,GETDATE())", cn))
            {
                cmd.Parameters.AddWithValue("@n", nom);
                cn.Open();
                idSub = (int)cmd.ExecuteScalar();
            }
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        INSERT INTO dbo.CATEGORIASSUBCATEGORIAS
          (idcategoria,idsubcategoria,activo)
        VALUES(@ic,@is,1)", cn))
            {
                cmd.Parameters.Add("@ic", SqlDbType.Int);
                cmd.Parameters.Add("@is", SqlDbType.Int).Value = idSub;
                cn.Open();
                foreach (ListItem itm in lstCategorias.Items)
                    if (itm.Selected)
                    {
                        cmd.Parameters["@ic"].Value = int.Parse(itm.Value);
                        cmd.ExecuteNonQuery();
                    }
            }
            CargarSubcategorias();
            pnlNewSub.Visible = false;
            txtNewSub.Text = "";
        }

        /// <summary>
        /// Al seleccionar “+ Agregar categoría” en lstCategorias,
        /// abrimos el panel de nueva categoría y ocultamos el de subcategoría.
        /// </summary>
        protected void lstCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si escogió “new”, muestro panel para crear categoría
            if (lstCategorias.SelectedValue == "new")
            {
                // Recarga dropdown de maestras y limpia el textbox
                CargarCatMaestra2();
                txtNewCat.Text = "";

                // Muestro/oculto paneles
                pnlNewCat.Visible = true;
                pnlNewSub.Visible = false;
            }
        }





        private void RegistrarOpcionesMedidas()
        {
            // --- Medidas ---
            var sbMedidas = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idmedida, tipomedida FROM dbo.MEDIDAS WHERE activo = 1 ORDER BY tipomedida", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        sbMedidas.AppendFormat("<option value=\"{0}\">{1}</option>",
                            dr.GetInt32(0), dr.GetString(1));
            }

            // --- Proveedores (para el combo en precios de compra) ---
            var sbProv = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT p.idproveedor, u.nombrenegocio
        FROM dbo.Proveedores p
        JOIN dbo.Usuarios u ON p.idusuario = u.idusuario
        WHERE u.activo = 1
        ORDER BY u.nombrenegocio;", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        sbProv.AppendFormat("<option value=\"{0}\">{1}</option>",
                            dr.GetInt32(0), dr.GetString(1));
            }

            // --- Inyecta AMBAS variables JS ---
            var script = $@"
<script type=""text/javascript"">
  var medidasOptions = `{sbMedidas}`;
  var supplierOptions = `{sbProv}`;
</script>";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "medidasYProveedores", script, false);
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
            var tipoProd = txtTipo.Text.Trim();
            var descuento = float.TryParse(txtDescuento.Text.Trim(), out var tmpD) ? tmpD : 0f;

            // —– Tarifas —–
            var tarifas = Request.Form.GetValues("tarifa") ?? Array.Empty<string>();
            var precioVal = Request.Form.GetValues("precioVal") ?? Array.Empty<string>();

            // —– Existencias (Medida + Cantidad) —–
            var medidas = Request.Form.GetValues("histMedida") ?? Array.Empty<string>();
            var cantidades = Request.Form.GetValues("histCantidad") ?? Array.Empty<string>();
            int filas = Math.Min(medidas.Length, cantidades.Length);

            // —– Precios de Compra —–
            var compProvs = Request.Form.GetValues("compProveedor") ?? Array.Empty<string>();
            var compPrecios = Request.Form.GetValues("compPrecio") ?? Array.Empty<string>();
            var compFechas = Request.Form.GetValues("compFecha") ?? Array.Empty<string>();
            int compCount = new[] { compProvs.Length, compPrecios.Length, compFechas.Length }.Min();

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
                        // 1) PRODUCTOS → newId
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
                            cmd.Parameters.AddWithValue("@tip", tipoProd);
                            cmd.Parameters.AddWithValue("@des", descuento);
                            newId = (int)cmd.ExecuteScalar();
                        }

                        // 2) EXISTENCIAS (por cada Medida+Cantidad)
                        var existenciasIds = new List<int>();
                        for (int i = 0; i < filas; i++)
                        {
                            if (!int.TryParse(medidas[i], out var idMedida) || idMedida <= 0) continue;
                            if (!int.TryParse(cantidades[i], out var cant) || cant <= 0) continue;

                            int idExist;
                            using (var cmdE = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIAS
  (idtienda, idproducto, idimagen, idmedida, idmarca, cantidad, fechaingreso)
OUTPUT INSERTED.idexistencia
VALUES (@tid, @pid, NULL, @med, @mar, @can, GETDATE());", cn, tx))
                            {
                                cmdE.Parameters.AddWithValue("@tid", tienda);
                                cmdE.Parameters.AddWithValue("@pid", newId);
                                cmdE.Parameters.AddWithValue("@med", idMedida);
                                cmdE.Parameters.AddWithValue("@mar", marca);
                                cmdE.Parameters.AddWithValue("@can", cant);
                                idExist = (int)cmdE.ExecuteScalar();
                            }
                            existenciasIds.Add(idExist);

                            // 2.1) Enlazar PÚBLICO a cada existencia (si aplica el modelo)
                            using (var cmdEP = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIASPUBLICOS (idexistencia, idpublico, activo)
VALUES (@eid, @pid, 1);", cn, tx))
                            {
                                cmdEP.Parameters.AddWithValue("@eid", idExist);
                                cmdEP.Parameters.AddWithValue("@pid", idPublico);
                                cmdEP.ExecuteNonQuery();
                            }
                        }

                        if (existenciasIds.Count == 0)
                            throw new Exception("Debe registrar al menos una existencia (medida y cantidad).");

                        // 3) PRECIOS DE VENTA (sobre la primera existencia)
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
                                if (!int.TryParse(tarifas[i], out var idNomPre) || idNomPre <= 0) continue;
                                if (i >= precioVal.Length) continue;
                                if (!decimal.TryParse(precioVal[i], out var p)) continue;

                                cmdPrecio.Parameters.Clear();
                                cmdPrecio.Parameters.AddWithValue("@idexistencia", existenciaPrincipal);
                                cmdPrecio.Parameters.AddWithValue("@idnombreprecio", idNomPre);
                                cmdPrecio.Parameters.AddWithValue("@precio", p);
                                cmdPrecio.ExecuteNonQuery();
                            }
                        }

                        // 4) IMAGEN (guarda y vincula a la última existencia)
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
                                cmdImg.Parameters.AddWithValue("@desc",
                                    string.IsNullOrWhiteSpace(txtImgDesc.Text) ? (object)DBNull.Value : txtImgDesc.Text.Trim());
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

                        // 5) PRECIOS DE COMPRA (tabla real: dbo.PRECIOSCOMPRAS, usa idexistencia)
                        const string sqlCompra = @"
INSERT INTO dbo.PRECIOSCOMPRAS
  (idexistencia, descripcion, precio, fecha)
VALUES
  (@eid, @desc, @precio, @fecha);";
                        using (var cmdCompra = new SqlCommand(sqlCompra, cn, tx))
                        {
                            for (int i = 0; i < compCount; i++)
                            {
                                if (!decimal.TryParse(compPrecios[i], out var precioCompra)) continue;

                                DateTime fechaCompra;
                                if (!DateTime.TryParse(compFechas[i], out fechaCompra))
                                    fechaCompra = DateTime.Today;

                                // Como tu tabla no tiene idproveedor, lo dejamos (opcional) en descripción
                                var provRaw = (compProvs[i] ?? "").Trim();
                                string descProv = (!string.IsNullOrEmpty(provRaw) && provRaw != "new")
                                                  ? "ProveedorId:" + provRaw
                                                  : null;

                                cmdCompra.Parameters.Clear();
                                cmdCompra.Parameters.AddWithValue("@eid", existenciaPrincipal);
                                cmdCompra.Parameters.AddWithValue("@desc", (object)descProv ?? DBNull.Value);
                                cmdCompra.Parameters.AddWithValue("@precio", precioCompra);
                                cmdCompra.Parameters.AddWithValue("@fecha", fechaCompra);
                                cmdCompra.ExecuteNonQuery();
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object AddProveedor(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del proveedor es requerido.");

            var conn = ConfigurationManager.ConnectionStrings["ConnectionString4"].ConnectionString;
            int idUsuario;
            int idProveedor;

            using (var cn = new SqlConnection(conn))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // (Opcional) evitar duplicados exactos
                        using (var chk = new SqlCommand(@"
SELECT TOP 1 p.idproveedor
FROM dbo.Proveedores p
JOIN dbo.Usuarios u ON p.idusuario = u.idusuario
WHERE u.nombrenegocio = @n AND u.activo = 1;", cn, tx))
                        {
                            chk.Parameters.AddWithValue("@n", nombre.Trim());
                            var exist = chk.ExecuteScalar();
                            if (exist != null)
                                return new { id = (int)exist, nombre = nombre.Trim() };
                        }

                        /// 1) Usuario
                        using (var cmdU = new SqlCommand(@"
INSERT INTO dbo.Usuarios (nombrenegocio)
OUTPUT INSERTED.idusuario
VALUES (@n);", cn, tx))
                        {
                            cmdU.Parameters.AddWithValue("@n", nombre.Trim());
                            idUsuario = (int)cmdU.ExecuteScalar();
                        }

                        // 2) Proveedor
                        using (var cmdP = new SqlCommand(@"
INSERT INTO dbo.Proveedores (idusuario)
OUTPUT INSERTED.idproveedor
VALUES (@u);", cn, tx))
                        {
                            cmdP.Parameters.AddWithValue("@u", idUsuario);
                            idProveedor = (int)cmdP.ExecuteScalar();
                        }



                        tx.Commit();
                    }
                    catch
                    {
                        try { tx.Rollback(); } catch { }
                        throw;
                    }
                }
            }

            return new { id = idProveedor, nombre = nombre.Trim() };
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

