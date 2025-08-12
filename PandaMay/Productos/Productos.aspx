<%@ Page 
    Title="Productos"
    Language="C#"
    MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true"
    CodeBehind="Productos.aspx.cs"
    Inherits="PandaMay.Productos.Productos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
  Productos
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <!-- Aquí tu CSS adicional -->
  <style>
    /* Contenedor principal */
    .detalle-tarjeta {
      border: 2px solid #e0e0e0;
      border-radius: 12px;
      background-color: #fafafa;
      padding: 20px;
      margin-top: 20px;
      box-shadow: 0 0 10px rgba(0,0,0,0.1);
    }

    /* Cabecera: datos + foto */
    .detalle-cabecera {
      display: flex;
      gap: 20px;
      align-items: flex-start;
    }

    /* Foto del producto */
    .producto-img {
      width: 180px;
      height: 180px;
      object-fit: cover;
      border-radius: 10px;
      border: 1px solid #ccc;
    }

    /* Bloque de texto con los datos */
    .detalle-info p {
      margin: 5px 0;
      font-size: 15px;
    }

    /* Grid de precios */
    .detalle-precios {
      margin-top: 20px;
    }
    .tabla-precios {
      width: 100%;
      border-collapse: collapse;
      font-size: 14px;
    }
    .tabla-precios th,
    .tabla-precios td {
      padding: 8px;
      border: 1px solid #ccc;
    }
    .tabla-precios th {
      background-color: #f0f0f0;
      font-weight: bold;
    }
  </style>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">

  <asp:Panel ID="pnlTabla" runat="server" Visible="true">

    <!-- Cabecera: botones y buscador -->
    <div style="display:flex; justify-content:center; align-items:center; gap:10px; margin:30px 0;">
     <asp:HyperLink ID="lnkRegresar" runat="server"
  CssClass="orangebutton"
  Text="Regresar"
  NavigateUrl="~/Login.aspx" />


      <asp:Button ID="btnCrearProducto" runat="server" CssClass="greenbutton"
        Text="Crear Producto" OnClick="btnCrearProducto_Click" />

      <asp:TextBox ID="txtBuscar" runat="server" CssClass="buscador"
        placeholder="Buscar Productos..." AutoPostBack="true"
        OnTextChanged="txtBuscar_TextChanged" />
    </div>

    <!-- Flex principal -->
    <div style="display:flex; width:100%;">

      <!-- Panel izquierdo: detalles -->
      <div style="width:35%; padding-right:20px;">
        <asp:Panel ID="pnlDetalles" runat="server"
           Visible="false"
           CssClass="detalle-tarjeta">
          <h4>Detalles completos</h4>

          <!-- Detalles del producto seleccionado -->
          <div class="detalle-cabecera">
            <!-- Datos textuales -->
            <div class="detalle-info">
              <p><strong>Nombre:</strong> <asp:Label ID="lblNombre" runat="server" /></p>
              <p><strong>Referencia:</strong> <asp:Label ID="lblReferencia" runat="server" /></p>
              <p><strong>Código de barras:</strong> <asp:Label ID="lblCodigoBarras" runat="server" /></p>
              <p><strong>Descuento:</strong> <asp:Label ID="lblDescuento" runat="server" /></p>
              <p><strong>Marca:</strong> <asp:Label ID="lblMarca" runat="server" /></p>
              <p><strong>Unidad:</strong> <asp:Label ID="lblUnidad" runat="server" /></p>
              <p><strong>Categoría:</strong> <asp:Label ID="lblCategoria" runat="server" /></p>
              <p><strong>Subcategoría:</strong> <asp:Label ID="lblSubcategoria" runat="server" /></p>
              <p><strong>Categoría maestra:</strong> <asp:Label ID="lblCatMaestra" runat="server" /></p>
              <p><strong>Tipo:</strong> <asp:Label ID="lblTipo" runat="server" /></p>
              <p><strong>Activo:</strong> <asp:CheckBox ID="chkActivo" runat="server" Enabled="false" /></p>
            </div>
            <!-- Imagen del producto -->
            <asp:Image ID="imgFotoDetalle" runat="server"
                       CssClass="producto-img" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvCombosProductos" runat="server" AutoGenerateColumns="true"
              GridLines="Both" BorderColor="#ccc" BorderWidth="1px" Width="100%" />
          </div>
          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvDetallesCompras" runat="server" AutoGenerateColumns="true"
              GridLines="Both" BorderColor="#ccc" BorderWidth="1px" Width="100%" />
          </div>
          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvDetallesTraslados" runat="server" AutoGenerateColumns="true"
              GridLines="Both" BorderColor="#ccc" BorderWidth="1px" Width="100%" />
          </div>
          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvDetallesVentas" runat="server" AutoGenerateColumns="true"
              GridLines="Both" BorderColor="#ccc" BorderWidth="1px" Width="100%" />
          </div>

          <div class="detalle-precios">
           <asp:GridView ID="gvPrecios" runat="server"
    AutoGenerateColumns="false"
    CssClass="tabla-precios">
  <Columns>
    <%-- <asp:BoundField DataField="idprecio" HeaderText="ID Precio" /> --%>
    <%--<asp:BoundField DataField="idproducto" HeaderText="ID Producto" />--%>
    <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
    <asp:BoundField DataField="precio" HeaderText="Precio" DataFormatString="{0:C}" />
    <asp:BoundField DataField="cantidad" HeaderText="Cantidad" />
    <asp:CheckBoxField DataField="activo" HeaderText="Activo" />
  </Columns>
            </asp:GridView>
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
  <asp:GridView ID="gvPreciosCompras" runat="server"
      AutoGenerateColumns="false"
      ShowHeaderWhenEmpty="True"
      EmptyDataText="Sin precios de compra registrados."
      GridLines="Both" BorderColor="#ccc" BorderWidth="1px" Width="100%">
    <HeaderStyle BackColor="Black" ForeColor="White" Font-Bold="True" />
    <Columns>
      <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
      <asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="{0:C}" HtmlEncode="false" />
      <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
    </Columns>
  </asp:GridView>
