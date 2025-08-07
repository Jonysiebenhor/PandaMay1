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
  <!-- Validaciones -->
  <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="validator"
    HeaderText="Corrige estos errores:" ValidationGroup="prod" />
  <asp:Label ID="lblError" runat="server" CssClass="validator" Visible="false" />

  <!-- Regresar -->
  <asp:Button ID="btnRegresar" runat="server"
    CssClass="bluebutton"
    Text="Regresar"
    OnClientClick="window.history.back(); return false;" />

  <!-- ================= DATOS PRINCIPALES ================= -->
<fieldset class="section">
  <legend>Datos de Producto</legend>
  <div class="form-grid">

    <!-- Categoría maestra (sólo lectura) -->
    <div class="field-group">
      <label>Categoría maestra:</label>
      <asp:DropDownList
          ID="ddlCatMaestra"
          runat="server"
          CssClass="form-control"
          Enabled="false" />
    </div>

    <!-- Categoría (sólo lectura) -->
    <div class="field-group">
      <label>Categoría:</label>
      <asp:DropDownList
          ID="ddlCategoria"
          runat="server"
          CssClass="form-control"
          Enabled="false" />
    </div>

    <!-- Subcategoría (selección activa + opción “Agregar”) -->
    <div class="field-group">
      <label>Subcategoría:</label>
      <asp:DropDownList
          ID="ddlSubcategoria"
          runat="server"
          CssClass="form-control"
          AutoPostBack="true"
          OnSelectedIndexChanged="ddlSubcategoria_SelectedIndexChanged">
      </asp:DropDownList>
      <asp:RequiredFieldValidator
          ID="rfvSubcategoria"
          runat="server"
          ControlToValidate="ddlSubcategoria"
          InitialValue=""
          ErrorMessage="* Requerido"
          Display="Dynamic"
          ValidationGroup="prod"
          CssClass="validator" />
    </div>

  </div>
</fieldset>

<!-- Panel para crear Cat. Maestra → Cat. → Subcat. -->
<asp:Panel ID="pnlAddFullSubcat" runat="server"
    Visible="false" CssClass="section" style="margin-top:1rem;">
  <fieldset>
    <legend>Agregar nueva jerarquía</legend>
    <div class="form-grid">
      <!-- Sólo: Nombre de Maestra, Nombre de Categoría, Nombre de Subcategoría -->
      <div class="field-group">
        <label>Cat. Maestra:</label>
        <asp:TextBox ID="txtNewCatMFull" runat="server"
            CssClass="form-control" placeholder="Nombre..." />
      </div>
      <div class="field-group">
        <label>Categoría:</label>
        <asp:TextBox ID="txtNewCatFull" runat="server"
            CssClass="form-control" placeholder="Nombre..." />
      </div>
      <div class="field-group">
        <label>Subcategoría:</label>
        <asp:TextBox ID="txtNewSubFull" runat="server"
            CssClass="form-control" placeholder="Nombre..." />
      </div>
    </div>
    <div style="text-align:center; margin-top:1rem;">
      <asp:Button ID="btnAgregarSubFull" runat="server"
          CssClass="greenbutton" Text="Agregar"
           OnClick="btnAgregarSubFull_Click"
      CausesValidation="false"  
      ValidationGroup=""         
   />
    </div>
  </fieldset>
</asp:Panel>


      <!-- Tienda -->
      <div class="field-group">
        <label>Tienda:</label>
        <asp:DropDownList ID="ddlTienda" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvTienda" runat="server"
            ControlToValidate="ddlTienda" InitialValue=""
            ErrorMessage="* Requerido" Display="Dynamic" ValidationGroup="prod"
            CssClass="validator" />
      </div>

      <!-- Unidad -->
      <div class="field-group">
        <label>Unidad de medida:</label>
        <asp:DropDownList ID="ddlUnidad" runat="server" CssClass="form-control"
            AutoPostBack="true" CausesValidation="false"
            OnSelectedIndexChanged="ddlUnidad_SelectedIndexChanged" />
        <asp:RequiredFieldValidator ID="rfvUnidad" runat="server"
            ControlToValidate="ddlUnidad" InitialValue=""
            ErrorMessage="* Requerido" Display="Dynamic" ValidationGroup="prod"
            CssClass="validator" />
        <asp:Panel ID="pnlAddUnidad" runat="server" Visible="false" style="margin-top:.5rem;">
          <asp:TextBox ID="txtNewUnidad" runat="server" CssClass="form-control"
              placeholder="Nueva unidad..." />
          <asp:Button ID="btnGuardarUnidad" runat="server" CssClass="greenbutton"
              Text="Guardar unidad" OnClick="btnGuardarUnidad_Click"
              CausesValidation="false" />
        </asp:Panel>
      </div>

      <%-- Marca --%>
      <div class="field-group">
        <label>Marca:</label>
       <asp:DropDownList ID="ddlMarca" runat="server" CssClass="form-control"
    AppendDataBoundItems="true"
    AutoPostBack="true" CausesValidation="false"
    OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged">
  <%-- ítem por defecto --%>
  <asp:ListItem Text="-- Marca --" Value="" />
  <%-- opción para agregar una nueva --%>
  <asp:ListItem Text="-- Agregar nueva marca --" Value="-1" />
