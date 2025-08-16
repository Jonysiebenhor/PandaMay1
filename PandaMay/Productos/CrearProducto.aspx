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

    /* ===== Borde negro para secciones ===== */
    .section-box { border:2px solid #000; padding:1rem; border-radius:6px; }
    .section-box legend { padding:0 .5rem; font-weight:700; }
    /* Evita doble borde cuando hay fieldsets anidados dentro de Paneles ASP.NET */
    .section-box fieldset { border:0; padding:0; margin:0; }
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

  <!-- ========================================================= -->
  <!-- 1) CATEGORÍAS (maestra, categoría, subcategoría)          -->
  <!-- ========================================================= -->
  <fieldset class="section section-box">
    <legend>Categorías</legend>

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

    <!-- Panel nueva Subcategoría -->
    <asp:Panel ID="pnlNewSub" runat="server" Visible="false" CssClass="section">
      <div class="form-grid">
        <div class="field-group">
          <label>Ingrese Subcategoría:</label>
          <asp:TextBox ID="txtNewSub" runat="server" CssClass="textbox" />
          <asp:RequiredFieldValidator ID="rfvNewSub" runat="server"
              ControlToValidate="txtNewSub" ErrorMessage="* Requerido"
              Display="Dynamic" ValidationGroup="subcat" CssClass="validator" />
        </div>

        <div class="field-group">
          <label>Seleccione Categorías asociadas:</label>
          <asp:ListBox ID="lstCategorias" runat="server" CssClass="dropdownlist"
              SelectionMode="Multiple" AutoPostBack="true"
              OnSelectedIndexChanged="lstCategorias_SelectedIndexChanged"
              Rows="6" />
          <asp:RequiredFieldValidator ID="rfvLstCat" runat="server"
              ControlToValidate="lstCategorias" InitialValue=""
              ErrorMessage="* Seleccione al menos una" Display="Dynamic"
              ValidationGroup="subcat" CssClass="validator" />
        </div>
      </div>

      <div style="text-align:center; margin-top:1rem;">
        <asp:Button ID="btnAddSub" runat="server" CssClass="greenbutton"
             Text="Guardar Subcategoría" OnClick="btnAddSub_Click"
             ValidationGroup="subcat" CausesValidation="true" />
      </div>
    </asp:Panel>

    <!-- Panel nueva Categoría -->
    <asp:Panel ID="pnlNewCat" runat="server" Visible="false" CssClass="section">
      <div class="form-grid">
        <div class="field-group">
          <label>Ingrese Categoría:</label>
          <asp:TextBox ID="txtNewCat" runat="server" CssClass="textbox" />
          <asp:RequiredFieldValidator ID="rfvNewCat" runat="server"
              ControlToValidate="txtNewCat" ErrorMessage="* Requerido"
              Display="Dynamic" ValidationGroup="cat" CssClass="validator" />
        </div>

        <div class="field-group">
          <label>Seleccione Categoría Maestra:</label>
          <asp:DropDownList ID="ddlCatMaestra2" runat="server" CssClass="dropdownlist"
              AutoPostBack="true" OnSelectedIndexChanged="ddlCatMaestra2_SelectedIndexChanged"
              ValidationGroup="cat">
            <asp:ListItem Text="-- Seleccione maestra --" Value="" />
            <asp:ListItem Text="+ Agregar maestra" Value="new" />
          </asp:DropDownList>
        </div>
      </div>

      <div style="text-align:center; margin-top:1rem;">
        <asp:Button ID="btnAddCat" runat="server" CssClass="greenbutton"
            Text="Guardar Categoría" OnClick="btnAddCat_Click"
            ValidationGroup="cat" />
      </div>
    </asp:Panel>

    <!-- Panel nueva Categoría Maestra -->
    <asp:Panel ID="pnlNewCatM" runat="server" Visible="false" CssClass="section">
      <div class="form-grid">
        <div class="field-group">
          <label>Ingrese Categoría Maestra:</label>
          <asp:TextBox ID="txtNewCatM" runat="server" CssClass="textbox" />
          <asp:RequiredFieldValidator ID="rfvNewCatM" runat="server"
              ControlToValidate="txtNewCatM" ErrorMessage="* Requerido"
              Display="Dynamic" ValidationGroup="catm" CssClass="validator" />
        </div>
      </div>

      <div style="text-align:center; margin-top:1rem;">
        <asp:Button ID="btnAddCatM" runat="server" CssClass="greenbutton"
            Text="Guardar Maestra" OnClick="btnAddCatM_Click"
            ValidationGroup="catm" CausesValidation="true" />
      </div>
    </asp:Panel>
  </fieldset>

  <!-- ========================================================= -->
  <!-- 2) DETALLE DE PRODUCTO                                    -->
  <!-- ========================================================= -->
  <fieldset class="section section-box">
    <legend>Detalle de producto</legend>
    <div class="form-grid">
      <!-- Tienda -->
      <div class="field-group">
        <label>Tienda:</label>
        <asp:DropDownList ID="ddlTienda" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvTienda" runat="server"
            ControlToValidate="ddlTienda" InitialValue=""
            ErrorMessage="* Requerido" Display="Dynamic" ValidationGroup="prod"
            CssClass="validator" />
      </div>

      <!-- Marca -->
      <div class="field-group">
        <label>Marca:</label>
        <asp:DropDownList ID="ddlMarca" runat="server" CssClass="form-control"
            AppendDataBoundItems="true" AutoPostBack="true"
            CausesValidation="false" OnSelectedIndexChanged="ddlMarca_SelectedIndexChanged">
          <asp:ListItem Text="-- Marca --" Value="" />
          <asp:ListItem Text="-- Agregar nueva marca --" Value="0" />
        </asp:DropDownList>
        <asp:Panel ID="pnlAddMarca" runat="server" Visible="false" style="margin-top:.5rem;">
          <asp:TextBox ID="txtNewMarca" runat="server" CssClass="form-control" placeholder="Nueva marca..." />
          <asp:Button ID="btnGuardarMarca" runat="server" CssClass="greenbutton"
              Text="Guardar marca" OnClick="btnGuardarMarca_Click" CausesValidation="false" />
        </asp:Panel>
      </div>

      <!-- Nombre -->
      <div class="field-group">
        <label>Nombre:</label>
        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
            ControlToValidate="txtNombre" ErrorMessage="* Requerido"
            Display="Dynamic" ValidationGroup="prod" CssClass="validator" />
      </div>

      <!-- Referencia -->
      <div class="field-group">
        <label>Referencia:</label>
        <asp:TextBox ID="txtReferencia" runat="server" CssClass="form-control" />
      </div>

      <!-- Código de barras -->
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

      <!-- Activo -->
      <div class="field-group">
        <label>Activo:</label>
        <asp:CheckBox ID="chkActivo" runat="server" Checked="true" />
      </div>

      <!-- Tipo de producto -->
      <div class="field-group">
        <label>Tipo de producto:</label>
        <asp:TextBox ID="txtTipo" runat="server" CssClass="form-control" />
      </div>

      <!-- Tipo de públicos -->
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
        <asp:RequiredFieldValidator ID="rfvPublico" runat="server"
            ControlToValidate="ddlPublico" InitialValue=""
            ErrorMessage="* Requerido" Display="Dynamic"
            ValidationGroup="prod" CssClass="validator" />
      </div>

      <!-- Descuento -->
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

  <!-- ========================================================= -->
  <!-- 3) PRECIOS DE VENTA                                       -->
  <!-- ========================================================= -->
  <fieldset class="section section-box">
    <legend>Precios de Venta (2 o más)</legend>
    <div id="preciosContainer"></div>
    <span id="btnAddTarifa" class="add-btn">+</span>
  </fieldset>

    <!-- Panel de "Nueva tarifa" (oculto por defecto; lo usa btnGuardarTarifa_Click en el .cs) -->
