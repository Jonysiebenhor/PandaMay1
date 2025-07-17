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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">

  <!-- Panel principal que siempre mostramos -->
  <asp:Panel ID="pnlTabla" runat="server" Visible="true">

    <!-- Cabecera: regresar, crear y buscador -->
    <div style="display:flex; justify-content:center; align-items:center; gap:10px; margin:30px 0;">
      <asp:Button
  ID="btnRegresar"
  runat="server"
  CssClass="orangebutton"
  Text="Regresar"
  OnClientClick="window.history.back(); return false;" />


      <asp:Button
        ID="btnCrearProducto"
        runat="server"
        CssClass="greenbutton"
        Text="Crear Producto"
        OnClick="btnCrearProducto_Click" />

      <asp:TextBox
        ID="txtBuscar"
        runat="server"
        CssClass="buscador"
        placeholder="Buscar Productos..."
        AutoPostBack="true"
        OnTextChanged="txtBuscar_TextChanged" />
    </div>

    <!-- Flex: izquierda = detalles (35%), derecha = lista (65%) -->
    <div style="display:flex; width:100%;">

      <!-- IZQUIERDA: Panel de detalles -->
      <div style="width:35%; padding-right:20px;">
        <asp:Panel ID="pnlDetalles" runat="server" Visible="false">
          <h4>Detalles completos</h4>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvCombosProductos" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvDetallesCompras" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvDetallesTraslados" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvDetallesVentas" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvPrecios" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvPreciosCompras" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView ID="gvAtributos" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto;">
            <asp:GridView ID="gvExistencias" runat="server"
              AutoGenerateColumns="true" GridLines="Both"
              BorderColor="#ccc" BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc" HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc" RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

        </asp:Panel>
      </div>

      <!-- DERECHA: Lista de productos -->
      <div style="width:65%; margin-left:auto; overflow-x:auto;">
        <asp:GridView ID="GridView1" runat="server"
          DataKeyNames="idproducto"
          AutoGenerateColumns="false"
          AutoGenerateSelectButton="True"
          OnSelectedIndexChanging="Select1"
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
                  Text='<%# Eval("unidad") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio (3)">
              <ItemTemplate>
                <asp:Label ID="lblPrecio3" runat="server"
                  Text='<%# Eval("tresomas") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio (Doc)">
              <ItemTemplate>
                <asp:Label ID="lblDocena" runat="server"
                  Text='<%# Eval("docena") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio (F)">
              <ItemTemplate>
                <asp:Label ID="lblFardo" runat="server"
                  Text='<%# Eval("fardo") %>' />
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
            <asp:TemplateField HeaderText="Imagen">
              <ItemTemplate>
                <asp:Label ID="lblImagen" runat="server"
                  Text='<%# Eval("foto") %>' />
              </ItemTemplate>
            </asp:TemplateField>
               <asp:TemplateField HeaderText="Imagen">
   <ItemTemplate>
     <asp:Image ID="imgFoto" runat="server"
       Width="60px" Height="60px"
       ImageUrl='<%#
         Eval("foto") == DBNull.Value
           ? ResolveUrl("~/images/no-image.png")
           : "data:image/png;base64," + Convert.ToBase64String((byte[])Eval("foto"))
       %>' />
   </ItemTemplate>
 </asp:TemplateField>
          </Columns>
        </asp:GridView>
      </div>

    </div>
  </asp:Panel>

</asp:Content>
