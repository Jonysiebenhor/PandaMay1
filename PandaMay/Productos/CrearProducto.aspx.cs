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
                    "SELECT idsubcategoria, nombre FROM dbo.SUBCATEGORIAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlSubcategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idsubcategoria"].ToString()));

                // Tiendas
                ddlTienda.Items.Clear();
                ddlTienda.Items.Add(new ListItem("-- Tienda --", ""));
                using (var cmd2 = new SqlCommand(
                    "SELECT idtienda, nombre FROM dbo.TIENDAS WHERE activo=1", cn))
                using (var dr2 = cmd2.ExecuteReader())
                    while (dr2.Read())
                        ddlTienda.Items.Add(new ListItem(
                            dr2["nombre"].ToString(),
                            dr2["idtienda"].ToString()));

                // Unidades
                ddlUnidad.Items.Clear();
                ddlUnidad.Items.Add(new ListItem("-- Unidad --", ""));
                using (var cmd3 = new SqlCommand(
                    "SELECT idunidaddemedida, nombre FROM dbo.UNIDADDEMEDIDAS WHERE activo=1", cn))
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
                    "SELECT idmarca, nombre FROM dbo.MARCAS WHERE activo=1", cn))
                using (var dr4 = cmd4.ExecuteReader())
                    while (dr4.Read())
                        ddlMarca.Items.Add(new ListItem(
                            dr4["nombre"].ToString(),
                            dr4["idmarca"].ToString()));
                ddlMarca.Items.Add(new ListItem("-- Agregar nueva marca --", "0"));
            }
        }

        protected void ddlUnidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlAddUnidad.Visible = ddlUnidad.SelectedValue == "0";
        }

        protected void btnGuardarUnidad_Click(object sender, EventArgs e)
        {
            string nueva = txtNewUnidad.Text.Trim();
            if (string.IsNullOrEmpty(nueva)) return;

            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.UNIDADDEMEDIDAS(nombre) OUTPUT INSERTED.idunidaddemedida VALUES(@n)", cn))
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
            string nueva = txtNewMarca.Text.Trim();
            if (string.IsNullOrEmpty(nueva)) return;

            int idNew;
            using (var cn = new SqlConnection(_connString))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.MARCAS(nombre) OUTPUT INSERTED.idmarca VALUES(@n)", cn))
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

            // Lectura de campos fijos
            int subcat = int.Parse(ddlSubcategoria.SelectedValue);
            int unidad = int.Parse(ddlUnidad.SelectedValue);
            int marca = int.Parse(ddlMarca.SelectedValue);
            string nombre = txtNombre.Text.Trim();
            string referencia = txtReferencia.Text.Trim();
            long codBarras = long.Parse(txtCodigoBarras.Text.Trim());
            bool activo = chkActivo.Checked;
            string tipoProd = txtTipo.Text.Trim();
            float descuento = float.Parse(txtDescuento.Text.Trim());

            // Filas dinámicas
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

            int newId;  // Declarado aquí

            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1) INSERT PRODUCTO
                        using (var cmd = new SqlCommand(@"
    INSERT INTO dbo.PRODUCTOS
      (idsubcategoria,idunidaddemedida,idmarca,nombre,fecha,
       referencia,codigodebarras,activo,tipodeproducto,descuento)
    OUTPUT INSERTED.idproducto
    VALUES
      (@sub,@uni,@mar,@nom,GETDATE(),@ref,@cb,@act,@tip,@des)", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@sub", subcat);
                            cmd.Parameters.AddWithValue("@uni", unidad);
                            cmd.Parameters.AddWithValue("@mar", marca);
                            cmd.Parameters.AddWithValue("@nom", nombre);
                            cmd.Parameters.AddWithValue("@ref", referencia);
                            cmd.Parameters.AddWithValue("@cb", codBarras);
                            cmd.Parameters.AddWithValue("@act", activo);
                            cmd.Parameters.AddWithValue("@tip", tipoProd);
                            cmd.Parameters.AddWithValue("@des", descuento);

                            newId = (int)cmd.ExecuteScalar();
                        }

                        // 2) INSERT precios dinámicos
                        for (int i = 0; i < tarifas.Length; i++)
                        {
                            using (var cmd2 = new SqlCommand(@"
    INSERT INTO dbo.PRECIOS
      (idproducto,nombre,precio,cantidadmin,aplica,fecha)
    VALUES
      (@id,@nom,@pre,@min,@app,GETDATE())", cn, tx))
                            {
                                cmd2.Parameters.AddWithValue("@id", newId);
                                cmd2.Parameters.AddWithValue("@nom", tarifas[i]);
                                cmd2.Parameters.AddWithValue("@pre", decimal.Parse(precioVal[i]));
                                cmd2.Parameters.AddWithValue("@min", int.Parse(cantMin[i]));
                                cmd2.Parameters.AddWithValue("@app", aplicaEn[i]);
                                cmd2.ExecuteNonQuery();
                            }
                        }

                        // 3) INSERT existencias dinámicas
                        for (int i = 0; i < exColor.Length; i++)
                        {
                            using (var cmd3 = new SqlCommand(@"
    INSERT INTO dbo.EXISTENCIAS
      (idtienda,idproducto,color,medida,cantidad,fechaingreso)
    VALUES
      (@tid,@id,@col,@med,@can,GETDATE())", cn, tx))
                            {
                                cmd3.Parameters.AddWithValue("@tid", int.Parse(ddlTienda.SelectedValue));
                                cmd3.Parameters.AddWithValue("@id", newId);
                                cmd3.Parameters.AddWithValue("@col", exColor[i]);
                                cmd3.Parameters.AddWithValue("@med", exMedida[i]);
                                cmd3.Parameters.AddWithValue("@can", int.Parse(exCant[i]));
                                cmd3.ExecuteNonQuery();
                            }
                        }

                        // 4) INSERT imagen (si existe)
                        if (imgBytes != null)
                        {
                            using (var cmd4 = new SqlCommand(@"
    INSERT INTO dbo.IMAGENES
      (idcolor,foto,descripcion,activo,fecha)
    VALUES
      (@col,@bin,@desc,1,GETDATE())", cn, tx))
                            {
                                // En lugar de operador ternario, usamos if/else:
                                if (exColor.Length > 0)
                                    cmd4.Parameters.AddWithValue("@col", exColor[0]);
                                else
                                    cmd4.Parameters.AddWithValue("@col", DBNull.Value);

                                cmd4.Parameters.AddWithValue("@bin", imgBytes);
                                cmd4.Parameters.AddWithValue("@desc", txtImgDesc.Text.Trim());
                                cmd4.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            Response.Redirect("Productos.aspx");
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Productos.aspx");
        }
    }
}