<asp:Panel ID="pnlAddTarifa" runat="server" Visible="false" CssClass="section section-box">
  <legend>Nueva tarifa</legend>
  <div class="form-grid">
    <div class="field-group">
      <label>Nombre de tarifa:</label>
      <asp:TextBox ID="txtTarifaNombre" runat="server" CssClass="form-control" />
    </div>
    <div class="field-group">
      <label>Cantidad mínima:</label>
      <asp:TextBox ID="txtTarifaCantidad" runat="server" CssClass="form-control" />
    </div>
  </div>
  <div style="text-align:center; margin-top:1rem;">
    <asp:Button ID="btnGuardarTarifa" runat="server"
      CssClass="greenbutton"
      Text="Guardar tarifa"
      OnClick="btnGuardarTarifa_Click" />
  </div>
</asp:Panel>


    <!-- 4) PRECIOS DE COMPRA -->
<asp:Panel ID="pnlPreciosCompra" runat="server" CssClass="section section-box">
  <fieldset>
    <legend>Precios de compra</legend>
    <div id="preciosCompraContainer"></div>
    <span id="btnAddPrecioCompra" class="add-btn">+</span>
  </fieldset>
</asp:Panel>

    <!-- 5) EXISTENCIAS (medida y cantidad) -->
<asp:Panel ID="pnlExistencias" runat="server" CssClass="section section-box">
  <fieldset>
    <legend>Existencias</legend>
    <div id="existenciasContainer"></div>
    <span id="btnAddExistencia" class="add-btn">+</span>
  </fieldset>
