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
      .invalid { border-color:#e74c3c !important; box-shadow:0 0 0 2px rgba(231,76,60,.15); }
  .field-error { color:#e74c3c; font-size:.9rem; margin-top:.25rem; }
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
    select.dropdownlist[multiple] option:checked{
    background:#80cdbb !important; color:#fff !important;
  }
  </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <asp:ScriptManager ID="smMain" runat="server" EnablePageMethods="true" />
  <h2>Agregar un nuevo producto</h2>
  <!-- Validaciones -->
  <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="validator"
    HeaderText="Corrige estos errores:" ValidationGroup="prod" />
  <asp:Label ID="lblError" runat="server" CssClass="validator" Visible="false" />

  <!-- Regresar -->
  <asp:Button
    ID="btnRegresar"
    runat="server"
    CssClass="orangebutton"
    Text="← Regresar"
    CausesValidation="false"
    UseSubmitBehavior="false"
    OnClientClick="location.href='Productos.aspx'; return false;" />


  <!-- ================= DATOS PRINCIPALES ================= -->
<fieldset class="section">
  <legend>Datos de Producto</legend>
  <div class="form-grid">

   
<div class="field-group">
  <label>Categorías (asociadas a la subcategoría):</label>
  <asp:ListBox ID="lstCategoriasRO" runat="server"
               CssClass="dropdownlist" SelectionMode="Multiple"
               Enabled="false" Rows="3" />
</div>

<div class="field-group">
  <label>Categoría(s) maestra(s):</label>
  <asp:ListBox ID="lstMaestrasRO" runat="server"
               CssClass="dropdownlist" SelectionMode="Multiple"
               Enabled="false" Rows="2" />
</div>

   <!-- Subcategoría (selección activa + opción “Agregar”) -->
<div class="field-group">
  <label>Subcategoría:</label>
  <asp:DropDownList
      ID="ddlSubcategoria"
      runat="server"
      CssClass="dropdownlist"
      AutoPostBack="true"
      OnSelectedIndexChanged="ddlSubcategoria_SelectedIndexChanged">
    <asp:ListItem Text="-- Seleccione subcategoría --" Value="" />
    <asp:ListItem Text="+ Agregar subcategoría" Value="new" />
    <%-- Items cargados en CargarSubcategorias() --%>
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

<!-- Panel para nueva Subcategoría -->
<asp:Panel ID="pnlNewSub" runat="server" Visible="false" CssClass="section">
  <div class="form-grid">
    <!-- Columna 1: Input de Subcategoría -->
    <div class="field-group">
      <label>Ingrese Subcategoría:</label>
       <asp:TextBox ID="txtNewSub" runat="server" CssClass="textbox" />
       <asp:RequiredFieldValidator
           ID="rfvNewSub"
           runat="server"
           ControlToValidate="txtNewSub"
          ErrorMessage="* Requerido"
          Display="Dynamic"
          ValidationGroup="subcat"
           CssClass="validator" />
    </div>


    <!-- Columna 2: Selección múltiple de Categorías -->
    <div class="field-group">
      <label>Seleccione Categorías asociadas:</label>
     <asp:ListBox ID="lstCategorias" runat="server" CssClass="dropdownlist"
           SelectionMode="Multiple" AutoPostBack="true"
           OnSelectedIndexChanged="lstCategorias_SelectedIndexChanged"
           Rows="6">
        <%-- Items cargados en CargarCategoriasListBox() --%>
      </asp:ListBox>
      <asp:RequiredFieldValidator
           ID="rfvLstCat"
           runat="server"
           ControlToValidate="lstCategorias"
           InitialValue=""
           ErrorMessage="* Seleccione al menos una"
           Display="Dynamic"
           ValidationGroup="subcat"
           CssClass="validator" />
    </div>
  </div>

  <!-- Botón centrado -->
  <div style="text-align:center; margin-top:1rem;">
    <asp:Button ID="btnAddSub" runat="server" CssClass="greenbutton"
         Text="Guardar Subcategoría"
         OnClick="btnAddSub_Click"
         ValidationGroup="subcat"
         CausesValidation="true" />
  </div>
</asp:Panel>

<!-- Panel para nueva Categoría -->
<asp:Panel ID="pnlNewCat" runat="server" Visible="false" CssClass="section">
  <div class="form-grid">
    <!-- Input de nueva Categoría -->
    <div class="field-group">
      <label>Ingrese Categoría:</label>
      <asp:TextBox
          ID="txtNewCat"
          runat="server"
          CssClass="textbox" />
      <asp:RequiredFieldValidator
          ID="rfvNewCat"
          runat="server"
          ControlToValidate="txtNewCat"
          ErrorMessage="* Requerido"
          Display="Dynamic"
          ValidationGroup="cat"
          CssClass="validator" />
    </div>

    <!-- Dropdown de Maestra -->
    <div class="field-group">
      <label>Seleccione Categoría Maestra:</label>
      <asp:DropDownList
          ID="ddlCatMaestra2"
          runat="server"
          CssClass="dropdownlist"
         AutoPostBack="true"
           OnSelectedIndexChanged="ddlCatMaestra2_SelectedIndexChanged"
           ValidationGroup="cat">
        <asp:ListItem Text="-- Seleccione maestra --" Value="" />
        <asp:ListItem Text="+ Agregar maestra" Value="new" />
        <%-- Items cargados en CargarCatMaestra2() --%>
      </asp:DropDownList>
    </div>
  </div>

  <!-- Botón centrado -->
  <div style="text-align:center; margin-top:1rem;">
    <asp:Button
        ID="btnAddCat"
        runat="server"
        CssClass="greenbutton"
        Text="Guardar Categoría"
        OnClick="btnAddCat_Click"
        ValidationGroup="cat" />
  </div>
</asp:Panel>

<!-- Panel para nueva Categoría Maestra -->
<asp:Panel ID="pnlNewCatM" runat="server" Visible="false" CssClass="section"  ValidationGroup="catm">
  <div class="form-grid">
    <div class="field-group">
      <label>Ingrese Categoría Maestra:</label>
      <asp:TextBox
          ID="txtNewCatM"
          runat="server"
          CssClass="textbox" />
      <asp:RequiredFieldValidator
          ID="rfvNewCatM"
          runat="server"
          ControlToValidate="txtNewCatM"
          ErrorMessage="* Requerido"
          Display="Dynamic"
          ValidationGroup="catm"
          CssClass="validator" />
    </div>
  </div>

  <!-- Botón centrado -->
  <div style="text-align:center; margin-top:1rem;">
    <asp:Button
        ID="btnAddCatM"
        runat="server"
        CssClass="greenbutton"
        Text="Guardar Maestra"
        OnClick="btnAddCatM_Click"
        ValidationGroup="catm"
        CausesValidation="true" />
  </div>
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
  <asp:ListItem Text="-- Agregar nueva marca --" Value="0" />
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

<script type="text/html" id="precioCompraTpl">
  <div class="form-grid compra-row" style="border:1px solid #ddd; padding:.5rem; margin-bottom:.5rem;">
    <!-- Proveedor -->
    <div class="field-group">
      <label>Proveedor:</label>
      <select name="compProveedor" class="dropdownlist proveedor-select">
        <option value="">-- Proveedores --</option>
        <option value="new">+ Agregar proveedor</option>
        ${supplierOptions}
      </select>

      <!-- Mini-form de alta de proveedor (oculto por defecto) -->
      <div class="add-proveedor" style="display:none; margin-top:.5rem;">
        <input type="text" class="textbox nuevo-prov-nombre" placeholder="Nombre del proveedor..." />
        <button type="button" class="greenbutton btn-guardar-prov">Guardar</button>
        <button type="button" class="orangebutton btn-cancelar-prov">Cancelar</button>
      </div>
    </div>

    <!-- Precio -->
    <div class="field-group">
      <label>Precio:</label>
      <input type="number" step="0.01" name="compPrecio" class="textbox" />
    </div>

    <!-- Activo -->
    <div class="field-group">
      <label>Activo:</label>
      <select name="compActivo" class="dropdownlist">
        <option value="1">Sí</option>
        <option value="0">No</option>
      </select>
    </div>

    <!-- Fecha -->
    <div class="field-group">
      <label>Fecha:</label>
      <input type="date" name="compFecha" class="textbox" value="${hoy}" />
    </div>
  </div>
</script>



    <!-- Plantilla de fila de Existencia: sólo Medida + Cantidad -->
<script type="text/html" id="historialTpl">
  <div class="form-grid existencia-row" style="border:1px solid #ddd;padding:.5rem;margin-bottom:.5rem;">
    <div class="field-group">
      <label>Medida:</label>
      <select name="histMedida" class="form-control">
        <option value="">-- Medida --</option>
        ${medidasOptions}
      </select>
    </div>
    <div class="field-group">
      <label>Cantidad:</label>
      <input type="number" name="histCantidad" class="form-control" />
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
  ValidationGroup="prod"
  OnClientClick="return validarAntesDeGuardar();" />
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

<!-- Lógica JS única para Precios y Existencias -->
<script type="text/javascript">
    // ——— PRECIOS ———
    function buildTarifaOptions() {
        var opts = '<option value="">-- Seleccione tarifa --</option>';
        tarifaData.forEach(function (t) {
            opts += '<option value="' + t.id + '">' + t.nombre + ' (' + t.cantidad + ')</option>';
        });
        return opts; // ← ya no agregamos la opción "new"
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
        updateTarifaFilters(); // solo recalcula deshabilitados; no muestra panel
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
        // 1) tomo la plantilla
        var raw = document.getElementById('historialTpl').innerHTML;
        // 2) substituyo el placeholder por la variable JS medidasOptions
        var filled = raw.replace('${medidasOptions}', medidasOptions);
        // 3) lo convierto en nodo y lo añado al contenedor
        var div = document.createElement('div');
        div.innerHTML = filled;
        document.getElementById('existenciasContainer').appendChild(div);
    }



    // — PRECIOS DE COMPRA —
    function addPrecioCompraRow() {
        var tpl = document.getElementById('precioCompraTpl').innerHTML;
        // ¡Importante! Inyectamos supplierOptions y hoy en el HTML
        var filled = tpl
            .replace('${supplierOptions}', (typeof supplierOptions !== 'undefined' ? supplierOptions : ''))
            .replace('${hoy}', (typeof hoy !== 'undefined' ? hoy : ''));

        var cont = document.getElementById('preciosCompraContainer');
        var div = document.createElement('div');
        div.innerHTML = filled;
        cont.appendChild(div);

        // Conectamos eventos del proveedor en esta fila
        wireProveedorRow(div);
    }

  
    
/* ==================== PROVEEDORES: wire + alta en vivo ==================== */

        function wireProveedorRow(row) {
  var sel        = row.querySelector('.proveedor-select');
        var panel      = row.querySelector('.add-proveedor');
        var txt        = row.querySelector('.nuevo-prov-nombre');
        var btnGuardar = row.querySelector('.btn-guardar-prov');
        var btnCancel  = row.querySelector('.btn-cancelar-prov');

        if (!sel) return;

        // Mostrar el mini-form cuando eligen "+ Agregar proveedor"
        sel.addEventListener('change', function () {
    if (sel.value === 'new') {
            panel.style.display = 'block';
        if (txt) txt.focus();
    } else {
            panel.style.display = 'none';
    }
  });

        // Guardar nuevo proveedor
        if (btnGuardar) {
            btnGuardar.addEventListener('click', function () {
                guardarNuevoProveedor(row);
            });
  }

        // Permitir Enter dentro del textbox para guardar
        if (txt) {
            txt.addEventListener('keydown', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    if (btnGuardar) btnGuardar.click();
                }
            });
  }

        // Cancelar alta rápida
        if (btnCancel) {
            btnCancel.addEventListener('click', function () {
                panel.style.display = 'none';
                sel.value = ''; // obligamos a volver a elegir
            });
  }
}

        // Llama al WebMethod AddProveedor y actualiza TODOS los selects sin recargar
        function guardarNuevoProveedor(row) {
  var txt        = row.querySelector('.nuevo-prov-nombre');
        var sel        = row.querySelector('.proveedor-select');
        var panel      = row.querySelector('.add-proveedor');
        var btnGuardar = row.querySelector('.btn-guardar-prov');

        if (!txt || !sel) return;

        var nombre = (txt.value || '').trim();
        if (!nombre) {
            alert('Escribe el nombre del proveedor.');
        txt.focus();
        return;
  }

        // Evitar doble click
        if (btnGuardar) {
    if (btnGuardar.disabled) return;
        btnGuardar.disabled = true;
  }

  // Requiere: <asp: ScriptManager EnablePageMethods="true" />
        PageMethods.AddProveedor(nombre,
        function (result) {
      // success
      if (!result || !result.id) {
            alert('No se recibió un ID válido del servidor.');
        if (btnGuardar) btnGuardar.disabled = false;
        return;
      }

      // Insertar en el <select> actual justo después de "new"
            var opt = document.createElement('option');
            opt.value = result.id;
            opt.textContent = result.nombre;

      if (sel.options.length > 0) {
                sel.insertBefore(opt, sel.options[1] || null); // después de "new"
      } else {
                sel.appendChild(opt);
      }
            sel.value = result.id;

            // Propagar a TODOS los combos de proveedor de la página
            document.querySelectorAll('.proveedor-select').forEach(function (otro) {
        if ([...otro.options].some(o => o.value === String(result.id))) return; // ya existe
            var o2 = document.createElement('option');
            o2.value = result.id;
            o2.textContent = result.nombre;

        if (otro.options.length > 0) {
                otro.insertBefore(o2, otro.options[1] || null);
        } else {
                otro.appendChild(o2);
        }
      });

            // Actualizar la variable global supplierOptions para futuras filas
            if (typeof supplierOptions !== 'undefined') {
                supplierOptions = `<option value="${result.id}">${result.nombre}</option>` + supplierOptions;
      }

            // Cerrar mini-form y limpiar
            panel.style.display = 'none';
            txt.value = '';

            if (btnGuardar) btnGuardar.disabled = false;
    },
            function (err) {
      // error
      var msg = (err && err.get_message) ? err.get_message() : 'Error al crear proveedor';
            alert(msg);
            if (btnGuardar) btnGuardar.disabled = false;
    }
            );
}

            /* ==================== INICIALIZACIÓN ==================== */
            document.addEventListener('DOMContentLoaded', function () {
  // — Tarifas —
  try {
    var contP = document.getElementById('preciosContainer');
            if (contP) {
                contP.innerHTML = '';
            if (typeof addTarifaRow === 'function') {
                addTarifaRow();
            var btnAddTarifa = document.getElementById('btnAddTarifa');
            if (btnAddTarifa) {
                btnAddTarifa.addEventListener('click', function (e) {
                    e.preventDefault();
                    addTarifaRow();
                });
        }
      }
    }
  } catch (e) { /* noop */}

  // — Existencias —
            try {
    var contE = document.getElementById('existenciasContainer');
            if (contE) {
                contE.innerHTML = '';
            if (typeof addExistenciaRow === 'function') {
                addExistenciaRow();
            var btnAddExistencia = document.getElementById('btnAddExistencia');
            if (btnAddExistencia) {
                btnAddExistencia.addEventListener('click', function (e) {
                    e.preventDefault();
                    addExistenciaRow();
                });
        }
      }
    }
  } catch (e) { /* noop */}

  // — Precios de compra —
            try {
    var contC = document.getElementById('preciosCompraContainer');
            if (contC) {
                contC.innerHTML = '';
            if (typeof addPrecioCompraRow === 'function') {
                // addPrecioCompraRow debe llamar internamente a wireProveedorRow(row)
                addPrecioCompraRow();
            var btnAddPrecioCompra = document.getElementById('btnAddPrecioCompra');
            if (btnAddPrecioCompra) {
                btnAddPrecioCompra.addEventListener('click', function (e) {
                    e.preventDefault();
                    addPrecioCompraRow(); // idem
                });
        }
      }
    }
  } catch (e) { /* noop */}
});
</script>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var lb = document.getElementById('<%= lstCategorias.ClientID %>');
    if (!lb) return;

    lb.addEventListener('mousedown', function (e) {
        var opt = e.target;
        if (opt && opt.tagName === 'OPTION') {
            if (opt.value === 'new') return; // aquí sí quieres el postback para abrir el panel
            e.preventDefault();              // evita el cambio “normal” (y el postback)
            opt.selected = !opt.selected;    // togglear sin Ctrl
            
        }
    });
});
    </script>


    <script>
        (function () {
            function addError(el, msg) {
                if (!el) return;
                el.classList.add('invalid');
                var fg = el.closest('.field-group') || el.parentElement;
                if (fg) {
                    var err = fg.querySelector('.field-error');
                    if (!err) {
                        err = document.createElement('div');
                        err.className = 'field-error';
                        fg.appendChild(err);
                    }
                    err.textContent = msg || 'Campo requerido';
                }
            }

            function clearErrors(scope) {
                (scope || document).querySelectorAll('.invalid').forEach(n => n.classList.remove('invalid'));
                (scope || document).querySelectorAll('.field-error').forEach(n => n.remove());
            }

            function attachAutoClear() {
                document.querySelectorAll('input, select, textarea').forEach(function (el) {
                    el.addEventListener('input', function () {
                        el.classList.remove('invalid');
                        var fe = (el.closest('.field-group') || el.parentElement).querySelector('.field-error'); if (fe) fe.remove();
                    });
                    el.addEventListener('change', function () {
                        el.classList.remove('invalid');
                        var fe = (el.closest('.field-group') || el.parentElement).querySelector('.field-error'); if (fe) fe.remove();
                    });
                });
            }
            attachAutoClear();

            window.validarAntesDeGuardar = function () {
                clearErrors();
                var ok = true, firstBad = null;

                // ---- Controles principales (server) ----
                var ddlUnidad = document.getElementById('<%= ddlUnidad.ClientID %>');
      var ddlMarca = document.getElementById('<%= ddlMarca.ClientID %>');
      var ddlTienda = document.getElementById('<%= ddlTienda.ClientID %>');
      var ddlSubcat = document.getElementById('<%= ddlSubcategoria.ClientID %>');
    var ddlPublico    = document.getElementById('<%= ddlPublico.ClientID %>');
    var txtNombre     = document.getElementById('<%= txtNombre.ClientID %>');
    var txtCB         = document.getElementById('<%= txtCodigoBarras.ClientID %>');

                function need(el, msg) {
                    if (!el || !el.value) { ok = false; addError(el, msg); if (!firstBad) firstBad = el; }
                }
                need(txtNombre, 'Ingrese el nombre');
                need(txtCB, 'Ingrese código de barras');
                need(ddlUnidad, 'Seleccione una unidad');
                need(ddlMarca, 'Seleccione una marca');
                need(ddlTienda, 'Seleccione una tienda');
                need(ddlSubcat, 'Seleccione subcategoría');
                need(ddlPublico, 'Seleccione un tipo de público');

                // ---- Existencias dinámicas ----
                var exRows = document.querySelectorAll('#existenciasContainer .existencia-row');
                if (exRows.length === 0) {
                    ok = false;
                    // Marca el contenedor si no hay filas
                    var contE = document.querySelector('#existenciasContainer');
                    addError(contE, 'Agregue al menos una existencia');
                    if (!firstBad) firstBad = contE;
                } else {
                    exRows.forEach(function (r) {
                        var med = r.querySelector('select[name="histMedida"]');
                        var cant = r.querySelector('input[name="histCantidad"]');
                        if (!med || !med.value) { ok = false; addError(med, 'Seleccione medida'); if (!firstBad) firstBad = med; }
                        if (!cant || +cant.value <= 0) { ok = false; addError(cant, 'Cantidad > 0'); if (!firstBad) firstBad = cant; }
                    });
                }

                // ---- Tarifas dinámicas (2 o más) ----
                var tRows = document.querySelectorAll('#preciosContainer .tarifa-row');
                if (tRows.length < 2) {
                    ok = false;
                    var contT = document.querySelector('#preciosContainer');
                    addError(contT, 'Agregue al menos 2 tarifas');
                    if (!firstBad) firstBad = contT;
                }
                tRows.forEach(function (r) {
                    var sel = r.querySelector('select[name="tarifa"]');
                    var p = r.querySelector('input[name="precioVal"]');
                    if (!sel || !sel.value) { ok = false; addError(sel, 'Seleccione tarifa'); if (!firstBad) firstBad = sel; }
                    if (!p || +p.value <= 0) { ok = false; addError(p, 'Precio > 0'); if (!firstBad) firstBad = p; }
                });

                // ---- Precios de compra (si hay fila, validar precio) ----
                var cRows = document.querySelectorAll('#preciosCompraContainer .compra-row');
                cRows.forEach(function (r) {
                    var precio = r.querySelector('input[name="compPrecio"]');
                    if (precio && precio.value !== '' && +precio.value <= 0) {
                        ok = false; addError(precio, 'Precio > 0'); if (!firstBad) firstBad = precio;
                    }
                });

                if (!ok && firstBad) {
                    // Llevar la vista al primer error
                    firstBad.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    firstBad.focus({ preventScroll: true });
                }

                return ok; // true => permite postback, false => lo cancela (se conserva la imagen/filas)
            };
        })();
    </script>



</asp:Content>
