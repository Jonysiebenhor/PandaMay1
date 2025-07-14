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
            // 1) Limpio y coloco el "-- Seleccione --" en cada DropDown
            ddlSubcategoria.Items.Clear();
            ddlSubcategoria.Items.Add(new ListItem("-- Subcategoría --", ""));
            ddlUnidad.Items.Clear();
            ddlUnidad.Items.Add(new ListItem("-- Unidad --", ""));
            ddlMarca.Items.Clear();
            ddlMarca.Items.Add(new ListItem("-- Marca --", ""));
            ddlTienda.Items.Clear();
            ddlTienda.Items.Add(new ListItem("-- Tienda --", ""));
            ddlColor.Items.Clear();
            ddlColor.Items.Add(new ListItem("-- Color --", ""));
            ddlColor.Items.Add(new ListItem("Otro...", "0"));  // opción para Color personalizado
            ddlMedida.Items.Clear();
            ddlMedida.Items.Add(new ListItem("-- Medida --", ""));
            ddlImgColor.Items.Clear();
            ddlImgColor.Items.Add(new ListItem("-- Color --", ""));

            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

                // Subcategorías
                using (var cmd = new SqlCommand(
                    "SELECT idsubcategoria, nombre FROM dbo.SUBCATEGORIAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlSubcategoria.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idsubcategoria"].ToString()));

                // Unidades de medida
                using (var cmd = new SqlCommand(
                    "SELECT idunidaddemedida, nombre FROM dbo.UNIDADDEMEDIDAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlUnidad.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idunidaddemedida"].ToString()));

                // Marcas
                using (var cmd = new SqlCommand(
                    "SELECT idmarca, nombre FROM dbo.MARCAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlMarca.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idmarca"].ToString()));

                // Tiendas
                using (var cmd = new SqlCommand(
                    "SELECT idtienda, nombre FROM dbo.TIENDAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlTienda.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idtienda"].ToString()));

                // **Colores** (ahora SIN filtrar por 'activo')
                using (var cmd = new SqlCommand(
                    "SELECT idcolor, nombre FROM dbo.COLORES", cn))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var txt = dr["nombre"].ToString();
                        var val = dr["idcolor"].ToString();
                        ddlColor.Items.Add(new ListItem(txt, val));
                        ddlImgColor.Items.Add(new ListItem(txt, val));
                    }
                }

                // Medidas
                using (var cmd = new SqlCommand(
                    "SELECT idmedida, nombre FROM dbo.MEDIDAS WHERE activo=1", cn))
                using (var dr = cmd.ExecuteReader())
                    while (dr.Read())
                        ddlMedida.Items.Add(new ListItem(
                            dr["nombre"].ToString(),
                            dr["idmedida"].ToString()));
            }
        }

        protected void ddlColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlColorOtro.Visible = ddlColor.SelectedValue == "0";
        }

        protected void btnGuardarProducto_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            // --- Validaciones con TryParse ---
            if (!int.TryParse(ddlSubcategoria.SelectedValue, out int subcat))
            { MostrarError("Debe seleccionar una subcategoría."); return; }
            if (!int.TryParse(ddlUnidad.SelectedValue, out int unidad))
            { MostrarError("Debe seleccionar una unidad de medida."); return; }
            if (!int.TryParse(ddlMarca.SelectedValue, out int marca))
            { MostrarError("Debe seleccionar una marca."); return; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MostrarError("El nombre no puede quedar vacío."); return; }
            if (!DateTime.TryParse(txtFecha.Text, out DateTime fecha))
            { MostrarError("La fecha no es válida."); return; }
            if (!long.TryParse(txtCodigoBarras.Text.Trim(), out long codBarras))
            { MostrarError("El código de barras debe ser un número."); return; }
            if (!float.TryParse(txtDescuento.Text.Trim(), out float descuento))
            { MostrarError("El descuento debe ser un número."); return; }
            if (!decimal.TryParse(txtPrecioUnidad.Text.Trim(), out decimal precioUni))
            { MostrarError("El precio unidad debe ser decimal."); return; }
            if (!decimal.TryParse(txtPrecioTres.Text.Trim(), out decimal precioTres))
            { MostrarError("El precio “3 o más” debe ser decimal."); return; }
            if (!decimal.TryParse(txtPrecioDocena.Text.Trim(), out decimal precioDocena))
            { MostrarError("El precio docena debe ser decimal."); return; }
            if (!decimal.TryParse(txtPrecioFardo.Text.Trim(), out decimal precioFardo))
            { MostrarError("El precio fardo debe ser decimal."); return; }
            if (!decimal.TryParse(txtPrecioCompra.Text.Trim(), out decimal precioCompra))
            { MostrarError("El precio de compra debe ser decimal."); return; }
            if (!int.TryParse(ddlTienda.SelectedValue, out int tienda))
            { MostrarError("Debe seleccionar una tienda."); return; }
            if (!int.TryParse(ddlMedida.SelectedValue, out int medida))
            { MostrarError("Debe seleccionar una medida."); return; }
            if (!int.TryParse(txtExistencia.Text.Trim(), out int existencia))
            { MostrarError("La existencia debe ser un número entero."); return; }

            // Campos opcionales
            string referencia = txtReferencia.Text.Trim();
            bool activo = chkActivo.Checked;
            string tipoProd = txtTipo.Text.Trim();
            string atrNom = txtAtrNombre.Text.Trim();
            string atrDesc = txtAtrDesc.Text.Trim();
            string descCompra = txtDescCompra.Text.Trim();
            string imgDesc = txtImgDesc.Text.Trim();

            // **Color dinámico** (si eligió “Otro...”)
            int colorId;
            if (pnlColorOtro.Visible &&
                !string.IsNullOrWhiteSpace(txtColorOtro.Text))
            {
                using (var cn = new SqlConnection(_connString))
                using (var cmd = new SqlCommand(@"
    INSERT INTO dbo.COLORES (nombre, fecha)
    OUTPUT INSERTED.idcolor
    VALUES (@nom, GETDATE())", cn))
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@nom", txtColorOtro.Text.Trim());
                    colorId = (int)cmd.ExecuteScalar();
                }
            }
            else
            {
                colorId = int.Parse(ddlColor.SelectedValue);
            }

            // --- Leer bytes de la imagen si hay ---
            byte[] imgBytes = null;
            if (fuImagen.HasFile)
            {
                using (var ms = new MemoryStream())
                {
                    fuImagen.PostedFile.InputStream.CopyTo(ms);
                    imgBytes = ms.ToArray();
                }
            }

            // --- Inserción en BD con transacción ---
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        int newId;
                        // 1) PRODUCTOS
                        using (var cmd = new SqlCommand(@"
    INSERT INTO dbo.PRODUCTOS
      (idsubcategoria,idunidaddemedida,idmarca,nombre,fecha,
       referencia,codigodebarras,activo,tipodeproducto,descuento)
    OUTPUT INSERTED.idproducto
    VALUES
      (@sub,@uni,@mar,@nom,@fec,@ref,@cb,@act,@tip,@des)",
                            cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@sub", subcat);
                            cmd.Parameters.AddWithValue("@uni", unidad);
                            cmd.Parameters.AddWithValue("@mar", marca);
                            cmd.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@fec", fecha);
                            cmd.Parameters.AddWithValue("@ref", referencia);
                            cmd.Parameters.AddWithValue("@cb", codBarras);
                            cmd.Parameters.AddWithValue("@act", activo);
                            cmd.Parameters.AddWithValue("@tip", tipoProd);
                            cmd.Parameters.AddWithValue("@des", descuento);
                            newId = (int)cmd.ExecuteScalar();
                        }

                        // 2) PRECIOS VENTA…
                        var tiposV = new[] { "unidad", "3 o más", "docena", "fardo" };
                        var valsV = new[] { precioUni, precioTres, precioDocena, precioFardo };
                        for (int i = 0; i < tiposV.Length; i++)
                        {
                            using (var cmd2 = new SqlCommand(@"
    INSERT INTO dbo.PRECIOS
      (idproducto,nombre,precio,fecha,activo)
    VALUES
      (@id,@nom,@pre,GETDATE(),1)", cn, tx))
                            {
                                cmd2.Parameters.AddWithValue("@id", newId);
                                cmd2.Parameters.AddWithValue("@nom", tiposV[i]);
                                cmd2.Parameters.AddWithValue("@pre", valsV[i]);
                                cmd2.ExecuteNonQuery();
                            }
                        }

                        // 3) PRECIO COMPRA…
                        using (var cmd3 = new SqlCommand(@"
    INSERT INTO dbo.PRECIOSCOMPRAS
      (idproducto,descripcion,precio,activo,fecha)
    VALUES
      (@id,@desc,@pre,1,GETDATE())", cn, tx))
                        {
                            cmd3.Parameters.AddWithValue("@id", newId);
                            cmd3.Parameters.AddWithValue("@desc", descCompra);
                            cmd3.Parameters.AddWithValue("@pre", precioCompra);
                            cmd3.ExecuteNonQuery();
                        }

                        // 4) EXISTENCIAS…
                        using (var cmd4 = new SqlCommand(@"
    INSERT INTO dbo.EXISTENCIAS
      (idtienda,idproducto,idcolor,idmedida,cantidad,fechaingreso)
    VALUES
      (@tid,@id,@col,@med,@can,GETDATE())", cn, tx))
                        {
                            cmd4.Parameters.AddWithValue("@tid", tienda);
                            cmd4.Parameters.AddWithValue("@id", newId);
                            cmd4.Parameters.AddWithValue("@col", colorId);
                            cmd4.Parameters.AddWithValue("@med", medida);
                            cmd4.Parameters.AddWithValue("@can", existencia);
                            cmd4.ExecuteNonQuery();
                        }

                        // 5) ATRIBUTOS (opcional)…
                        if (!string.IsNullOrWhiteSpace(atrNom))
                        {
                            using (var cmd5 = new SqlCommand(@"
    INSERT INTO dbo.ATRIBUTOS
      (idproducto,nombre,descripcion,activo,fecha)
    VALUES
      (@id,@nom,@des,1,GETDATE())", cn, tx))
                            {
                                cmd5.Parameters.AddWithValue("@id", newId);
                                cmd5.Parameters.AddWithValue("@nom", atrNom);
                                cmd5.Parameters.AddWithValue("@des", atrDesc);
                                cmd5.ExecuteNonQuery();
                            }
                        }

                        // 6) IMÁGENES (opcional)…
                        if (imgBytes != null)
                        {
                            using (var cmd6 = new SqlCommand(@"
    INSERT INTO dbo.IMAGENES
      (idcolor,foto,descripcion,activo,fecha)
    VALUES
      (@col,@bin,@desc,1,GETDATE())", cn, tx))
                            {
                                cmd6.Parameters.AddWithValue("@col", int.Parse(ddlImgColor.SelectedValue));
                                cmd6.Parameters.AddWithValue("@bin", imgBytes);
                                cmd6.Parameters.AddWithValue("@desc", imgDesc);
                                cmd6.ExecuteNonQuery();
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

            // Redirijo al listado si todo funcionó
            Response.Redirect("Productos.aspx");
        }

        private void MostrarError(string msg)
        {
            lblError.Text = msg;
            lblError.Visible = true;
        }
    }
}