</asp:Panel>


    <!-- 6) IMÁGENES (múltiples) -->
<fieldset class="section section-box">
  <legend>Imágenes</legend>
  <div class="form-grid">
    <div class="field-group">
      <label>Color imagen (opcional):</label>
      <asp:DropDownList ID="ddlImgColor" runat="server" CssClass="form-control" />
    </div>
    <div class="field-group">
      <label>Archivo(s):</label>
      <asp:FileUpload ID="fuImagenes" runat="server" AllowMultiple="true" />
      <small>Puedes seleccionar 1 o más archivos.</small>
    </div>
    <div class="field-group">
      <label>Descripción (opcional, se aplica a todas):</label>
      <asp:TextBox ID="txtImgDesc" runat="server" CssClass="form-control" />
    </div>
  </div>
</fieldset>







 <%-- ================== PLANTILLAS ================== --%>
<%-- Tarifa (ya la tenías) --%>
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

<%-- NUEVA: Precios de compra --%>
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

      <!-- Mini-form de alta de proveedor -->
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

<%-- NUEVA: Existencias (medida + cantidad) --%>
<script type="text/html" id="historialTpl">
  <div class="form-grid existencia-row" style="border:1px solid #ddd; padding:.5rem; margin-bottom:.5rem;">
    <!-- Medida (selección o nueva) -->
    <div class="field-group">
      <label>Medida:</label>
      <select name="histMedida" class="form-control medida-select">
        <option value="">-- Medida --</option>
        <option value="new">+ Nueva medida…</option>
        ${medidasOptions}
      </select>
    </div>

    <!-- Cantidad -->
    <div class="field-group">
      <label>Cantidad:</label>
      <input type="number" name="histCantidad" class="form-control" />
    </div>

    <!-- Panel de “nueva medida” (se muestra solo si eligen + Nueva medida) -->
    <div class="medida-panel" style="grid-column:1 / -1; display:none; border-top:1px dashed #ccc; padding-top:.5rem; margin-top:.5rem;">
      <div class="form-grid">
        <div class="field-group">
          <label>Medida EU (texto/número):</label>
          <input type="text" name="histMedidaEu" class="form-control" />
        </div>
        <div class="field-group">
          <label>Ancho:</label>
          <input type="number" step="0.01" name="histAncho" class="form-control" />
        </div>
        <div class="field-group">
          <label>Largo:</label>
          <input type="number" step="0.01" name="histLargo" class="form-control" />
        </div>
        <div class="field-group">
          <label>Alto:</label>
          <input type="number" step="0.01" name="histAlto" class="form-control" />
        </div>
        <div class="field-group">
          <label>Profundidad:</label>
          <input type="number" step="0.01" name="histProfundidad" class="form-control" />
        </div>
        <div class="field-group">
          <label>Circunferencia:</label>
          <input type="number" step="0.01" name="histCircunferencia" class="form-control" />
        </div>
      </div>
      <small>Si dejas estos campos vacíos se guardarán como NULL.</small>
    </div>
  </div>
</script>


