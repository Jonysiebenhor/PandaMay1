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
            // ——— Traer tarifas para dropdown dinámico (sin 'cantidad') ———
            var listaTarifas = new List<object>();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "SELECT idnombreprecio, nombre FROM dbo.NOMBRESPRECIOS WHERE activo = 1 ORDER BY nombre", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        listaTarifas.Add(new
                        {
                            id = dr.GetInt32(0),
                            nombre = dr.GetString(1)
                        });
                    }
                }
            }

            var jsonTarifas = new System.Web.Script.Serialization.JavaScriptSerializer()
                                   .Serialize(listaTarifas);
            var script = $@"
<script type=""text/javascript"">
  var tarifaData = {jsonTarifas};
</script>";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "tarifaData", script, false);

            // ——— Medidas y proveedores para plantillas dinámicas ———
            RegistrarOpcionesMedidas();

            // ——— Inyectar tiendas y fecha de hoy en el cliente ———
            var sbT = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand("SELECT idtienda,nombre FROM TIENDAS WHERE activo=1", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        sbT.AppendFormat("<option value=\"{0}\">{1}</option>",
                                         dr.GetInt32(0),
                                         dr.GetString(1));
                }
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
            try
            {
                using (var cn = new SqlConnection(_connString))
                {
                    cn.Open();

                    // TIENDAS
                    ddlTienda.Items.Clear();
                    ddlTienda.Items.Add(new ListItem("-- Tienda --", ""));
                    using (var cmd = new SqlCommand(
                        "SELECT idtienda, nombre FROM dbo.TIENDAS WHERE ISNULL(activo,1)=1 ORDER BY nombre", cn))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ddlTienda.Items.Add(new ListItem(
                                dr.IsDBNull(1) ? "(sin nombre)" : dr.GetString(1),
                                dr.GetInt32(0).ToString()));
                        }
                    }

                    // MARCAS
                    ddlMarca.Items.Clear();
                    ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
                    ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));
                    using (var cmd = new SqlCommand(
                        "SELECT idmarca, nombre FROM dbo.MARCAS WHERE ISNULL(activo,1)=1 ORDER BY nombre", cn))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ddlMarca.Items.Add(new ListItem(
                                dr.IsDBNull(1) ? "(sin nombre)" : dr.GetString(1),
                                dr.GetInt32(0).ToString()));
                        }
                    }

                    // COLORES (para imágenes)
                    ddlImgColor.Items.Clear();
                    ddlImgColor.Items.Add(new ListItem("-- Color imagen --", ""));
                    using (var cmd = new SqlCommand(
                        "SELECT idcolor, nombre FROM dbo.COLORES ORDER BY nombre", cn))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ddlImgColor.Items.Add(new ListItem(
                                dr.IsDBNull(1) ? "(sin nombre)" : dr.GetString(1),
                                dr.GetInt32(0).ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si algo falla, muéstralo y así sabemos por qué no se llenó.
                MostrarError("No se pudieron cargar las listas: " + ex.Message);
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

        // Recarga SOLO la lista de marcas y deja seleccionada la recién creada
        // Recarga SOLO la lista de marcas y deja seleccionada la recién creada
        private void RecargarMarcasSeleccionando(int idNew)
        {
            try
            {
                ddlMarca.Items.Clear();
                ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
                ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));

                using (var cn = new SqlConnection(_connString))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(
                        "SELECT idmarca, nombre FROM dbo.MARCAS WHERE ISNULL(activo,1)=1 ORDER BY nombre", cn))
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ddlMarca.Items.Add(new ListItem(
                                dr.IsDBNull(1) ? "(sin nombre)" : dr.GetString(1),
                                dr.GetInt32(0).ToString()));
                        }
                    }
                }

                // Seleccionar la nueva marca al final
                var item = ddlMarca.Items.FindByValue(idNew.ToString());
                if (item != null) ddlMarca.SelectedValue = idNew.ToString();
            }
            catch (Exception ex)
            {
                MostrarError("No se pudo recargar la lista de marcas: " + ex.Message);
            }
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
            // --- Medidas (mostrar medidaeu; si viene null, caer a tipomedida) ---
            var sbMedidas = new StringBuilder();
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(@"
        SELECT idmedida,
               COALESCE(medidaeu, tipomedida) AS nombre
        FROM dbo.MEDIDAS
        WHERE ISNULL(activo,1)=1
        ORDER BY nombre;", cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        sbMedidas.AppendFormat("<option value=\"{0}\">{1}</option>",
                            dr.GetInt32(0),
                            dr.IsDBNull(1) ? "(sin nombre)" : dr.GetString(1));
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

            // Inyecta ambas variables JS
            var script = $@"
