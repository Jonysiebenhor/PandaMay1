<%@ Page 
    Title="Crear Producto" 
    Language="C#" 
    MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" 
    CodeBehind="CrearProducto.aspx.cs"
    Inherits="PandaMay.Productos.CrearProducto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
  <title>Crear Producto</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
  <h2>Alta de Producto</h2>

  <!-- Label para errores -->
  <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false" />

  <asp:Panel ID="pnlForm" runat="server">
    <!-- DATOS PRINCIPALES -->
    <fieldset>
      <legend>Datos de Producto</legend>
      <table>
        <tr>
          <td>Subcategoría:</td>
          <td><asp:DropDownList ID="ddlSubcategoria" runat="server" /></td>
        </tr>
        <tr>
          <td>Unidad de medida:</td>
          <td><asp:DropDownList ID="ddlUnidad" runat="server" /></td>
        </tr>
        <tr>
          <td>Marca:</td>
          <td><asp:DropDownList ID="ddlMarca" runat="server" /></td>
        </tr>
        <tr>
          <td>Nombre:</td>
          <td><asp:TextBox ID="txtNombre" runat="server" /></td>
        </tr>
        <tr>
          <td>Fecha creación:</td>
          <td><asp:TextBox ID="txtFecha" runat="server" TextMode="Date" /></td>
        </tr>
        <tr>
          <td>Referencia:</td>
          <td><asp:TextBox ID="txtReferencia" runat="server" /></td>
        </tr>
        <tr>
          <td>Código de barras:</td>
          <td><asp:TextBox ID="txtCodigoBarras" runat="server" /></td>
        </tr>
        <tr>
          <td>Activo:</td>
          <td><asp:CheckBox ID="chkActivo" runat="server" Checked="true" /></td>
        </tr>
        <tr>
          <td>Tipo de producto:</td>
          <td><asp:TextBox ID="txtTipo" runat="server" /></td>
        </tr>
        <tr>
          <td>Descuento (%):</td>
          <td><asp:TextBox ID="txtDescuento" runat="server" /></td>
        </tr>
      </table>
    </fieldset>

    <!-- PRECIOS DE VENTA -->
    <fieldset>
      <legend>Precios de Venta</legend>
      <table>
        <tr>
          <td>Precio unidad:</td>
          <td><asp:TextBox ID="txtPrecioUnidad" runat="server" /></td>
        </tr>
        <tr>
          <td>Precio “3 o más”:</td>
          <td><asp:TextBox ID="txtPrecioTres" runat="server" /></td>
        </tr>
        <tr>
          <td>Precio docena:</td>
          <td><asp:TextBox ID="txtPrecioDocena" runat="server" /></td>
        </tr>
        <tr>
          <td>Precio fardo:</td>
          <td><asp:TextBox ID="txtPrecioFardo" runat="server" /></td>
        </tr>
      </table>
    </fieldset>

    <!-- PRECIO COMPRA -->
    <fieldset>
      <legend>Precio de Compra</legend>
      <table>
        <tr>
          <td>Descripción compra:</td>
          <td><asp:TextBox ID="txtDescCompra" runat="server" /></td>
        </tr>
        <tr>
          <td>Precio compra:</td>
          <td><asp:TextBox ID="txtPrecioCompra" runat="server" /></td>
        </tr>
      </table>
    </fieldset>

    <!-- EXISTENCIAS -->
    <fieldset>
      <legend>Existencias Iniciales</legend>
      <table>
        <tr>
          <td>Tienda:</td>
          <td><asp:DropDownList ID="ddlTienda" runat="server" /></td>
        </tr>
        <tr>
          <td>Color:</td>
          <td>
            <asp:DropDownList 
              ID="ddlColor" 
              runat="server" 
              AutoPostBack="true"
              OnSelectedIndexChanged="ddlColor_SelectedIndexChanged" />
            <asp:Panel 
              ID="pnlColorOtro" 
              runat="server" 
              Visible="false" 
              Style="margin-top:5px;">
              <asp:Label 
                ID="lblColorOtro" 
                runat="server" 
                Text="Color (personalizado):" 
                AssociatedControlID="txtColorOtro" />
              <asp:TextBox 
                ID="txtColorOtro" 
                runat="server" />
            </asp:Panel>
          </td>
        </tr>
        <tr>
          <td>Medida:</td>
          <td><asp:DropDownList ID="ddlMedida" runat="server" /></td>
        </tr>
        <tr>
          <td>Cantidad:</td>
          <td><asp:TextBox ID="txtExistencia" runat="server" /></td>
        </tr>
      </table>
    </fieldset>

    <!-- ATRIBUTOS -->
    <fieldset>
      <legend>Atributo (opcional)</legend>
      <table>
        <tr>
          <td>Nombre atributo:</td>
          <td><asp:TextBox ID="txtAtrNombre" runat="server" /></td>
        </tr>
        <tr>
          <td>Descripción atributo:</td>
          <td>
            <asp:TextBox 
              ID="txtAtrDesc" 
              runat="server" 
              TextMode="MultiLine" 
              Rows="2" />
          </td>
        </tr>
      </table>
    </fieldset>

    <!-- IMÁGENES -->
    <fieldset>
      <legend>Imagen</legend>
      <table>
        <tr>
          <td>Color:</td>
          <td><asp:DropDownList ID="ddlImgColor" runat="server" /></td>
        </tr>
        <tr>
          <td>Archivo:</td>
          <td><asp:FileUpload ID="fuImagen" runat="server" /></td>
        </tr>
        <tr>
          <td>Descripción imagen:</td>
          <td><asp:TextBox ID="txtImgDesc" runat="server" /></td>
        </tr>
      </table>
    </fieldset>

    <!-- BOTÓN GUARDAR -->
    <div style="text-align:center; margin-top:15px;">
      <asp:Button 
        ID="btnGuardarProducto" 
        runat="server" 
        CssClass="greenbutton" 
        Text="Guardar Producto" 
        OnClick="btnGuardarProducto_Click" />
    </div>
  </asp:Panel>
</asp:Content>

