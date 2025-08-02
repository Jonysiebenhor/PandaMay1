using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;


namespace PandaMay.Empleados
{
    public partial class DetalleVentas : System.Web.UI.Page
    {
        Conectar conectado = new Conectar();
        double diferencia;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["usuario"] is null)
                {
                    Response.Redirect("/Login.aspx");
                }
                String usuario = Session["usuario"].ToString();
                TextBox36.Text = usuario;
                
                conectado.conectar();
                GridView1.DataSource = conectado.GetProductosConTarifas();
                GridView1.DataBind();

                conectado.desconectar();

                foreach (GridViewRow roww in GridView3.Rows)//crea la suma para el total del gridview
                {
                    double total = 0;
                    total = Convert.ToDouble(roww.Cells[3].Text);
                    Label1.Text = Convert.ToString(total);
                    
                }
            }
            Label17.Visible = false;
            Label8.Text = Label1.Text;
            
            string total2 = Convert.ToString(Label8.Text);
            String total3= Convert.ToString(TextBox37.Text);
            if (total3 == "")
            {
                total3 = "0";
            }
            String total4 = Convert.ToString(TextBox8.Text);
            if (total4 == "")
            {
                total4 = "0";
            }
            String total5 = Convert.ToString(TextBox10.Text);
            if (total5 == "")
            {
                total5 = "0";
            }
            double total22 = Convert.ToDouble(total2);
            double total33 = Convert.ToDouble(total3);
            double total44 = Convert.ToDouble(total4);
            double total55 = Convert.ToDouble(total5);
            diferencia =total22-total33-total44-total55;
            Label10.Text= Convert.ToString(diferencia);//diferencia del total de la venta, con lo que se va a cobrar.
            DropDownList1.SelectedValue = DropDownList5.SelectedValue;

        }

        protected void ddltienda_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            String usuario = Session["usuario"].ToString();
            conectado.conectar();
            string buscar = TextBox1.Text;
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                GridView1.DataSource = conectado.BuscarProductosConTarifas(buscar);
            }
            else
            {
                GridView1.DataSource = conectado.GetProductosConTarifas();
            }

            conectado.desconectar();

        }
        DataTable dt = new DataTable();
        protected void Select1(object sender, GridViewSelectEventArgs e)
        {
            // 1) Fila seleccionada
            GridViewRow fila = GridView1.Rows[e.NewSelectedIndex];

            // 2) Leer los labels por ID
            string nombre = (fila.FindControl("lblNombreProd") as Label).Text;
            string txtU = (fila.FindControl("lblPrecioU") as Label).Text;
            string txt3 = (fila.FindControl("lblPrecio3") as Label).Text;
            string txtDoc = (fila.FindControl("lblDocena") as Label).Text;
            string txtF = (fila.FindControl("lblFardo") as Label).Text;

            // 3) Parsear cada precio como moneda
            decimal precioU = Decimal.Parse(txtU, NumberStyles.Currency, CultureInfo.CurrentCulture);
            decimal precio3 = Decimal.Parse(txt3, NumberStyles.Currency, CultureInfo.CurrentCulture);
            decimal precioDoc = Decimal.Parse(txtDoc, NumberStyles.Currency, CultureInfo.CurrentCulture);
            decimal precioF = Decimal.Parse(txtF, NumberStyles.Currency, CultureInfo.CurrentCulture);

            // 4) Opcional: a int si no manejas decimales
            int unidad = (int)precioU;
            int tresomas = (int)precio3;
            int docena = (int)precioDoc;
            int fardo = (int)precioF;

            // 5) Lógica de carrito (GridView2/3)
            if (GridView3.Rows.Count > 0)
            {
                // Reconstruimos dt con las filas actuales
                dt = new DataTable();
                foreach (TableCell th in GridView3.HeaderRow.Cells)
                    dt.Columns.Add(th.Text);
                foreach (GridViewRow r in GridView3.Rows)
                {
                    var dr = dt.NewRow();
                    for (int i = 0; i < r.Cells.Count; i++)
                        dr[i] = r.Cells[i].Text;
                    dt.Rows.Add(dr);
                }

                // ¿Ya existe el producto?
                int idx = dt.Rows.Cast<DataRow>()
                                 .ToList()
                                 .FindIndex(r => r[1].ToString() == nombre);
                if (idx >= 0)
                {
                    int vieja = Convert.ToInt32(dt.Rows[idx][0]);
                    dt.Rows.RemoveAt(idx);
                    int nuevaCant = vieja + 1;

                    int precioAplicado = nuevaCant < 3 ? unidad
                                         : nuevaCant < 12 ? tresomas
                                         : docena;
                    dt.Rows.Add(nuevaCant, nombre, precioAplicado, nuevaCant * precioAplicado);
                }
                else
                {
                    dt.Rows.Add(1, nombre, unidad, unidad);
                }
            }
            else
            {
                // Carrito vacío: armamos dt de cero
                dt = new DataTable();
                dt.Columns.Add("Cantidad"); dt.Columns.Add("Nombre");
                dt.Columns.Add("Precio"); dt.Columns.Add("Total");
                var dr = dt.NewRow();
                dr["Cantidad"] = 1;
                dr["Nombre"] = nombre;
                dr["Precio"] = unidad;
                dr["Total"] = unidad;
                dt.Rows.Add(dr);
            }

            // 6) Bindeamos a ambos grids
            GridView2.DataSource = dt; GridView2.DataBind();
            GridView3.DataSource = dt; GridView3.DataBind();

            // 7) Recalculamos total
            double total = 0;
            foreach (GridViewRow rw in GridView3.Rows)
                total += Convert.ToDouble(rw.Cells[3].Text);
            Label1.Text = total.ToString("N2");
        }



        /// <summary>
        /// Carga la miniatura de imagen para cada fila del grid.
        /// </summary>
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var drv = (DataRowView)e.Row.DataItem;
            int idImg = drv["idimagen"] != DBNull.Value
                ? Convert.ToInt32(drv["idimagen"])
                : 0;

            var img = (Image)e.Row.FindControl("imgMini");
            if (img == null) return;

            if (idImg > 0)
                img.ImageUrl = $"~/VerImagen.ashx?imgid={idImg}&t={DateTime.Now.Ticks}";
            else
                img.ImageUrl = ResolveUrl("~/uploads/productos/aretes.png");
        }

        protected void RowUpdatingEvent1(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            GridViewRow fila = GridView2.Rows[e.RowIndex];
            //string punto = ".";
            char uno = char.Parse("1");
            char dos = char.Parse("2");
            String nombre = (fila.FindControl("TextBox10") as System.Web.UI.WebControls.TextBox).Text.ToUpper();
            String cantidadd = (fila.FindControl("TextBox9") as System.Web.UI.WebControls.TextBox).Text.ToUpper();
           /* for (int j = 0; j < cantidadd.Length; j++)//validar que la cantidad no sea un decimal
            {
                if (cantidadd=cantidad)
                if (uno == cantidadd[j] || dos == cantidadd[j])
                {
                }
                else
                {
                    Response.Write("<script>alert('X ERROR X: No se puede agregar decimales en la cantidad')</script>");
                }
            }*/
                    int cantidad = Convert.ToInt32(cantidadd);
                    String precioo = (fila.FindControl("TextBox11") as System.Web.UI.WebControls.TextBox).Text.ToUpper();
                    double precio = Convert.ToDouble(precioo);
                    String totall = (fila.FindControl("TextBox12") as System.Web.UI.WebControls.TextBox).Text.ToUpper();
                    double total = Convert.ToDouble(totall);
                    int kp = Convert.ToInt32(e.RowIndex);//se obtiene la fila del gridview que se esta modificando
                    double total2 = cantidad * precio;
                    for (int i = 0; i < GridView3.HeaderRow.Cells.Count; i++)
                    {
                        dt.Columns.Add(GridView3.HeaderRow.Cells[i].Text); //se agregan los nombres de las columnas
                    }
                    //  add each of the data rows to the table
                    foreach (GridViewRow row in GridView3.Rows)
                    {
                        DataRow dr;
                        dr = dt.NewRow();

                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            dr[i] = row.Cells[i].Text;//se agregan los datos de las columnas
                        }
                        dt.Rows.Add(dr);
                    }
                    String df = Convert.ToString(GridView3.Rows[kp].Cells[2].Text);
                    double dff = Convert.ToDouble(df);
                    conectado.conectar();
                    String buscar = nombre;
                    DataRow rows = conectado.buscarproducto(buscar).Rows[0];
                    String codigodebarras = Convert.ToString(Convert.ToString(rows["codigodebarras"]));
                    String referencia = Convert.ToString(Convert.ToString(rows["referencia"]));
                    String nombreproducto = Convert.ToString(Convert.ToString(rows["nombre"]));
                    double unidad = Convert.ToDouble(Convert.ToString(rows["unidad"]));
                    double tresomas = Convert.ToDouble(Convert.ToString(rows["tresomas"]));
                    double docena = Convert.ToDouble(Convert.ToString(rows["docena"]));
                    //double fardo = Convert.ToDouble(Convert.ToString(rows["fardo"]));
                    if (precio != dff)
                    {
                        dt.Rows.Add(cantidad, nombre, precio, total2);
                        dt.Rows.RemoveAt(kp);//se elimina la linea que contiene registro con ese nombre
                    }
                    else
                    {
                        if (cantidad < 3)//se agrega la linea que contiene registro con ese nombre pero con mas cantidad y se cambia el precio
                        {
                            dt.Rows.Add(cantidad, nombre, unidad, (cantidad * unidad));
                            dt.Rows.RemoveAt(kp);//se elimina la linea que contiene registro con ese nombre
                        }
                        else if (cantidad >= 3 && cantidad < 12)
                        {
                            dt.Rows.Add(cantidad, nombre, tresomas, (cantidad * tresomas));
                            dt.Rows.RemoveAt(kp);//se elimina la linea que contiene registro con ese nombre
                        }
                        else if (cantidad >= 12)
                        {
                            dt.Rows.Add(cantidad, nombre, docena, (cantidad * docena));
                            dt.Rows.RemoveAt(kp);//se elimina la linea que contiene registro con ese nombre
                        }
                    }

                    GridView2.EditIndex = -1;
                    GridView2.DataSource = dt;
                    GridView2.DataBind();
                    GridView3.DataSource = dt;
                    GridView3.DataBind();
                    double totalt = 0;
                    foreach (GridViewRow roww in GridView3.Rows)//crea la suma para el total del gridview
                    {
                        totalt += Convert.ToDouble(roww.Cells[3].Text);
                    }
                    Label1.Text = Convert.ToString(totalt);
                  
            
        }

        protected void RowEditingEvent1(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            GridView2.EditIndex = e.NewEditIndex;
        }
        protected void RowCancelingEvent1(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            for (int i = 0; i < GridView3.HeaderRow.Cells.Count; i++)
            {
                dt.Columns.Add(GridView3.HeaderRow.Cells[i].Text); //se agregan los nombres de las columnas
            }
            //  add each of the data rows to the table
            foreach (GridViewRow row in GridView3.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text;//se agregan los datos de las columnas
                }
                dt.Rows.Add(dr);
            }
            GridView2.EditIndex = -1;
            GridView2.DataSource = dt;
            GridView2.DataBind();
            GridView3.DataSource = dt;
            GridView3.DataBind();
        }
        protected void Eliminar(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            GridViewRow fila = GridView2.Rows[e.RowIndex];
            for (int i = 0; i < GridView3.HeaderRow.Cells.Count; i++)
            {
                dt.Columns.Add(GridView3.HeaderRow.Cells[i].Text); //se agregan los nombres de las columnas
            }
            //  add each of the data rows to the table
            foreach (GridViewRow row in GridView3.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text;//se agregan los datos de las columnas
                }
                dt.Rows.Add(dr);
            }
            dt.Rows.RemoveAt(e.RowIndex);
            GridView2.DataSource = dt;
            GridView2.DataBind();
            GridView3.DataSource = dt;
            GridView3.DataBind();
            double totalt = 0;
            foreach (GridViewRow roww in GridView3.Rows)//crea la suma para el total del gridview
            {
                totalt += Convert.ToDouble(roww.Cells[3].Text);
            }
            Label1.Text = Convert.ToString(totalt);
        }

        protected void Anterior(object sender, EventArgs e)
        {
            Response.Redirect("/empleados/ventas.aspx");
        }
        protected void anterior2(object sender, EventArgs e)
        {
            div1.Visible = true;
            div2.Visible = false;
        }
        protected void Siguiente(object sender, EventArgs e)
        {
            if (DropDownList5.SelectedValue == "0")
            {
                Response.Write("<script>alert('ERROR: Ingrese tienda de venta')</script>");
            }
            else if (GridView2.Rows.Count>0)
            {
                div1.Visible = false;
                div2.Visible = true;
            }
            else
            {
                Response.Write("<script>alert('ERROR: Ingrese algun producto')</script>");
            }
        }
        protected void guardar(object sender, EventArgs e)
        {
            if (TextBox8.Text != "" || TextBox17.Text != "" || DropDownList2.SelectedValue != "0" || TextBox9.Text != "")
            {
                if (TextBox8.Text == "" || TextBox17.Text == "" || DropDownList2.SelectedValue == "0" || TextBox9.Text == "")
                {
                    //verificar lo de banco
                    Response.Write("<script>alert('ERROR: Faltan Datos de la Transferencia 1')</script>");
                }
                else
                {
                    // colocar la cadena de insert que va en los pagos conectado.conectar
                }
            }
            else if (TextBox10.Text != "" || TextBox11.Text != "" || DropDownList3.SelectedValue != "0" || TextBox12.Text != "")
            {
                if (TextBox10.Text == "" || TextBox11.Text == "" || DropDownList3.SelectedValue == "0" || TextBox12.Text == "")
                {
                    //verificar lo de banco
                    Response.Write("<script>alert('ERROR: Faltan Datos de la Transferencia 2')</script>");
                }
                else
                {
                    // colocar la cadena de insert que va en los pagos conectado.conectar
                    
                }
            }
            else if (diferencia != 0)
            {
                Label17.Visible = true;
                Label17.Text = "La diferencia debe ser cero (0)";
                // TextBox8.BorderColor.
                //Response.Write("<script>alert('ERROR: Corriga los pagos')</script>");

                div1.Visible = false;
                div2.Visible = true;
            }
            else if (txtnombre.Text == "" || ddlzona.Text== "--Zona, Aldea, Lugar" || TextBox2.Text== "" || TextBox3.Text == "" || TextBox31.Text == "")
            {

                Response.Write("<script>alert('ERROR: Faltan Datos')</script>");

                div1.Visible = false;
                div2.Visible = true;
            }
            else
            {
                conectado.conectar();
                String usuario = TextBox31.Text;
                String dpi = null;
                String primerNombre = txtnombre.Text;
                String segundoNombre = null;
                String tercerNombre = null;
                String primerApellido = null;
                String segundoApellido = null;
                String apellidoCasada = null;
                String nombrenegocio = null;
                String producto = null;
                String idioma = "Español";
                String correo = null;
                String nit = null;
                String telefono = TextBox3.Text;
                String nacimiento = null;
                String genero = null;
                String activo = "true";
                String nombrebanco = null;
                String tipocuenta = null;
                String cuentabancaria = null;
                String contraseña = null;

                //aca se crea el usuariocliente
                conectado.usuarios(usuario, dpi, primerNombre, segundoNombre, tercerNombre, primerApellido, segundoApellido, apellidoCasada, nombrenegocio, producto, idioma, correo, nit, telefono, nacimiento, genero, activo, nombrebanco, tipocuenta, cuentabancaria, contraseña);
                
                // aca se crea el cliente
                DataRow rows1 = conectado.ultimousuario().Rows[0];              //obtener el idusuariocliente
                String idusuario = Convert.ToString(Convert.ToString(rows1["idusuario"]));   //obtener el idusuariocliente
                String descuento = null;
                String tarifa = null;
                String codigodebarras = null;
                conectado.clientes( idusuario,  descuento,  tarifa,  codigodebarras);

                // aca se manda a llamar el id del cliente que se acaba de crear
                DataRow rows2 = conectado.ultimocliente().Rows[0];              //obtener el idcliente
                String idcliente = Convert.ToString(Convert.ToString(rows2["idcliente"]));   //obtener el idcliente

                //aca se ingresa al empleado a la venta que se esta realizando
                String usuario1 = Session["usuario"].ToString();                             //obtener el idempleado
                DataRow rows = conectado.consultaUsuarioloign(usuario1).Rows[0];              //obtener el idempleado
                String idempleado = Convert.ToString(Convert.ToString(rows["idusuario"]));   //obten    er el idempleado
                String idtienda = DropDownList5.SelectedValue;   //obtener el idtienda
                String tipocomprobante = null; 
                String estado = "activo"; 
                String referencia = null;
                conectado.ventas(idcliente, idempleado, idtienda, tipocomprobante, estado, referencia);

                //aca se ingresan los datos de la direccion del cliente
                String iddepartamento = ddldepartamento.SelectedValue;
                DataRow rows4 = conectado.pais(iddepartamento).Rows[0];              //obtener el idpais
                String idpais = Convert.ToString(Convert.ToString(rows4["idpais"])); 
                String idmunicipio= ddlmunicipio.SelectedValue;
                String idzona= ddlzona.SelectedValue;
                DataRow rows3 = conectado.tarifas(idzona).Rows[0];              //obtener el idempleado
                String idmensajeria = Convert.ToString(Convert.ToString(rows3["idmensajeria"]));//obtener la idmensjaeria
                String idtarifa = Convert.ToString(Convert.ToString(rows3["idtarifa"]));
                String direccion = TextBox2.Text;
                //ingresar la direccion
                conectado.direcciones(idcliente, idpais, iddepartamento, idmunicipio, idzona, idtarifa, idempleado, idtienda , idmensajeria, direccion);

                //ingresar pago en efectivo


                conectado.desconectar();
                Response.Write("<script>alert('Exito al guardar')</script>");
            }
        }
        protected void ddldepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlmunicipio.Items.Clear();
            ddlzona.Items.Clear();
            ddlzona0.Items.Clear();
            montoenvio.Text = "";

            ddlmunicipio.Items.Insert(0, new ListItem("--Municipio", "0"));
            ddlzona.Items.Insert(0, new ListItem("--Zona, Aldea, Lugar", "0"));

        }
        protected void ddlmunicipio_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlzona.Items.Clear();
            ddlzona0.Items.Clear();
            montoenvio.Text = "";
            ddlzona.Items.Insert(0, new ListItem("--Zona, Aldea, Lugar", "0"));

        }
        protected void ddlzona_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlzona0.Items.Clear();
        }
        protected void ddlzona0_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
            protected void ddlbanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList2.SelectedValue != "0")
            { 
            conectado.conectar();
            int banco =Convert.ToInt32(DropDownList2.Text)-1;
            //TextBox38.Text = banco;
            DataRow rows = conectado.cuentabanco().Rows[banco];
             TextBox38.Text = Convert.ToString(Convert.ToString(rows["numerodecuenta"]));
            }
            else
            {
                TextBox38.Text = "Numero de cuenta";
            }
        }
        protected void ddlbanco2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList3.SelectedValue != "0")
            {
                conectado.conectar();
                int banco = Convert.ToInt32(DropDownList3.Text) - 1;
                //TextBox38.Text = banco;
                DataRow rows = conectado.cuentabanco().Rows[banco];
                TextBox39.Text = Convert.ToString(Convert.ToString(rows["numerodecuenta"]));
            }
            else
            {
                TextBox39.Text = "Numero de cuenta";
            }
        }
        protected void DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
        }

       /* protected void Button2_Click(object sender, EventArgs e)
        {

        }*/

    }
}