</asp:DropDownList>

        <asp:RequiredFieldValidator ID="rfvMarca" runat="server"
            ControlToValidate="ddlMarca" InitialValue=""
            ErrorMessage="* Requerido" Display="Dynamic" ValidationGroup="prod"
            CssClass="validator" />
        <asp:Panel ID="pnlAddMarca" runat="server" Visible="false" style="margin-top:.5rem;">
          <asp:TextBox ID="txtNewMarca" runat="server" CssClass="form-control"
              placeholder="Nueva marca..." />
          <asp:Button ID="btnGuardarMarca" runat="server" CssClass="greenbutton"
              Text="Guardar marca" OnClick="btnGuardarMarca_Click"
              CausesValidation="false" />
        </asp:Panel>
      </div>

      <!-- Nombre, Referencia, Código de Barras, Activo, Tipo, Descuento -->
      <div class="field-group">
        <label>Nombre:</label>
        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
            ControlToValidate="txtNombre" ErrorMessage="* Requerido"
            Display="Dynamic" ValidationGroup="prod" CssClass="validator" />
      </div>
      <div class="field-group">
        <label>Referencia:</label>
        <asp:TextBox ID="txtReferencia" runat="server" CssClass="form-control" />
      </div>
      <div class="field-group">
        <label>Código de barras:</label>
        <asp:TextBox ID="txtCodigoBarras" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvCB" runat="server"
            ControlToValidate="txtCodigoBarras" ErrorMessage="* Requerido"
            Display="Dynamic" ValidationGroup="prod" CssClass="validator" />
        <asp:RegularExpressionValidator ID="revCB" runat="server"
            ControlToValidate="txtCodigoBarras" ValidationExpression="^\d+$"
            ErrorMessage="* Sólo dígitos" Display="Dynamic" ValidationGroup="prod" />
      </div>
      <div class="field-group">
        <label>Activo:</label>
        <asp:CheckBox ID="chkActivo" runat="server" Checked="true" />
      </div>
      <div class="field-group">
        <label>Tipo de producto:</label>
        <asp:TextBox ID="txtTipo" runat="server" CssClass="form-control" />
      </div>

        <!-- Nuevo: Tipo de públicos -->
  <div class="field-group">
    <label>Tipo de públicos:</label>
    <asp:DropDownList ID="ddlPublico" runat="server" CssClass="form-control">
      <asp:ListItem Text="-- Seleccione público --" Value="" />
      <asp:ListItem Text="Adulto"         Value="1" />
      <asp:ListItem Text="Niñ@s"          Value="2" />
      <asp:ListItem Text="Niño"           Value="3" />
      <asp:ListItem Text="Niña"           Value="4" />
      <asp:ListItem Text="Infantes"       Value="5" />
      <asp:ListItem Text="Mayor de edad"  Value="6" />
      <asp:ListItem Text="Todos"          Value="7" />
    </asp:DropDownList>
    <asp:RequiredFieldValidator 
        ID="rfvPublico" 
        runat="server"
        ControlToValidate="ddlPublico"
        InitialValue=""
        ErrorMessage="* Requerido"
        Display="Dynamic" 
        ValidationGroup="prod" 
        CssClass="validator" />
  </div>


      <div class="field-group">
        <label>Descuento (%):</label>
        <asp:TextBox ID="txtDescuento" runat="server" CssClass="form-control" />
        <asp:RangeValidator ControlToValidate="txtDescuento"
            MinimumValue="0" MaximumValue="100" Type="Integer"
            ErrorMessage="* 0–100" Display="Dynamic"
            ValidationGroup="prod" CssClass="validator" />
      </div>
    </div>
  </fieldset>

  <!-- ================= TARIFA DINÁMICA ================= -->
  <asp:Panel ID="pnlAddTarifa" runat="server" Visible="false" CssClass="section">
    <legend>Nueva tarifa</legend>
    <asp:TextBox ID="txtTarifaNombre" runat="server" CssClass="form-control"
        Placeholder="Nombre de tarifa..." />
    <asp:TextBox ID="txtTarifaCantidad" runat="server" CssClass="form-control"
        Placeholder="Cantidad mínima..." />
    <asp:Button ID="btnGuardarTarifa" runat="server" CssClass="greenbutton"
        Text="Guardar tarifa" OnClick="btnGuardarTarifa_Click" />
  </asp:Panel>

  <fieldset class="section">
    <legend>Precios de Venta (2 o más)</legend>
    <div id="preciosContainer"></div>
    <span id="btnAddTarifa" class="add-btn">+</span>
  </fieldset>

  <!-- ================= EXISTENCIAS DINÁMICAS ================= -->
  <asp:Panel ID="pnlExistencias" runat="server" CssClass="section">
  <fieldset>
    <legend>Existencias</legend>
    <div id="existenciasContainer"></div>
    <span id="btnAddExistencia" class="add-btn">+</span>
  </fieldset>