<%-- ================== LÓGICA JS ================== --%>
<script type="text/javascript">
    // ——— PRECIOS (venta) ———
    function buildTarifaOptions() {
        var opts = '<option value="">-- Seleccione tarifa --</option>';
        if (Array.isArray(tarifaData)) {
            tarifaData.forEach(function (t) {
                opts += '<option value="' + t.id + '">' + t.nombre + '</option>';
            });
        }
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

    function onTarifaChange() { updateTarifaFilters(); }

    function addTarifaRow() {
        var tplEl = document.getElementById('tarifaTpl');
        if (!tplEl) return;
        var cont = document.getElementById('preciosContainer');
        var div = document.createElement('div');
        div.innerHTML = tplEl.innerHTML;
        cont.appendChild(div);
        var sel = div.querySelector('select[name="tarifa"]');
        if (sel) {
            sel.innerHTML = buildTarifaOptions();
            sel.addEventListener('change', onTarifaChange);
            updateTarifaFilters();
        }
    }

    // ——— EXISTENCIAS ———
    function wireExistenciaRow(row) {
        if (!row) return;

        var sel = row.querySelector('select[name="histMedida"]');
        var panel = row.querySelector('.add-medida');
        var btnSave = row.querySelector('.btn-save-medida');
        var btnCanc = row.querySelector('.btn-cancel-medida');

        if (sel) {
            sel.addEventListener('change', function () {
                if (sel.value === 'new') {
                    panel.style.display = 'block';
                } else if (panel) {
                    panel.style.display = 'none';
                }
            });
        }
        if (btnCanc) {
            btnCanc.addEventListener('click', function () {
                if (panel) panel.style.display = 'none';
                if (sel) sel.value = '';
            });
        }
        if (btnSave) {
            btnSave.addEventListener('click', function () {
                guardarNuevaMedida(row);
            });
        }
    }

    function addExistenciaRow() {
        var tplEl = document.getElementById('historialTpl');
        if (!tplEl) return;

        var raw = tplEl.innerHTML;
        var filled = raw.replace('${medidasOptions}', (typeof medidasOptions !== 'undefined' ? medidasOptions : ''));

        var div = document.createElement('div');
        div.innerHTML = filled;
        document.getElementById('existenciasContainer').appendChild(div);

        wireExistenciaRow(div);
    }


    function guardarNuevaMedida(row) {
        var meu = (row.querySelector('.m-meu') || {}).value || '';
        var an = (row.querySelector('.m-ancho') || {}).value || '';
        var la = (row.querySelector('.m-largo') || {}).value || '';
        var al = (row.querySelector('.m-alto') || {}).value || '';
        var pr = (row.querySelector('.m-prof') || {}).value || '';
        var ci = (row.querySelector('.m-circ') || {}).value || '';

        PageMethods.AddMedida(meu, an, la, al, pr, ci,
            function (res) {
                var text = res && res.nombre ? res.nombre : '(sin nombre)';
                var value = res.id;

                // Insertar la nueva medida en TODOS los selects de medida
                document.querySelectorAll('select[name="histMedida"]').forEach(function (sel) {
                    if ([...sel.options].some(o => o.value === String(value))) return;

                    var opt = document.createElement('option');
                    opt.value = value; opt.textContent = text;

                    // después de placeholder y de 'new'
                    var idx = 0;
                    if (sel.options.length > 0 && sel.options[0].value === '') idx = 1;
                    if (sel.options.length > 1 && sel.options[1].value === 'new') idx = 2;
                    sel.add(opt, idx);
                });

                // Seleccionar en la fila actual y cerrar panel
                var sel = row.querySelector('select[name="histMedida"]');
                if (sel) sel.value = String(value);
                var panel = row.querySelector('.add-medida');
                if (panel) panel.style.display = 'none';

                // Limpiar campos del mini-form
                row.querySelectorAll('.add-medida input').forEach(function (i) { i.value = ''; });
            },
            function (err) {
                alert((err && err.get_message) ? err.get_message() : 'No se pudo guardar la medida.');
            }
        );
    }


    // ——— PRECIOS DE COMPRA ———
    function addPrecioCompraRow() {
        var tplEl = document.getElementById('precioCompraTpl');
        if (!tplEl) return;
        var filled = tplEl.innerHTML
            .replace('${supplierOptions}', (typeof supplierOptions !== 'undefined' ? supplierOptions : ''))
            .replace('${hoy}', (typeof hoy !== 'undefined' ? hoy : ''));

        var cont = document.getElementById('preciosCompraContainer');
        var div = document.createElement('div');
        div.innerHTML = filled;
        cont.appendChild(div);
        wireProveedorRow(div);
    }

    function wireExistenciaRow(row) {
        var sel = row.querySelector('.medida-select');
        var panel = row.querySelector('.medida-panel');
        if (!sel || !panel) return;

        // Mostrar/ocultar el panel de “Nueva medida”
        sel.addEventListener('change', function () {
            panel.style.display = (sel.value === 'new') ? 'block' : 'none';
        });
    }


    // —— PROVEEDOR (alta rápida) ——
    function wireProveedorRow(row) {
        var sel = row.querySelector('.proveedor-select');
        var panel = row.querySelector('.add-proveedor');
        var txt = row.querySelector('.nuevo-prov-nombre');
        var btnGuardar = row.querySelector('.btn-guardar-prov');
        var btnCancel = row.querySelector('.btn-cancelar-prov');

        if (!sel) return;

        sel.addEventListener('change', function () {
            if (sel.value === 'new') {
                panel.style.display = 'block';
                if (txt) txt.focus();
            } else {
                panel.style.display = 'none';
            }
        });

        if (btnGuardar) btnGuardar.addEventListener('click', function () { guardarNuevoProveedor(row); });

        if (txt) txt.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') { e.preventDefault(); if (btnGuardar) btnGuardar.click(); }
        });

        if (btnCancel) btnCancel.addEventListener('click', function () {
            panel.style.display = 'none';
            sel.value = ''; // obligamos a volver a elegir
        });
    }

    function guardarNuevoProveedor(row) {
        var txt = row.querySelector('.nuevo-prov-nombre');
        var sel = row.querySelector('.proveedor-select');
        var panel = row.querySelector('.add-proveedor');
        var btnGuardar = row.querySelector('.btn-guardar-prov');
        if (!txt || !sel) return;

        var nombre = (txt.value || '').trim();
        if (!nombre) { alert('Escribe el nombre del proveedor.'); txt.focus(); return; }

        if (btnGuardar) { if (btnGuardar.disabled) return; btnGuardar.disabled = true; }

        PageMethods.AddProveedor(nombre,
            function (result) {
                if (!result || !result.id) { alert('No se recibió un ID válido.'); if (btnGuardar) btnGuardar.disabled = false; return; }

                var opt = document.createElement('option');
                opt.value = result.id; opt.textContent = result.nombre;
                if (sel.options.length > 0) sel.insertBefore(opt, sel.options[1] || null); else sel.appendChild(opt);
                sel.value = result.id;

                document.querySelectorAll('.proveedor-select').forEach(function (otro) {
                    if ([...otro.options].some(o => o.value === String(result.id))) return;
                    var o2 = document.createElement('option');
                    o2.value = result.id; o2.textContent = result.nombre;
                    if (otro.options.length > 0) otro.insertBefore(o2, otro.options[1] || null); else otro.appendChild(o2);
                });

                if (typeof supplierOptions !== 'undefined') {
                    supplierOptions = '<option value="' + result.id + '">' + result.nombre + '</option>' + supplierOptions;
                }

                panel.style.display = 'none'; txt.value = '';
                if (btnGuardar) btnGuardar.disabled = false;
            },
            function (err) {
                var msg = (err && err.get_message) ? err.get_message() : 'Error al crear proveedor';
                alert(msg);
                if (btnGuardar) btnGuardar.disabled = false;
            }
        );
    }

    // —— Inicialización ——
    document.addEventListener('DOMContentLoaded', function () {
        // Precios (venta)
        var contP = document.getElementById('preciosContainer');
        if (contP) {
            contP.innerHTML = '';
            addTarifaRow();
            var btnAddTarifa = document.getElementById('btnAddTarifa');
            if (btnAddTarifa) btnAddTarifa.addEventListener('click', function (e) { e.preventDefault(); addTarifaRow(); });
        }

        // Existencias
        var contE = document.getElementById('existenciasContainer');
        if (contE) {
            contE.innerHTML = '';
            addExistenciaRow();
            var btnAddExistencia = document.getElementById('btnAddExistencia');
            if (btnAddExistencia) btnAddExistencia.addEventListener('click', function (e) { e.preventDefault(); addExistenciaRow(); });
        }

        // Precios de compra
        var contC = document.getElementById('preciosCompraContainer');
        if (contC) {
            contC.innerHTML = '';
            addPrecioCompraRow();
            var btnAddPrecioCompra = document.getElementById('btnAddPrecioCompra');
            if (btnAddPrecioCompra) btnAddPrecioCompra.addEventListener('click', function (e) { e.preventDefault(); addPrecioCompraRow(); });
        }
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
                    if (!err) { err = document.createElement('div'); err.className = 'field-error'; fg.appendChild(err); }
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

                var ddlMarca = document.getElementById('<%= ddlMarca.ClientID %>');     // ya no es obligatorio
      var ddlTienda = document.getElementById('<%= ddlTienda.ClientID %>');
      var ddlSubcat = document.getElementById('<%= ddlSubcategoria.ClientID %>');
    var ddlPublico= document.getElementById('<%= ddlPublico.ClientID %>');
    var txtNombre = document.getElementById('<%= txtNombre.ClientID %>');
    var txtCB     = document.getElementById('<%= txtCodigoBarras.ClientID %>');

                function need(el, msg) { if (!el || !el.value) { ok = false; addError(el, msg); if (!firstBad) firstBad = el; } }
                need(txtNombre, 'Ingrese el nombre');
                need(txtCB, 'Ingrese código de barras');
                // Marca opcional: quitamos esta línea -> need(ddlMarca, 'Seleccione una marca');
                need(ddlTienda, 'Seleccione una tienda');
                need(ddlSubcat, 'Seleccione subcategoría');
                need(ddlPublico, 'Seleccione un tipo de público');

                // Existencias
                var exRows = document.querySelectorAll('#existenciasContainer .existencia-row');
                if (exRows.length === 0) {
                    ok = false; var contE = document.querySelector('#existenciasContainer');
                    addError(contE, 'Agregue al menos una existencia'); if (!firstBad) firstBad = contE;
                } else {
                    exRows.forEach(function (r) {
                        var medSel = r.querySelector('select[name="histMedida"]');
                        var cant = r.querySelector('input[name="histCantidad"]');
                        if (!medSel || !medSel.value) { ok = false; addError(medSel, 'Seleccione o cree una medida'); if (!firstBad) firstBad = medSel; }
                        if (!cant || +cant.value <= 0) { ok = false; addError(cant, 'Cantidad > 0'); if (!firstBad) firstBad = cant; }

                        if (medSel && medSel.value === 'new') {
                            var medEu = r.querySelector('input[name="histMedidaEu"]');
                            if (!medEu || medEu.value.trim() === '') {
                                ok = false; addError(medEu, 'Ingrese la Medida EU'); if (!firstBad) firstBad = medEu;
                            }
                            // Los demás (ancho/largo/alto/…) son opcionales -> se guardan NULL si están vacíos
                        }
                    });
                }


                // Tarifas (2+)
                var tRows = document.querySelectorAll('#preciosContainer .tarifa-row');
                if (tRows.length < 2) { ok = false; var contT = document.querySelector('#preciosContainer'); addError(contT, 'Agregue al menos 2 tarifas'); if (!firstBad) firstBad = contT; }
                tRows.forEach(function (r) {
                    var sel = r.querySelector('select[name="tarifa"]');
                    var p = r.querySelector('input[name="precioVal"]');
                    if (!sel || !sel.value) { ok = false; addError(sel, 'Seleccione tarifa'); if (!firstBad) firstBad = sel; }
                    if (!p || +p.value <= 0) { ok = false; addError(p, 'Precio > 0'); if (!firstBad) firstBad = p; }
                });

                // Compra (si hay)
                var cRows = document.querySelectorAll('#preciosCompraContainer .compra-row');
                cRows.forEach(function (r) {
                    var precio = r.querySelector('input[name="compPrecio"]');
                    if (precio && precio.value !== '' && +precio.value <= 0) { ok = false; addError(precio, 'Precio > 0'); if (!firstBad) firstBad = precio; }
                });

                if (!ok && firstBad) { firstBad.scrollIntoView({ behavior: 'smooth', block: 'center' }); firstBad.focus({ preventScroll: true }); }
                return ok;
            };
        })();
    </script>

</asp:Content>
