<%@ Page 
    Title="Crear Producto"
    Language="C#"
    MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true"
    UnobtrusiveValidationMode="None"
    CodeBehind="CrearProducto.aspx.cs"
    Inherits="PandaMay.Productos.CrearProducto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
  <title>Crear Producto</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <style>
    .form-grid        { display:grid; grid-template-columns:1fr 1fr; gap:1rem; }
    .field-group      { display:flex; flex-direction:column; }
    .field-group label{ font-weight:bold; margin-bottom:.25rem; }
    .validator        { color:red; font-size:.9rem; }
    .section          { margin-bottom:2rem; }
    .section legend   { font-size:1.1rem; font-weight:bold; }
    .add-btn          { background:#444; color:#fff; border-radius:50%; 
                         width:30px; height:30px; line-height:30px; text-align:center;
                         cursor:pointer; user-select:none; }
    .bluebutton       { background:#3498db; color:#fff; padding:.5rem 1rem;
                         border:none; border-radius:4px; margin-bottom:1rem; }
    .greenbutton      { background:#2ecc71; color:#fff; padding:.5rem 1rem;
                         border:none; border-radius:4px; }
    .form-control     { padding:.4rem; border:1px solid #ccc; border-radius:4px; }
  </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
  <h2>Agregar un nuevo producto</h2>
  <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="validator"
      HeaderText="Corrige estos errores:" />

  <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false" />

  <!-- Botón Regresar -->
  <asp:Button ID="btnRegresar" runat="server" CssClass="bluebutton"
      Text="Regresar" OnClick="btnRegresar_Click" />

  <!-- Datos Principales -->
  <fieldset class="section">
    <legend>Datos de Producto</legend>
    <div class="form-grid">

      <!-- Subcategoría -->
      <div class="field-group">
        <label>Subcategoría:</label>
        <asp:DropDownList ID="ddlSubcategoria" runat="server"
            CssClass="form-control" />
      </div>

      <!-- Tienda (requerido) -->
      <div class="field-group">
        <label>Tienda:</label>
        <asp:DropDownList ID="ddlTienda" runat="server"
            CssClass="form-control" />
        <asp:RequiredFieldValidator ControlToValidate="ddlTienda"
            InitialValue="" ErrorMessage="* Requerido"
            runat="server" Display="Dynamic" CssClass="validator" />
      </div>

      <!-- Unidad + “Agregar nueva” -->
      <div class="field-group">
        <label>Unidad de medida:</label>
        <asp:DropDownList ID="ddlUnidad" runat="server"
            CssClass="form-control"
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlUnidad_SelectedIndexChanged" />
        <asp:RequiredFieldValidator ControlToValidate="ddlUnidad"
            InitialValue="" ErrorMessage="* Requerido"
            runat="server" Display="Dynamic" CssClass="validator" />
        <asp:Panel ID="pnlAddUnidad" runat="server" Visible="false"
            style="margin-top:.5rem;">
          <asp:TextBox ID="txtNewUnidad" runat="server"
              CssClass="form-control"
              placeholder="Nueva unidad..." />
          <asp:Button ID="btnGuardarUnidad" runat="server"
              CssClass="greenbutton"
              Text="Guardar unidad"
              OnClick="btnGuardarUnidad_Click" />
        </asp:Panel>
      </div>

      <!-- Marca + “Agregar nueva” -->
      <div class="field-group">
        <label>Marca:</label>
        <asp:DropDownList ID="ddlMarca" runat="server"
            CssClass="form-control"
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged" />
        <asp:RequiredFieldValidator ControlToValidate="ddlMarca"
            InitialValue="" ErrorMessage="* Requerido"
            runat="server" Display="Dynamic" CssClass="validator" />
        <asp:Panel ID="pnlAddMarca" runat="server" Visible="false"
            style="margin-top:.5rem;">
          <asp:TextBox ID="txtNewMarca" runat="server"
              CssClass="form-control"
              placeholder="Nueva marca..." />
          <asp:Button ID="btnGuardarMarca" runat="server"
              CssClass="greenbutton"
              Text="Guardar marca"
              OnClick="btnGuardarMarca_Click" />
        </asp:Panel>
      </div>

      <!-- Nombre -->
      <div class="field-group">
        <label>Nombre:</label>
        <asp:TextBox ID="txtNombre" runat="server"
            CssClass="form-control" />
        <asp:RequiredFieldValidator ControlToValidate="txtNombre"
            ErrorMessage="* Requerido"
            runat="server" Display="Dynamic" CssClass="validator" />
      </div>

      <!-- Referencia -->
      <div class="field-group">
        <label>Referencia:</label>
        <asp:TextBox ID="txtReferencia" runat="server"
            CssClass="form-control" />
      </div>

      <!-- Código de barras -->
      <div class="field-group">
        <label>Código de barras:</label>
        <asp:TextBox ID="txtCodigoBarras" runat="server"
            CssClass="form-control" />
        <asp:RequiredFieldValidator ControlToValidate="txtCodigoBarras"
            ErrorMessage="* Requerido"
            runat="server" Display="Dynamic" CssClass="validator" />
        <asp:RegularExpressionValidator ControlToValidate="txtCodigoBarras"
            ValidationExpression="^\d+$"
            ErrorMessage="* Sólo dígitos"
            runat="server" Display="Dynamic" CssClass="validator" />
      </div>

      <!-- Activo -->
      <div class="field-group">
        <label>Activo:</label>
        <asp:CheckBox ID="chkActivo" runat="server" Checked="true" />
      </div>

      <!-- Tipo de producto -->
      <div class="field-group">
        <label>Tipo de producto:</label>
        <asp:TextBox ID="txtTipo" runat="server"
            CssClass="form-control" />
      </div>

      <!-- Descuento (%) -->
      <div class="field-group">
        <label>Descuento (%):</label>
        <asp:TextBox ID="txtDescuento" runat="server"
            CssClass="form-control" />
        <asp:RangeValidator ControlToValidate="txtDescuento"
            MinimumValue="0" MaximumValue="100" Type="Integer"
            ErrorMessage="* 0–100"
            runat="server" Display="Dynamic" CssClass="validator" />
      </div>
    </div>
  </fieldset>

  <!-- Precios Dinámicos -->
  <fieldset class="section">
    <legend>Precios de Venta (2 o más)</legend>
    <div id="preciosContainer"></div>
    <span class="add-btn" onclick="addTarifaRow();return false;">+</span>
  </fieldset>

  <!-- Existencias Dinámicas -->
  <fieldset class="section">
    <legend>Existencias Iniciales</legend>
    <div id="existenciasContainer"></div>
    <span class="add-btn" onclick="addExistenciaRow();return false;">+</span>
  </fieldset>

  <!-- Imagen -->
  <fieldset class="section">
    <legend>Imagen</legend>
    <div class="form-grid">
      <div class="field-group">
        <label>Color imagen:</label>
        <asp:DropDownList ID="ddlImgColor" runat="server"
            CssClass="form-control" />
      </div>
      <div class="field-group">
        <label>Archivo:</label>
        <asp:FileUpload ID="fuImagen" runat="server" />
      </div>
      <div class="field-group">
        <label>Descripción imagen:</label>
        <asp:TextBox ID="txtImgDesc" runat="server"
            CssClass="form-control" />
      </div>
    </div>
  </fieldset>

  <!-- Guardar -->
  <div style="text-align:center; margin-top:2rem;">
    <asp:Button ID="btnGuardarProducto" runat="server"
        CssClass="greenbutton" Text="Guardar Producto"
        OnClick="btnGuardarProducto_Click" />
  </div>

  <!-- Plantillas JS -->
  <script type="text/html" id="tarifaTpl">
    <div class="form-grid tarifa-row">
      <div class="field-group">
        <label>Tarifa:</label>
        <select name="tarifa" class="form-control">
          <option value="unidad">Precio unidad</option>
          <option value="3omas">Precio 3 o más</option>
          <option value="docena">Precio docena</option>
          <option value="fardo">Precio fardo</option>
        </select>
      </div>
      <div class="field-group">
        <label>Aplica en:</label><input class="form-control" type="text" name="aplicaEn" />
      </div>
      <div class="field-group">
        <label>Cant. mínima:</label><input class="form-control" type="number" name="cantMin" />
      </div>
      <div class="field-group">
        <label>Precio:</label><input class="form-control" type="number" step="0.01" name="precioVal" />
      </div>
    </div>
  </script>

  <script type="text/html" id="existenciaTpl">
    <div class="form-grid existencia-row">
      <div class="field-group">
        <label>Color:</label><input class="form-control" type="text" name="exColor" />
      </div>
      <div class="field-group">
        <label>Medida:</label><input class="form-control" type="text" name="exMedida" />
      </div>
      <div class="field-group">
        <label>Cantidad:</label><input class="form-control" type="number" name="exCant" />
      </div>
    </div>
  </script>

  <script>
      function addTarifaRow() {
          var tpl = document.getElementById('tarifaTpl').innerHTML;
          document.getElementById('preciosContainer')
              .insertAdjacentHTML('beforeend', tpl);
      }
      function addExistenciaRow() {
          var tpl = document.getElementById('existenciaTpl').innerHTML;
          document.getElementById('existenciasContainer')
              .insertAdjacentHTML('beforeend', tpl);
      }
  </script>
</asp:Content>