</asp:Panel>

    <!-- ================= PRECIOS DE COMPRA ================= -->
<asp:Panel ID="pnlPreciosCompra" runat="server" CssClass="section">
  <fieldset>
    <legend>Precios de compra</legend>
    <div id="preciosCompraContainer"></div>
    <span id="btnAddPrecioCompra" class="add-btn">+</span>
  </fieldset>
</asp:Panel>

<!-- Plantilla de fila de Precio de Compra -->
<script type="text/html" id="precioCompraTpl">
  <div class="form-grid compra-row" style="border:1px solid #ddd; padding:.5rem; margin-bottom:.5rem;">
      <div class="field-group">
    <label>Proveedor:</label>
    <select name="compProveedor" class="form-control">${supplierOptions}</select>
  </div>

    <div class="field-group">
      <label>Precio:</label>
      <input type="number" step="0.01" name="compPrecio" class="form-control" />
    </div>
    <div class="field-group">
      <label>Activo:</label>
      <select name="compActivo" class="form-control">
        <option value="1">Sí</option>
        <option value="0">No</option>
      </select>
    </div>
    <div class="field-group">
      <label>Fecha:</label>
      <input type="date" name="compFecha" class="form-control" value="${hoy}" />
    </div>
  </div>
</script>


<!-- Plantilla de fila de existencia -->
<script type="text/html" id="historialTpl">
  <div class="form-grid existencia-row" style="border:1px solid #ddd;padding:.5rem;margin-bottom:.5rem;">
    <div class="field-group">
      <label>Tienda salida:</label>
      <select name="histTiendaSalida" class="form-control">${tiendaOptions}</select>
    </div>
    <div class="field-group">
      <label>Tienda recibe:</label>
      <select name="histTiendaRecibe" class="form-control">${tiendaOptions}</select>
    </div>
    <div class="field-group">
      <label>Fecha:</label>
      <input type="date" name="histFecha" class="form-control" value="${hoy}" />
    </div>
    <div class="field-group">
      <label>Cantidad:</label>
      <input type="number" name="histCantidad" class="form-control" />
    </div>
    <div class="field-group">
      <label>Referencia:</label>
      <input type="text" name="histReferencia" class="form-control" />
    </div>
    <div class="field-group">
      <label>Estado:</label>
      <select name="histEstado" class="form-control">
        <option value="Entrada">Entrada</option>
        <option value="Salida">Salida</option>
      </select>
    </div>
  </div>
</script>

  <!-- ================= IMAGEN ================= -->
  <fieldset class="section">
    <legend>Imagen</legend>
    <div class="form-grid">
      <div class="field-group">
        <label>Color imagen:</label>
        <asp:DropDownList ID="ddlImgColor" runat="server" CssClass="form-control" />
      </div>
      <div class="field-group">
        <label>Archivo:</label>
        <asp:FileUpload ID="fuImagen" runat="server" />
      </div>
      <div class="field-group">
        <label>Descripción imagen:</label>
        <asp:TextBox ID="txtImgDesc" runat="server" CssClass="form-control" />
      </div>
    </div>
  </fieldset>

  <!-- ================= GUARDAR ================= -->
  <div style="text-align:center; margin-top:2rem;">
    <asp:Button ID="btnGuardarProducto" runat="server"
      CssClass="greenbutton" Text="Guardar Producto"
      OnClick="btnGuardarProducto_Click"
      ValidationGroup="prod" />
  </div>