</div>


          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvAtributos" runat="server" AutoGenerateColumns="true"
              GridLines="Both" BorderColor="#ccc" BorderWidth="1px" Width="100%" />
          </div>
          <div style="overflow-x:auto;">
           <asp:GridView 
    ID="gvExistencias" 
    runat="server" 
    AutoGenerateColumns="false"
    CssClass="tabla-precios"
    GridLines="Both" 
    BorderColor="#ccc" 
    BorderWidth="1px" 
    Width="100%">
  <Columns>
    <asp:BoundField DataField="idexistencia" HeaderText="ID Existencia" />
    <asp:BoundField DataField="Color"       HeaderText="Color" />
    <asp:BoundField DataField="Publico"     HeaderText="Público" />
    <asp:BoundField DataField="Medida"      HeaderText="Medida" />
    <asp:BoundField DataField="cantidad"    HeaderText="Cantidad" />
  </Columns>
</asp:GridView>


          </div>
        </asp:Panel>
      </div>

      <!-- Panel derecho: lista de productos -->
      <div style="width:65%; margin-left:auto; overflow-x:auto;">
  <asp:GridView 
      ID="GridView1" 
      runat="server"
      DataKeyNames="idproducto,idimagen"
      AutoGenerateColumns="false"
      AutoGenerateSelectButton="True"
      ShowHeaderWhenEmpty="True"
      EmptyDataText="No hay productos para mostrar."
      OnSelectedIndexChanging="Select1"
      OnRowDataBound="GridView1_RowDataBound"
      GridLines="Both"
      BorderColor="#ccc"
      BorderWidth="1px"
      CellSpacing="5"
      HorizontalAlign="Center"
      Width="100%">
          
          <HeaderStyle BackColor="Black" ForeColor="White" Font-Bold="True" />
          <SelectedRowStyle BackColor="#80cdbb" Font-Bold="True" />

          <Columns>
            <asp:TemplateField HeaderText="Producto">
              <ItemTemplate>
                <asp:Label ID="lblNombre" runat="server"
                  Text='<%# Eval("nombre") %>' />
              </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Precio (U)">
              <ItemTemplate>
                <asp:Label ID="lblPrecioU" runat="server"
                  Text='<%# Eval("unidad", "{0:C}") %>' />
              </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Precio (3)">
              <ItemTemplate>
                <asp:Label ID="lblPrecio3" runat="server"
                  Text='<%# Eval("tresomas", "{0:C}") %>' />
              </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Precio (Doc)">
              <ItemTemplate>
                <asp:Label ID="lblDocena" runat="server"
                  Text='<%# Eval("docena", "{0:C}") %>' />
              </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Precio (F)">
              <ItemTemplate>
                <asp:Label ID="lblFardo" runat="server"
                  Text='<%# Eval("fardo", "{0:C}") %>' />
              </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Código de barras">
              <ItemTemplate>
                <asp:Label ID="lblCodigoBarras" runat="server"
                  Text='<%# Eval("codigodebarras") %>' />
              </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Referencia">
              <ItemTemplate>
                <asp:Label ID="lblReferencia" runat="server"
                  Text='<%# Eval("referencia") %>' />
              </ItemTemplate>
            </asp:TemplateField>

              <%-- campo oculto para capturar el id de la imagen --%>
<asp:BoundField DataField="idimagen" Visible="false" />


            <asp:TemplateField HeaderText="Imagen">
              <ItemTemplate>
                <asp:Image ID="imgFoto" runat="server" Width="60" Height="60" />
              </ItemTemplate>
            </asp:TemplateField>

            <%-- NUEVO campo que usa el helper GetPreciosHtml --%>
            <asp:TemplateField HeaderText="Precios">
  <ItemTemplate>
    <asp:Literal runat="server" Mode="PassThrough"
      Text='<%# GetPreciosHtml(Container.DataItem) %>' />
  </ItemTemplate>
</asp:TemplateField>

          </Columns>
        </asp:GridView>
      </div>
    </div>
  </asp:Panel>

</asp:Content>
