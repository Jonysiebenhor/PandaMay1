<%@ Page Title="Productos" Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="Productos.aspx.cs"
    Inherits="PandaMay.Productos.Productos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
  <title>Productos</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <!-- Aquí puedes añadir CSS adicional si lo deseas -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">

  <!-- 1) Solo botón “Crear Producto” al entrar -->
  <div style="text-align:center; margin:30px 0;">
    <asp:Button
      ID="btnCrearProducto"
      runat="server"
      CssClass="greenbutton"
      Text="Crear Producto"
      OnClick="btnCrearProducto_Click" />
  </div>

  <!-- 2) Panel con buscador y disposiciones 35%/65% -->
  <asp:Panel ID="pnlTabla" runat="server" Visible="false">

    <!-- Encabezado: botón Regresar + buscador -->
     <div style="display:flex; justify-content:center; align-items:center; gap:10px; margin-bottom:20px;">
      <asp:Button
        ID="btnRegresar"
        runat="server"
        CssClass="orangebutton"
        Text="Regresar"
        OnClick="btnRegresar_Click" />

        <!-- Nuevo botón Crear Producto (abre en pestaña nueva) -->
     <asp:Button
  ID="btnAbrirCrearProducto"
  runat="server"
  UseSubmitBehavior="false"
  CssClass="greenbutton"
  Text="Crear Producto"
  OnClientClick="window.open('CrearProducto.aspx', '_blank'); return false;" />


      <asp:TextBox
        ID="txtBuscar"
        runat="server"
        CssClass="buscador"
        placeholder="Buscar Productos..."
        AutoPostBack="True"
        OnTextChanged="txtBuscar_TextChanged" />
    </div>

    <!-- Contenedor flex para detalle (35%) y lista (65%) -->
    <div style="display:flex; width:100%;">

      <!-- IZQUIERDA: Detalles (35%) -->
      <div style="width:35%; padding-right:20px;">

        <asp:Panel ID="pnlDetalles" runat="server" Visible="false">
          <h4>Detalles completos</h4>

          <!-- Cada tabla de detalle dentro de un div scrollable -->
          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvCombosProductos"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvDetallesCompras"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvDetallesTraslados"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvDetallesVentas"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvPrecios"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvPreciosCompras"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto; margin-bottom:20px;">
            <asp:GridView
              ID="gvAtributos"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

          <div style="overflow-x:auto;">
            <asp:GridView
              ID="gvExistencias"
              runat="server"
              AutoGenerateColumns="true"
              GridLines="Both"
              BorderColor="#ccc"
              BorderWidth="1px"
              HeaderStyle-BorderColor="#ccc"
              HeaderStyle-BorderWidth="1px"
              RowStyle-BorderColor="#ccc"
              RowStyle-BorderWidth="1px"
              Width="100%" />
          </div>

        </asp:Panel>

      </div>

      <!-- DERECHA: Lista de productos (65%) -->
      <div style="width:65%; margin-left:auto; overflow-x:auto;">

        <asp:GridView
          ID="GridView1"
          runat="server"
          DataKeyNames="idproducto"
          ForeColor="Black"
          Width="100%"
          CellSpacing="5"
          HorizontalAlign="Center"
          AutoGenerateColumns="false"
          AutoGenerateSelectButton="True"
          OnSelectedIndexChanging="Select1"
          GridLines="Both"
          BorderColor="#ccc"
          BorderWidth="1px">

          <SelectedRowStyle BackColor="#80cdbb" Font-Bold="true" />
          <HeaderStyle BackColor="Black" Font-Bold="True" Font-Italic="False" ForeColor="White" />
          <SelectedRowStyle BorderStyle="Solid" />

          <Columns>
            <asp:TemplateField HeaderText="Producto">
              <ItemTemplate>
                <asp:Label ID="Label9" runat="server" Text='<%# Bind("nombre") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio(U)">
              <ItemTemplate>
                <asp:Label ID="Label10" runat="server" Text='<%# Bind("unidad") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio(3)">
              <ItemTemplate>
                <asp:Label ID="Label11" runat="server" Text='<%# Bind("tresomas") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio(Doc)">
              <ItemTemplate>
                <asp:Label ID="Label12" runat="server" Text='<%# Bind("docena") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Precio(F)">
              <ItemTemplate>
                <asp:Label ID="Label13" runat="server" Text='<%# Bind("fardo") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Código de barras">
              <ItemTemplate>
                <asp:Label ID="Label14" runat="server" Text='<%# Bind("codigodebarras") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Referencia">
              <ItemTemplate>
                <asp:Label ID="Label15" runat="server" Text='<%# Bind("referencia") %>' />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Imagen">
              <ItemTemplate>
                <asp:Label ID="Label16" runat="server" Text='<%# Bind("foto") %>' />
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
        </asp:GridView>

      </div>

    </div>
  </asp:Panel>

</asp:Content>