<!-- Plantilla de Precios -->
<script type="text/html" id="tarifaTpl">
  <div class="form-grid tarifa-row">
    <div class="field-group">
      <label>Tarifa:</label>
      <select name="tarifa" class="form-control"></select>
    </div>

    <div class="field-group">
      <label>Cant. mínima:</label>
      <input class="form-control" type="number" name="cantMin" />
    </div>
    <div class="field-group">
      <label>Precio:</label>
      <input class="form-control" type="number" step="0.01" name="precioVal" />
    </div>
  </div>
</script>

<!-- Lógica JS única para Precios y Existencias (reemplaza aquí) -->
<script type="text/javascript">
    // ——— PRECIOS ———
    function buildTarifaOptions() {
        var opts = '<option value="">-- Seleccione tarifa --</option>';
        tarifaData.forEach(function (t) {
            opts += '<option value="' + t.id + '">' + t.nombre + ' (' + t.cantidad + ')</option>';
        });
        opts += '<option value="new">➕ Agregar nueva tarifa</option>';
        return opts;
    }

    function updateTarifaFilters() {
        var selects = Array.from(document.querySelectorAll('select[name="tarifa"]'));
        var usados = selects.map(s => s.value).filter(v => v && v !== 'new');
        selects.forEach(function (s) {
            var actual = s.value;
            s.innerHTML = buildTarifaOptions();
            s.querySelectorAll('option').forEach(function (o) {
                if (usados.includes(o.value) && o.value !== actual) o.disabled = true;
            });
            s.value = actual;
        });
    }

    function onTarifaChange(e) {
        if (e.target.value === 'new') {
            document.getElementById('<%= pnlAddTarifa.ClientID %>').style.display = 'block';
        } else {
            updateTarifaFilters();
        }
    }

    function addTarifaRow() {
        var tpl = document.getElementById('tarifaTpl').innerHTML;
        var cont = document.getElementById('preciosContainer');
        var div = document.createElement('div');
        div.innerHTML = tpl;
        cont.appendChild(div);
        var sel = div.querySelector('select[name="tarifa"]');
        sel.innerHTML = buildTarifaOptions();
        sel.addEventListener('change', onTarifaChange);
        updateTarifaFilters();
    }

    // — EXISTENCIAS —  
    function addExistenciaRow() {
        var raw = document.getElementById('historialTpl').innerHTML;
        var filled = raw.replace('${medidasOptions}', medidasOptions);
        var div = document.createElement('div');
        div.innerHTML = filled;
        document.getElementById('existenciasContainer').appendChild(div);
    }

    // — PRECIOS DE COMPRA —
    function addPrecioCompraRow() {
        var tpl = document.getElementById('precioCompraTpl').innerHTML;
        var cont = document.getElementById('preciosCompraContainer');
        var div = document.createElement('div');
        div.innerHTML = tpl;
        cont.appendChild(div);
    }

    // — INICIALIZACIÓN ÚNICA —
    document.addEventListener('DOMContentLoaded', function () {
        // — Tarifas —
        var contP = document.getElementById('preciosContainer');
        contP.innerHTML = '';
        addTarifaRow();
        document.getElementById('btnAddTarifa')
            .addEventListener('click', function (e) {
                e.preventDefault();
                addTarifaRow();
            });

        // — Existencias —
        var contE = document.getElementById('existenciasContainer');
        contE.innerHTML = '';
        addExistenciaRow();
        document.getElementById('btnAddExistencia')
            .addEventListener('click', function (e) {
                e.preventDefault();
                addExistenciaRow();
            });

        // — Precios de compra —
        var contC = document.getElementById('preciosCompraContainer');
        contC.innerHTML = '';
        addPrecioCompraRow();
        document.getElementById('btnAddPrecioCompra')
            .addEventListener('click', function (e) {
                e.preventDefault();
                addPrecioCompraRow();
            });
    });
</script>


</asp:Content>