<script type=""text/javascript"">
  var medidasOptions = `{sbMedidas}`;   // las opciones de MEDIDAS (sin la opción 'new')
  var supplierOptions = `{sbProv}`;
</script>";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "medidasYProveedores", script, false);
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

            // Marca opcional -> NULL si no se elige
            int? idMarca = null;
            if (int.TryParse(ddlMarca.SelectedValue, out var marcaTmp) && marcaTmp > 0)
                idMarca = marcaTmp;

            // Requeridos
            if (!int.TryParse(ddlTienda.SelectedValue, out var tienda) || tienda == 0)
            { MostrarError("Seleccione tienda."); return; }
            if (!int.TryParse(ddlSubcategoria.SelectedValue, out var sc) || sc == 0)
            { MostrarError("Seleccione subcategoría."); return; }
            if (!int.TryParse(ddlPublico.SelectedValue, out var idPublico) || idPublico == 0)
            { MostrarError("Seleccione un tipo de público válido."); return; }

            // Campos
            var nombre = txtNombre.Text.Trim();
            var referencia = txtReferencia.Text.Trim();
            var cb = long.TryParse(txtCodigoBarras.Text.Trim(), out var tmpCb) ? tmpCb : 0L;
            var activo = chkActivo.Checked;
            var tipoProd = txtTipo.Text.Trim();
            var descuento = float.TryParse(txtDescuento.Text.Trim(), out var tmpD) ? tmpD : 0f;

            // Precios de venta
            var tarifas = Request.Form.GetValues("tarifa") ?? Array.Empty<string>();
            var precioVal = Request.Form.GetValues("precioVal") ?? Array.Empty<string>();
            var cantMinVal = Request.Form.GetValues("cantMin") ?? Array.Empty<string>();

            // ===== EXISTENCIAS (lectura de campos del form) =====
            var medidas = Request.Form.GetValues("histMedida") ?? new string[0];
            var cantidades = Request.Form.GetValues("histCantidad") ?? new string[0];

            // Campos cuando el usuario elige “+ Nueva medida…”
            var medidaEUs = Request.Form.GetValues("histMedidaEu") ?? new string[0];
            var anchos = Request.Form.GetValues("histAncho") ?? new string[0];
            var largos = Request.Form.GetValues("histLargo") ?? new string[0];
            var altos = Request.Form.GetValues("histAlto") ?? new string[0];
            var profundidades = Request.Form.GetValues("histProfundidad") ?? new string[0];
            var circunferencias = Request.Form.GetValues("histCircunferencia") ?? new string[0];

            // Nº de filas a procesar (al menos Medida + Cantidad)
            int filas = Math.Min(medidas.Length, cantidades.Length);


            // Precios de compra
            var compProvs = Request.Form.GetValues("compProveedor") ?? Array.Empty<string>();
            var compPrecios = Request.Form.GetValues("compPrecio") ?? Array.Empty<string>();
            var compFechas = Request.Form.GetValues("compFecha") ?? Array.Empty<string>();
            int compCount = new[] { compProvs.Length, compPrecios.Length, compFechas.Length }.Min();

            // **** Imágenes múltiples (opcional) ****
            var archivos = new List<System.Web.HttpPostedFile>();
            if (fuImagenes.HasFiles)
            {
                foreach (System.Web.HttpPostedFile f in fuImagenes.PostedFiles)
                    if (f != null && f.ContentLength > 0) archivos.Add(f);
            }

            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) PRODUCTO
                        int idProducto;
                        using (var cmd = new SqlCommand(@"
INSERT INTO dbo.PRODUCTOS
    (idsubcategoria, nombre, referencia, codigodebarras, activo, tipodeproducto, descuento, fecha)
OUTPUT INSERTED.idproducto
VALUES
    (@sub, @nom, @ref, @cb, @act, @tip, @des, GETDATE());", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@sub", sc);
                            cmd.Parameters.AddWithValue("@nom", nombre);
                            cmd.Parameters.AddWithValue("@ref", string.IsNullOrWhiteSpace(referencia) ? (object)DBNull.Value : referencia);
                            cmd.Parameters.AddWithValue("@cb", cb);
                            cmd.Parameters.AddWithValue("@act", activo);
                            cmd.Parameters.AddWithValue("@tip", string.IsNullOrWhiteSpace(tipoProd) ? (object)DBNull.Value : tipoProd);
                            cmd.Parameters.AddWithValue("@des", descuento);
                            idProducto = (int)cmd.ExecuteScalar();
                        }

                        // 2) EXISTENCIAS (una por medida/cantidad). Marca puede ir NULL.
                        var existenciasIds = new List<int>();

                        // Helpers locales (compatibles con C# 7.3)
                        decimal? TryDecLocal(string[] arr, int idx)
                        {
                            if (arr == null || idx >= arr.Length) return null;

                            var strVal = (arr[idx] ?? "").Trim();   // <— antes se llamaba "s"
                            if (strVal == "") return null;

                            decimal d;
                            return decimal.TryParse(strVal, out d) ? (decimal?)d : null;
                        }

                        Action<SqlCommand, string, decimal?> AddDecParamLocal = (cmd, name, val) =>
                        {
                            var p = cmd.Parameters.Add(name, SqlDbType.Decimal);
                            p.Precision = 18; p.Scale = 2; p.Value = (object)val ?? DBNull.Value;
                        };


                        for (int i = 0; i < filas; i++)
                        {
                            // cantidad obligatoria (>0)
                            if (!int.TryParse(cantidades[i], out var cant) || cant <= 0) continue;

                            // Determinar idMedida
                            int idMedida = 0;
                            var medRaw = (i < medidas.Length ? (medidas[i] ?? "").Trim() : "");

                            if (string.Equals(medRaw, "new", StringComparison.OrdinalIgnoreCase))
                            {
                                // Crear una nueva medida con datos opcionales (se guardan en NULL si no se llenan)
                                string medEU = (i < medidaEUs.Length ? medidaEUs[i]?.Trim() : null);
                                decimal? ancho = TryDecLocal(anchos, i);
                                decimal? largo = TryDecLocal(largos, i);
                                decimal? alto = TryDecLocal(altos, i);
                                decimal? profundidad = TryDecLocal(profundidades, i);
                                decimal? circunf = TryDecLocal(circunferencias, i);

                                using (var cmdM = new SqlCommand(@"
INSERT INTO dbo.MEDIDAS
    (medidaeu, tipomedida, ancho, largo, alto, profundidad, circunferencia, descripcion, activo)
OUTPUT INSERTED.idmedida
VALUES
    (@meu, NULL, @an, @la, @al, @pr, @ci, NULL, 1);", cn, tx))
                                {
                                    cmdM.Parameters.AddWithValue("@meu", string.IsNullOrWhiteSpace(medEU) ? (object)DBNull.Value : medEU);
                                    AddDecParamLocal(cmdM, "@an", ancho);
                                    AddDecParamLocal(cmdM, "@la", largo);
                                    AddDecParamLocal(cmdM, "@al", alto);
                                    AddDecParamLocal(cmdM, "@pr", profundidad);
                                    AddDecParamLocal(cmdM, "@ci", circunf);

                                    idMedida = (int)cmdM.ExecuteScalar();
                                }
                            }
                            else
                            {
                                // Usar una medida existente seleccionada en el combo
                                if (!int.TryParse(medRaw, out idMedida) || idMedida <= 0) continue;
                            }

                            // Insertar la existencia con esa medida
                            using (var cmdE = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIAS
    (idtienda, idproducto, idimagen, idmedida, idmarca, cantidad, fechaingreso, activo)
OUTPUT INSERTED.idexistencia
VALUES
    (@tid, @pid, NULL, @med, @mar, @can, GETDATE(), 1);", cn, tx))
                            {
                                cmdE.Parameters.AddWithValue("@tid", tienda);
                                cmdE.Parameters.AddWithValue("@pid", idProducto);
                                cmdE.Parameters.AddWithValue("@med", idMedida);
                                cmdE.Parameters.AddWithValue("@mar", (object)idMarca ?? DBNull.Value);
                                cmdE.Parameters.AddWithValue("@can", cant);

                                var idExist = (int)cmdE.ExecuteScalar();
                                existenciasIds.Add(idExist);

                                // Público asociado
                                using (var cmdEP = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIASPUBLICOS (idexistencia, idpublico, activo)
VALUES (@eid, @pid, 1);", cn, tx))
                                {
                                    cmdEP.Parameters.AddWithValue("@eid", idExist);
                                    cmdEP.Parameters.AddWithValue("@pid", idPublico);
                                    cmdEP.ExecuteNonQuery();
                                }
                            }
                        }

                        if (existenciasIds.Count == 0)
                            throw new Exception("Debe registrar al menos una existencia (medida y cantidad).");

                        // Elegimos una existencia "principal" para precios e imágenes
                        int existenciaPrincipal = existenciasIds[0];


                        // 3) PRECIOS DE VENTA (con cantidad mínima)
                        using (var cmdPrecio = new SqlCommand(@"
INSERT INTO dbo.PRECIOS
    (idexistencia, idnombreprecio, precio, cantidad, activo, fecha)
VALUES
    (@idexistencia, @idnombreprecio, @precio, @cantidad, 1, GETDATE());", cn, tx))
                        {
                            for (int i = 0; i < tarifas.Length; i++)
                            {
                                if (!int.TryParse(tarifas[i], out var idNomPre) || idNomPre <= 0) continue;
                                if (i >= precioVal.Length) continue;
                                if (!decimal.TryParse(precioVal[i], out var p)) continue;

                                int cantMin = 0;
                                if (i < cantMinVal.Length) int.TryParse(cantMinVal[i], out cantMin);

                                cmdPrecio.Parameters.Clear();
                                cmdPrecio.Parameters.AddWithValue("@idexistencia", existenciaPrincipal);
                                cmdPrecio.Parameters.AddWithValue("@idnombreprecio", idNomPre);
                                var pParam = cmdPrecio.Parameters.Add("@precio", SqlDbType.Decimal);
                                pParam.Precision = 18; pParam.Scale = 2; pParam.Value = p;
                                cmdPrecio.Parameters.Add("@cantidad", SqlDbType.Int).Value =
                                    cantMin > 0 ? (object)cantMin : DBNull.Value;
                                cmdPrecio.ExecuteNonQuery();
                            }
                        }

                        // 4) IMÁGENES MÚLTIPLES -> IMAGENES + EXISTENCIASIMAGENES
                        if (archivos.Count > 0)
                        {
                            // Color opcional (NULL si no se elige)
                            int idColorSel = 0;
                            int.TryParse(ddlImgColor.SelectedValue, out idColorSel);
                            string descComun = string.IsNullOrWhiteSpace(txtImgDesc.Text)
                                ? null
                                : txtImgDesc.Text.Trim();

                            foreach (var file in archivos)
                            {
                                byte[] bytes;
                                using (var ms = new MemoryStream())
                                {
                                    file.InputStream.CopyTo(ms);
                                    bytes = ms.ToArray();
                                }

                                int idImagen;
                                using (var cmdImg = new SqlCommand(@"
INSERT INTO dbo.IMAGENES (idcolor, foto, descripcion, activo, fecha)
OUTPUT INSERTED.idimagen
VALUES (@col, @bin, @desc, 1, GETDATE());", cn, tx))
                                {
                                    cmdImg.Parameters.AddWithValue("@col", idColorSel > 0 ? (object)idColorSel : DBNull.Value);
                                    cmdImg.Parameters.Add("@bin", SqlDbType.VarBinary, bytes.Length).Value = bytes;
                                    // Si no hay descripción global, guardamos el nombre del archivo
                                    var desc = (object)(descComun ?? System.IO.Path.GetFileName(file.FileName)) ?? DBNull.Value;
                                    cmdImg.Parameters.AddWithValue("@desc", desc);
                                    idImagen = (int)cmdImg.ExecuteScalar();
                                }

                                // Relación muchos-a-muchos con la existencia
                                using (var cmdRel = new SqlCommand(@"
INSERT INTO dbo.EXISTENCIASIMAGENES (idexistencia, idimagen, activo)
VALUES (@eid, @iid, 1);", cn, tx))
                                {
                                    cmdRel.Parameters.AddWithValue("@eid", existenciaPrincipal);
                                    cmdRel.Parameters.AddWithValue("@iid", idImagen);
                                    cmdRel.ExecuteNonQuery();
                                }
                            }
                        }

                        // 5) PRECIOS DE COMPRA
                        using (var cmdCompra = new SqlCommand(@"
INSERT INTO dbo.PRECIOSCOMPRAS (idexistencia, descripcion, precio, activo, fecha)
VALUES (@eid, @desc, @precio, 1, @fecha);", cn, tx))
                        {
                            for (int i = 0; i < compCount; i++)
                            {
                                if (!decimal.TryParse(compPrecios[i], out var precioCompra)) continue;
                                DateTime fechaCompra;
                                if (!DateTime.TryParse(compFechas[i], out fechaCompra))
                                    fechaCompra = DateTime.Today;

                                var provRaw = (compProvs[i] ?? "").Trim();
                                string descProv = (!string.IsNullOrEmpty(provRaw) && provRaw != "new")
                                    ? "ProveedorId:" + provRaw
                                    : null;

                                cmdCompra.Parameters.Clear();
                                cmdCompra.Parameters.AddWithValue("@eid", existenciaPrincipal);
                                cmdCompra.Parameters.AddWithValue("@desc", (object)descProv ?? DBNull.Value);
                                var pc = cmdCompra.Parameters.Add("@precio", SqlDbType.Decimal);
                                pc.Precision = 18; pc.Scale = 2; pc.Value = precioCompra;
                                cmdCompra.Parameters.AddWithValue("@fecha", fechaCompra);
                                cmdCompra.ExecuteNonQuery();
                            }
                        }

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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object AddMedida(
    string medidaeu,
    string ancho,
    string largo,
    string alto,
    string profundidad,
    string circunferencia)
        {
            var conn = ConfigurationManager.ConnectionStrings["ConnectionString4"].ConnectionString;

            decimal? dAncho = TryDec(ancho);
            decimal? dLargo = TryDec(largo);
            decimal? dAlto = TryDec(alto);
            decimal? dProf = TryDec(profundidad);
            decimal? dCirc = TryDec(circunferencia);

            int idMedida;
            using (var cn = new SqlConnection(conn))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.MEDIDAS
  (medidaeu, tipomedida, ancho, largo, alto, profundidad, circunferencia, activo)
OUTPUT INSERTED.idmedida
VALUES
  (@meu, NULL, @ancho, @largo, @alto, @prof, @circ, 1);", cn))
            {
                cmd.Parameters.AddWithValue("@meu",
                    string.IsNullOrWhiteSpace(medidaeu) ? (object)DBNull.Value : medidaeu.Trim());

                AddDecParam(cmd, "@ancho", dAncho);
                AddDecParam(cmd, "@largo", dLargo);
                AddDecParam(cmd, "@alto", dAlto);
                AddDecParam(cmd, "@prof", dProf);
                AddDecParam(cmd, "@circ", dCirc);

                cn.Open();
                idMedida = (int)cmd.ExecuteScalar();
            }

            var nombreMostrar = string.IsNullOrWhiteSpace(medidaeu) ? "(sin nombre)" : medidaeu.Trim();
            return new { id = idMedida, nombre = nombreMostrar };
        }

        // helpers
        private static decimal? TryDec(string s)
        {
            if (decimal.TryParse((s ?? "").Trim(), out var d)) return d;
            return null;
        }
        private static void AddDecParam(SqlCommand cmd, string name, decimal? value)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.Decimal);
            p.Precision = 18;
            p.Scale = 2;
            p.Value = value.HasValue ? (object)value.Value : DBNull.Value;
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

