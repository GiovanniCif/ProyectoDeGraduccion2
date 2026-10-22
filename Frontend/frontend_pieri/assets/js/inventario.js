const INVENTARIO_CATALOGS = {
  tiposMovimiento: [
    { id: 1, nombre: 'Entrada' }, { id: 2, nombre: 'Salida' }, { id: 3, nombre: 'Ajuste' }, { id: 4, nombre: 'Merma' }
  ]
};

let editModeProducto = false;
let currentProductoId = null;
let editModeInsumo = false;
let currentInsumoId = null;

let catalogoTiposPrenda = [];
let catalogoTallas = [];
let catalogoColores = [];
let catalogoUnidades = [];

let currentCatalogoTipo = null;
let currentSelectElement = null;
let currentCatalogPickerField = null;

const ADD_NEW_VALUE = '__add_new__';

const CATALOG_PICKER_CONFIG = {
  ptTipoPrenda: {
    title: 'Seleccionar tipo de prenda',
    placeholder: 'Sin tipo de prenda',
    getOptions: () => catalogoTiposPrenda
  },
  ptTalla: {
    title: 'Seleccionar talla',
    placeholder: 'Sin talla',
    getOptions: () => catalogoTallas
  },
  ptColor: {
    title: 'Seleccionar color',
    placeholder: 'Sin color',
    getOptions: () => catalogoColores
  },
  ptUnidad: {
    title: 'Seleccionar unidad de medida',
    placeholder: 'Sin unidad de medida',
    getOptions: () => catalogoUnidades
  },
  insUnidad: {
    title: 'Seleccionar unidad de medida',
    placeholder: 'Sin unidad de medida',
    getOptions: () => catalogoUnidades
  }
};

function generarCodigoProducto() {
  const timestamp = Date.now().toString().slice(-6);
  return `PT-${timestamp}`;
}

function generarCodigoInsumo() {
  const timestamp = Date.now().toString().slice(-6);
  return `INS-${timestamp}`;
}

function autoGenerarCodigos() {
  if (!editModeProducto) {
    document.getElementById('ptCodigo').value = generarCodigoProducto();
  }
  if (!editModeInsumo) {
    document.getElementById('insCodigo').value = generarCodigoInsumo();
  }
}

function resetProductoForm() {
  document.getElementById('productoForm')?.reset();
  editModeProducto = false;
  currentProductoId = null;
  document.getElementById('ptId').value = '';
  document.getElementById('btnGuardarProducto').textContent = 'Guardar producto';
  document.getElementById('btnCancelarPt').style.display = 'none';
  actualizarTodosCatalogPickers();
  autoGenerarCodigos();
}

function resetInsumoForm() {
  document.getElementById('insumoForm')?.reset();
  editModeInsumo = false;
  currentInsumoId = null;
  document.getElementById('insId').value = '';
  document.getElementById('btnGuardarInsumo').textContent = 'Guardar insumo';
  document.getElementById('btnCancelarIns').style.display = 'none';
  actualizarTodosCatalogPickers();
  autoGenerarCodigos();
}

document.addEventListener('DOMContentLoaded', async () => {
  console.log('=== Iniciando carga de inventario ===');
  
  await cargarCatalogos();
  console.log('Catálogos cargados:', {
    TipoPrenda: catalogoTiposPrenda.length,
    Talla: catalogoTallas.length,
    Color: catalogoColores.length,
    Unidad: catalogoUnidades.length
  });
  
  await cargarCombosInventario();
  console.log('Combos llenados');
  
  await cargarProductos();
  await cargarInsumos();
  autoGenerarCodigos();

  document.getElementById('productoForm')?.addEventListener('submit', guardarProducto);
  document.getElementById('movProductoForm')?.addEventListener('submit', registrarMovimientoProducto);
  document.getElementById('insumoForm')?.addEventListener('submit', guardarInsumo);
  document.getElementById('movInsumoForm')?.addEventListener('submit', registrarMovimientoInsumo);
  document.getElementById('btnRecargarProductos')?.addEventListener('click', async () => {
    await cargarCatalogos();
    await cargarCombosInventario();
    await cargarProductos();
  });
  document.getElementById('btnRecargarInsumos')?.addEventListener('click', async () => {
    await cargarCatalogos();
    await cargarCombosInventario();
    await cargarInsumos();
  });
  document.getElementById('btnCancelarPt')?.addEventListener('click', resetProductoForm);
  document.getElementById('btnCancelarIns')?.addEventListener('click', resetInsumoForm);

  document.getElementById('ptTipoPrenda')?.addEventListener('change', handleSelectChange);
  document.getElementById('ptTalla')?.addEventListener('change', handleSelectChange);
  document.getElementById('ptColor')?.addEventListener('change', handleSelectChange);
  document.getElementById('ptUnidad')?.addEventListener('change', handleSelectChange);
  document.getElementById('insUnidad')?.addEventListener('change', handleSelectChange);

  document.querySelectorAll('[data-catalog-picker]').forEach(trigger => {
    trigger.addEventListener('click', () => abrirCatalogPicker(trigger.dataset.catalogPicker));
  });
  document.getElementById('catalogPickerSearch')?.addEventListener('input', event => {
    renderCatalogPickerOptions(event.target.value);
  });
  document.getElementById('btnClearCatalogSelection')?.addEventListener('click', limpiarCatalogPickerSelection);
  document.getElementById('btnAddCatalogFromPicker')?.addEventListener('click', abrirNuevoCatalogoDesdePicker);

  document.getElementById('btnGuardarCatalogo')?.addEventListener('click', guardarNuevoCatalogo);
  document.getElementById('btnVerAlertasStock')?.addEventListener('click', cargarAlertasStock);
  document.getElementById('btnRecargarAlertasStock')?.addEventListener('click', cargarAlertasStock);
  document.getElementById('btnFiltrarKardex')?.addEventListener('click', cargarKardexActual);
});

async function cargarCatalogos() {
  console.log('cargarCatalogos: Iniciando...');
  
  try {
    console.log('cargarCatalogos: Llamando a APIs...');
    
    const [tiposPrenda, tallas, colores, unidades] = await Promise.all([
      apiRequest('/Catalogos/tipos-prenda').then(r => { console.log('API tipos-prenda:', r); return r; }).catch(e => { console.log('API tipos-prenda FALLÓ:', e.message); return []; }),
      apiRequest('/Catalogos/tallas').then(r => { console.log('API tallas:', r); return r; }).catch(e => { console.log('API tallas FALLÓ:', e.message); return []; }),
      apiRequest('/Catalogos/colores').then(r => { console.log('API colores:', r); return r; }).catch(e => { console.log('API colores FALLÓ:', e.message); return []; }),
      apiRequest('/Catalogos/unidades-medida').then(r => { console.log('API unidades:', r); return r; }).catch(e => { console.log('API unidades FALLÓ:', e.message); return []; })
    ]);

    console.log('cargarCatalogos: Respuestas recibidas:', { tiposPrenda, tallas, colores, unidades });

    catalogoTiposPrenda = tiposPrenda || [];
    catalogoTallas = tallas || [];
    catalogoColores = colores || [];
    catalogoUnidades = unidades || [];

    console.log('cargarCatalogos: Longitudes después de asignar:', {
      TipoPrenda: catalogoTiposPrenda.length,
      Talla: catalogoTallas.length,
      Color: catalogoColores.length,
      Unidad: catalogoUnidades.length
    });

    if (catalogoTiposPrenda.length === 0) {
      console.log('cargarCatalogos: Usando valores de respaldo para TipoPrenda');
      catalogoTiposPrenda = [
        { id: 1, nombre: 'Camisa' },
        { id: 2, nombre: 'Pantalón' },
        { id: 3, nombre: 'Gabacha' },
        { id: 4, nombre: 'Uniforme completo' },
        { id: 5, nombre: 'Camisa tipo polo' },
        { id: 6, nombre: 'Chaleco' },
        { id: 7, nombre: 'Falda' },
        { id: 8, nombre: 'Vestido' }
      ];
    }
    if (catalogoTallas.length === 0) {
      console.log('cargarCatalogos: Usando valores de respaldo para Tallas');
      catalogoTallas = [
        { id: 1, nombre: 'XS' }, { id: 2, nombre: 'S' }, { id: 3, nombre: 'M' },
        { id: 4, nombre: 'L' }, { id: 5, nombre: 'XL' }, { id: 6, nombre: 'XXL' },
        { id: 7, nombre: 'XXXL' }, { id: 8, nombre: '28' }, { id: 9, nombre: '30' },
        { id: 10, nombre: '32' }, { id: 11, nombre: '34' }, { id: 12, nombre: '36' },
        { id: 13, nombre: '38' }, { id: 14, nombre: '40' }, { id: 15, nombre: '42' },
        { id: 16, nombre: '44' }, { id: 17, nombre: 'Talla única' }
      ];
    }
    if (catalogoColores.length === 0) {
      console.log('cargarCatalogos: Usando valores de respaldo para Colores');
      catalogoColores = [
        { id: 1, nombre: 'Negro' }, { id: 2, nombre: 'Blanco' }, { id: 3, nombre: 'Azul' },
        { id: 4, nombre: 'Rojo' }, { id: 5, nombre: 'Gris' }, { id: 6, nombre: 'Verde' },
        { id: 7, nombre: 'Amarillo' }, { id: 8, nombre: 'Naranja' }, { id: 9, nombre: 'Morado' },
        { id: 10, nombre: 'Rosa' }, { id: 11, nombre: 'Café' }, { id: 12, nombre: 'Beige' },
        { id: 13, nombre: 'Vino' }, { id: 14, nombre: 'Turquesa' }, { id: 15, nombre: 'Mostaza' }
      ];
    }
    if (catalogoUnidades.length === 0) {
      console.log('cargarCatalogos: Usando valores de respaldo para Unidades');
      catalogoUnidades = [
        { id: 1, nombre: 'Unidad' }, { id: 2, nombre: 'Metro' }, { id: 3, nombre: 'Kilogramo' },
        { id: 4, nombre: 'Gramo' }, { id: 5, nombre: 'Litro' }, { id: 6, nombre: 'Millilitro' },
        { id: 7, nombre: 'Rollo' }, { id: 8, nombre: 'Caja' }, { id: 9, nombre: 'Paquete' },
        { id: 10, nombre: 'Docena' }, { id: 11, nombre: 'Centímetro' }, { id: 12, nombre: 'Yarda' },
        { id: 13, nombre: 'Par' }, { id: 14, nombre: 'Juego' }
      ];
    }

    console.log('cargarCatalogos: Finalizado. Catálogos:', {
      TipoPrenda: catalogoTiposPrenda.map(c => c.nombre),
      Talla: catalogoTallas.map(c => c.nombre),
      Color: catalogoColores.map(c => c.nombre),
      Unidad: catalogoUnidades.map(c => c.nombre)
    });
  } catch (error) {
    console.error('cargarCatalogos: ERROR GENERAL:', error);
    console.warn('Error cargando catálogos, usando valores por defecto:', error);
  }
}

function fillSelectWithAddNew(elementId, options, placeholder, addNewLabel) {
  console.log(`fillSelectWithAddNew: Llenando ${elementId} con`, options.length, 'opciones');
  
  const select = document.getElementById(elementId);
  if (!select) {
    console.error(`fillSelectWithAddNew: No se encontró el elemento ${elementId}`);
    return;
  }

  const selectedValue = select.value;
  select.innerHTML = `<option value="">${placeholder}</option>`;
  
  options.forEach(option => {
    const opt = document.createElement("option");
    opt.value = option.id;
    opt.textContent = option.nombre;
    select.appendChild(opt);
  });

  const addNewOpt = document.createElement("option");
  addNewOpt.value = ADD_NEW_VALUE;
  addNewOpt.textContent = `+ ${addNewLabel}`;
  addNewOpt.style.color = '#7B1114';
  addNewOpt.style.fontWeight = '600';
  select.appendChild(addNewOpt);

  if ([...select.options].some(option => option.value === selectedValue)) {
    select.value = selectedValue;
  }

  actualizarCatalogPickerLabel(elementId);
  
  console.log(`fillSelectWithAddNew: ${elementId} ahora tiene`, select.options.length, 'opciones');
}

function actualizarCatalogPickerLabel(fieldId) {
  const config = CATALOG_PICKER_CONFIG[fieldId];
  const select = document.getElementById(fieldId);
  const trigger = document.querySelector(`[data-catalog-picker="${fieldId}"]`);
  const label = trigger?.querySelector('[data-catalog-picker-label]');

  if (!config || !select || !trigger || !label) return;

  const selectedOption = select.options[select.selectedIndex];
  const hasSelection = Boolean(select.value && select.value !== ADD_NEW_VALUE);
  label.textContent = hasSelection && selectedOption
    ? selectedOption.textContent
    : config.placeholder;
  trigger.classList.toggle('catalog-picker-empty', !hasSelection);
  trigger.setAttribute('aria-label', `${config.title}. Selección actual: ${label.textContent}`);
}

function actualizarTodosCatalogPickers() {
  Object.keys(CATALOG_PICKER_CONFIG).forEach(actualizarCatalogPickerLabel);
}

function abrirCatalogPicker(fieldId) {
  const config = CATALOG_PICKER_CONFIG[fieldId];
  const modalElement = document.getElementById('catalogPickerModal');
  const searchInput = document.getElementById('catalogPickerSearch');

  if (!config || !modalElement) return;

  currentCatalogPickerField = fieldId;
  document.getElementById('catalogPickerModalLabel').textContent = config.title;
  if (searchInput) searchInput.value = '';
  renderCatalogPickerOptions('');

  const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
  modalElement.addEventListener('shown.bs.modal', () => searchInput?.focus(), { once: true });
  modal.show();
}

function renderCatalogPickerOptions(searchText = '') {
  const config = CATALOG_PICKER_CONFIG[currentCatalogPickerField];
  const select = document.getElementById(currentCatalogPickerField);
  const container = document.getElementById('catalogPickerOptions');
  const emptyState = document.getElementById('catalogPickerEmpty');

  if (!config || !select || !container || !emptyState) return;

  const normalizedSearch = searchText.trim().toLocaleLowerCase('es');
  const options = config.getOptions().filter(option => {
    const searchableText = [option.nombre, option.abreviatura, option.descripcion]
      .filter(Boolean)
      .join(' ')
      .toLocaleLowerCase('es');
    return !normalizedSearch || searchableText.includes(normalizedSearch);
  });

  container.innerHTML = '';
  emptyState.classList.toggle('d-none', options.length > 0);

  options.forEach(option => {
    const button = document.createElement('button');
    const isSelected = String(option.id) === select.value;
    button.type = 'button';
    button.className = `list-group-item list-group-item-action catalog-picker-option${isSelected ? ' active' : ''}`;
    button.setAttribute('role', 'option');
    button.setAttribute('aria-selected', String(isSelected));

    const mainRow = document.createElement('span');
    mainRow.className = 'd-flex justify-content-between align-items-center gap-3';

    const name = document.createElement('span');
    name.className = 'catalog-picker-option-name';
    name.textContent = option.nombre;
    mainRow.appendChild(name);

    if (isSelected) {
      const selectedBadge = document.createElement('span');
      selectedBadge.className = 'badge rounded-pill text-bg-light';
      selectedBadge.textContent = 'Seleccionado';
      mainRow.appendChild(selectedBadge);
    }

    button.appendChild(mainRow);

    const detailText = [option.abreviatura, option.descripcion].filter(Boolean).join(' · ');
    if (detailText) {
      const detail = document.createElement('span');
      detail.className = 'catalog-picker-option-detail';
      detail.textContent = detailText;
      button.appendChild(detail);
    }

    button.addEventListener('click', () => seleccionarCatalogPickerOption(option.id));
    container.appendChild(button);
  });
}

function seleccionarCatalogPickerOption(optionId) {
  const select = document.getElementById(currentCatalogPickerField);
  if (!select) return;

  select.value = String(optionId);
  actualizarCatalogPickerLabel(currentCatalogPickerField);
  bootstrap.Modal.getInstance(document.getElementById('catalogPickerModal'))?.hide();
}

function limpiarCatalogPickerSelection() {
  const select = document.getElementById(currentCatalogPickerField);
  if (!select) return;

  select.value = '';
  actualizarCatalogPickerLabel(currentCatalogPickerField);
  bootstrap.Modal.getInstance(document.getElementById('catalogPickerModal'))?.hide();
}

function abrirNuevoCatalogoDesdePicker() {
  const fieldId = currentCatalogPickerField;
  const select = document.getElementById(fieldId);
  const pickerModalElement = document.getElementById('catalogPickerModal');
  const pickerModal = bootstrap.Modal.getInstance(pickerModalElement);

  if (!select) return;

  const openAddModal = () => {
    if (select.dataset.valueBeforeAdd === undefined) {
      select.dataset.valueBeforeAdd = select.value === ADD_NEW_VALUE ? '' : select.value;
    }
    select.value = ADD_NEW_VALUE;
    handleSelectChange({ target: select });
  };

  if (pickerModal) {
    pickerModalElement.addEventListener('hidden.bs.modal', openAddModal, { once: true });
    pickerModal.hide();
  } else {
    openAddModal();
  }
}

function llenarCatalogosEnComboboxes() {
  console.log('llenarCatalogosEnComboboxes: Iniciando...');
  fillSelectWithAddNew('ptTipoPrenda', catalogoTiposPrenda, 'Sin tipo de prenda', 'Agregar tipo de prenda');
  fillSelectWithAddNew('ptTalla', catalogoTallas, 'Sin talla', 'Agregar talla');
  fillSelectWithAddNew('ptColor', catalogoColores, 'Sin color', 'Agregar color');
  fillSelectWithAddNew('ptUnidad', catalogoUnidades, 'Sin unidad de medida', 'Agregar unidad');
  fillSelectWithAddNew('insUnidad', catalogoUnidades, 'Sin unidad de medida', 'Agregar unidad');
  console.log('llenarCatalogosEnComboboxes: Terminado');
}

async function cargarCombosInventario() {
  llenarCatalogosEnComboboxes();

  try {
    const [usuarios, pedidos, productos, insumos] = await Promise.all([
      apiRequest('/Usuarios').catch(() => []),
      apiRequest('/Pedidos').catch(() => []),
      apiRequest('/Inventario/productos-terminados').catch(() => []),
      apiRequest('/Inventario/insumos').catch(() => [])
    ]);

    fillSelect('movPtTipo', INVENTARIO_CATALOGS.tiposMovimiento.map(x => ({ value: x.id, label: x.nombre })), 'Seleccione');
    fillSelect('movPtUsuario', (usuarios || []).filter(u => u.estado).map(x => ({ value: x.id, label: x.username })), 'Seleccione');
    fillSelect('movPtPedido', (pedidos || []).map(x => ({ value: x.id, label: `${x.numeroPedido} | ${x.cliente}` })), 'Sin pedido');
    fillSelect('movPtProducto', (productos || []).map(x => ({ value: x.id, label: `${x.codigo} - ${x.nombre} (Stock: ${x.stockActual})` })), 'Seleccione un producto');

    fillSelect('movInsTipo', INVENTARIO_CATALOGS.tiposMovimiento.map(x => ({ value: x.id, label: x.nombre })), 'Seleccione');
    fillSelect('movInsUsuario', (usuarios || []).filter(u => u.estado).map(x => ({ value: x.id, label: x.username })), 'Seleccione');
    fillSelect('movInsInsumo', (insumos || []).map(x => ({ value: x.id, label: `${x.codigo} - ${x.nombre} (Stock: ${x.stockActual})` })), 'Seleccione un insumo');
  } catch (error) {
    console.warn('Error cargando datos adicionales:', error);
  }
}

function handleSelectChange(e) {
  const value = e.target.value;
  if (value === ADD_NEW_VALUE) {
    const selectId = e.target.id;
    currentSelectElement = e.target;

    let tipoLabel = '';
    let showAbreviatura = false;

    switch(selectId) {
      case 'ptTipoPrenda':
        currentCatalogoTipo = 'tipos-prenda';
        tipoLabel = 'Tipo de Prenda';
        break;
      case 'ptTalla':
        currentCatalogoTipo = 'tallas';
        tipoLabel = 'Talla';
        break;
      case 'ptColor':
        currentCatalogoTipo = 'colores';
        tipoLabel = 'Color';
        break;
      case 'ptUnidad':
      case 'insUnidad':
        currentCatalogoTipo = 'unidades-medida';
        tipoLabel = 'Unidad de Medida';
        showAbreviatura = true;
        break;
    }

    document.getElementById('catalogoTipoLabel').textContent = tipoLabel;
    document.getElementById('addCatalogoModalLabel').textContent = `Agregar ${tipoLabel}`;
    document.getElementById('abreviaturaField').style.display = showAbreviatura ? 'block' : 'none';
    
    document.getElementById('newCatalogoNombre').value = '';
    document.getElementById('newCatalogoAbreviatura').value = '';
    document.getElementById('newCatalogoDescripcion').value = '';

    const modal = new bootstrap.Modal(document.getElementById('addCatalogoModal'));
    modal.show();

    modal._element.addEventListener('hidden.bs.modal', function onHidden() {
      if (currentSelectElement && currentSelectElement.value === ADD_NEW_VALUE) {
        currentSelectElement.value = currentSelectElement.dataset.valueBeforeAdd || '';
        actualizarCatalogPickerLabel(currentSelectElement.id);
      }
      if (currentSelectElement) delete currentSelectElement.dataset.valueBeforeAdd;
      this.removeEventListener('hidden.bs.modal', onHidden);
    }, { once: true });
  }
}

function validarNombreCatalogo(nombre, tipo) {
  if (!nombre || nombre.trim().length === 0) {
    return { valido: false, mensaje: 'El nombre es obligatorio' };
  }

  const nombreLimpio = nombre.trim();

  if (nombreLimpio.length < 1) {
    return { valido: false, mensaje: 'El nombre debe tener al menos 1 carácter' };
  }

  if (nombreLimpio.length > 100) {
    return { valido: false, mensaje: 'El nombre no puede exceder 100 caracteres' };
  }

  const catalogos = obtenerCatalogoPorTipo(tipo);

  const existe = catalogos.some(c =>
    c.nombre && c.nombre.trim().toLowerCase() === nombreLimpio.toLowerCase()
  );

  if (existe) {
    return {
      valido: false,
      mensaje: `Ya existe un elemento con el nombre "${nombreLimpio}"`
    };
  }

  return { valido: true, mensaje: '' };
}

function obtenerCatalogoPorTipo(tipo) {
  switch(tipo) {
    case 'tipos-prenda': return catalogoTiposPrenda;
    case 'tallas': return catalogoTallas;
    case 'colores': return catalogoColores;
    case 'unidades-medida': return catalogoUnidades;
    default: return [];
  }
}

function obtenerNombreTipo(tipo) {
  switch(tipo) {
    case 'tipos-prenda': return 'Tipo de prenda';
    case 'tallas': return 'Talla';
    case 'colores': return 'Color';
    case 'unidades-medida': return 'Unidad de medida';
    default: return 'Elemento';
  }
}

async function guardarNuevoCatalogo() {
  const btnGuardar = document.getElementById('btnGuardarCatalogo');
  const nombreInput = document.getElementById('newCatalogoNombre');
  const abreviaturaInput = document.getElementById('newCatalogoAbreviatura');
  const descripcionInput = document.getElementById('newCatalogoDescripcion');

  const nombre = nombreInput?.value;
  const abreviatura = abreviaturaInput?.value;
  const descripcion = descripcionInput?.value;

  const validacion = validarNombreCatalogo(nombre, currentCatalogoTipo);
  if (!validacion.valido) {
    showAlert('inventarioAlert', validacion.mensaje, 'warning');
    return;
  }

  const nombreLimpio = nombre.trim();
  const abreviaturaLimpia = abreviatura?.trim();
  const descripcionLimpia = descripcion?.trim();

  nombreInput.value = nombreLimpio;
  if (abreviaturaInput) abreviaturaInput.value = abreviaturaLimpia || '';
  if (descripcionInput) descripcionInput.value = descripcionLimpia || '';

  if (btnGuardar) {
    btnGuardar.disabled = true;
    btnGuardar.textContent = 'Guardando...';
  }

  try {
    const catalogosActuales = obtenerCatalogoPorTipo(currentCatalogoTipo);
    const nombreTipo = obtenerNombreTipo(currentCatalogoTipo);

    const duplicadoBackend = catalogosActuales.some(c => 
      c.nombre.trim().toLowerCase() === nombreLimpio.toLowerCase()
    );

    if (duplicadoBackend) {
      showAlert('inventarioAlert', `Ya existe un${nombreTipo === 'Unidad de medida' ? 'a' : ''} ${nombreTipo.toLowerCase()} con el nombre "${nombreLimpio}".`, 'warning');
      if (btnGuardar) {
        btnGuardar.disabled = false;
        btnGuardar.textContent = 'Guardar';
      }
      return;
    }

    const payload = {
      nombre: nombreLimpio,
      descripcion: descripcionLimpia || null
    };

    if (currentCatalogoTipo === 'unidades-medida' && abreviaturaLimpia) {
      payload.abreviatura = abreviaturaLimpia.length > 20 ? abreviaturaLimpia.substring(0, 20) : abreviaturaLimpia;
    }

    const resultado = await apiRequest(`/Catalogos/${currentCatalogoTipo}`, {
      method: 'POST',
      body: JSON.stringify(payload)
    });

    bootstrap.Modal.getInstance(document.getElementById('addCatalogoModal'))?.hide();

    await cargarCatalogos();
    await cargarCombosInventario();

    if (currentSelectElement) {
      const nuevoId = resultado?.id;
      if (nuevoId) {
        currentSelectElement.value = nuevoId;
        delete currentSelectElement.dataset.valueBeforeAdd;
        actualizarCatalogPickerLabel(currentSelectElement.id);
      }
    }

    showAlert('inventarioAlert', `${nombreTipo} "${nombreLimpio}" agregado correctamente.`);
  } catch (error) {
    let mensajeError = error.message;
    if (mensajeError.includes('Ya existe') || mensajeError.includes('duplicado') || mensajeError.includes('duplicate')) {
      mensajeError = `Ya existe un${nombreTipo === 'Unidad de medida' ? 'a' : ''} ${nombreTipo.toLowerCase()} con este nombre. Por favor ingresa uno diferente.`;
    }
    showAlert('inventarioAlert', mensajeError, 'danger');
  } finally {
    if (btnGuardar) {
      btnGuardar.disabled = false;
      btnGuardar.textContent = 'Guardar';
    }
  }
}

async function guardarProducto(e) {
  e.preventDefault();
  hideAlert('inventarioAlert');
  
  const payload = {
    codigo: valueOf('ptCodigo'), 
    nombre: valueOf('ptNombre'), 
    idTipoPrenda: parseNullableInt(valueOf('ptTipoPrenda')),
    modelo: valueOf('ptModelo'), 
    idTalla: parseNullableInt(valueOf('ptTalla')), 
    idColor: parseNullableInt(valueOf('ptColor')),
    idUnidadMedida: parseNullableInt(valueOf('ptUnidad')), 
    stockActual: Number(valueOf('ptStockActual')), 
    stockMinimo: Number(valueOf('ptStockMinimo')),
    costoReferencial: Number(valueOf('ptCostoReferencial')), 
    precioReferencial: Number(valueOf('ptPrecioReferencial')), 
    observaciones: valueOf('ptObservaciones')
  };

  try {
    if (editModeProducto && currentProductoId) {
      await apiRequest(`/Inventario/productos-terminados/${currentProductoId}`, { 
        method: 'PUT', 
        body: JSON.stringify(payload) 
      });
      showAlert('inventarioAlert', 'Producto actualizado correctamente.');
    } else {
      await apiRequest('/Inventario/productos-terminados', { 
        method: 'POST', 
        body: JSON.stringify(payload) 
      });
      showAlert('inventarioAlert', 'Producto terminado guardado correctamente.');
    }
    
    resetProductoForm();
    await cargarProductos();
    await cargarCombosInventario();
  } catch (error) { showAlert('inventarioAlert', error.message, 'danger'); }
}

async function registrarMovimientoProducto(e) {
  e.preventDefault();
  hideAlert('inventarioAlert');
  const payload = {
    idProductoTerminado: Number(valueOf('movPtProducto')), 
    idTipoMovimiento: Number(valueOf('movPtTipo')), 
    idPedido: parseNullableInt(valueOf('movPtPedido')),
    cantidad: Number(valueOf('movPtCantidad')), 
    motivo: valueOf('movPtMotivo'), 
    referencia: valueOf('movPtReferencia'), 
    idUsuario: Number(valueOf('movPtUsuario')), 
    observaciones: valueOf('movPtObservaciones')
  };
  try {
    await apiRequest('/Inventario/productos-terminados/movimientos', { method: 'POST', body: JSON.stringify(payload) });
    showAlert('inventarioAlert', 'Movimiento de producto terminado registrado.');
    e.target.reset();
    await cargarProductos();
    await cargarCombosInventario();
  } catch (error) { showAlert('inventarioAlert', error.message, 'danger'); }
}

async function guardarInsumo(e) {
  e.preventDefault();
  hideAlert('inventarioAlert');
  
  const payload = {
    codigo: valueOf('insCodigo'), 
    nombre: valueOf('insNombre'), 
    categoria: valueOf('insCategoria'), 
    idUnidadMedida: parseNullableInt(valueOf('insUnidad')),
    stockActual: Number(valueOf('insStockActual')), 
    stockMinimo: Number(valueOf('insStockMinimo')), 
    costoReferencial: Number(valueOf('insCostoReferencial')), 
    observaciones: valueOf('insObservaciones')
  };

  try {
    await apiRequest('/Inventario/insumos', { 
      method: 'POST', 
      body: JSON.stringify(payload) 
    });
    showAlert('inventarioAlert', 'Insumo guardado correctamente.');
    e.target.reset();
    autoGenerarCodigos();
    await cargarInsumos();
    await cargarCombosInventario();
  } catch (error) { showAlert('inventarioAlert', error.message, 'danger'); }
}

async function registrarMovimientoInsumo(e) {
  e.preventDefault();
  hideAlert('inventarioAlert');
  const payload = {
    idInsumo: Number(valueOf('movInsInsumo')), 
    idTipoMovimiento: Number(valueOf('movInsTipo')), 
    cantidad: Number(valueOf('movInsCantidad')),
    motivo: valueOf('movInsMotivo'), 
    referencia: valueOf('movInsReferencia'), 
    idUsuario: Number(valueOf('movInsUsuario')), 
    observaciones: valueOf('movInsObservaciones')
  };
  try {
    await apiRequest('/Inventario/insumos/movimientos', { method: 'POST', body: JSON.stringify(payload) });
    showAlert('inventarioAlert', 'Movimiento de insumo registrado.');
    e.target.reset();
    await cargarInsumos();
    await cargarCombosInventario();
  } catch (error) { showAlert('inventarioAlert', error.message, 'danger'); }
}

async function cargarProductos() {
  try {
    const productos = await apiRequest('/Inventario/productos-terminados');
    const tbody = document.querySelector('#productosTable tbody');
    tbody.innerHTML = '';
    productos.forEach(p => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${p.id}</td><td>${p.codigo}</td><td>${p.nombre}</td><td>${p.tipoPrenda ?? '-'}</td><td>${p.stockActual}</td><td>${p.stockMinimo}</td><td>${money(p.precioReferencial)}</td>
      <td>
        <div class="table-actions">
          <button class="btn btn-outline-primary btn-sm btn-icon" title="Editar" onclick="editarProducto(${p.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
          </button>
          <button class="btn btn-outline-info btn-sm btn-icon" title="Kardex" onclick="verKardexProducto(${p.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line><polyline points="10 9 9 9 8 9"></polyline></svg>
          </button>
        </div>
      </td>`;
      tbody.appendChild(tr);
    });
  } catch (error) { showAlert('inventarioAlert', error.message, 'danger'); }
}

async function editarProducto(id) {
  try {
    const producto = await apiRequest(`/Inventario/productos-terminados/${id}`);
    
    editModeProducto = true;
    currentProductoId = id;
    document.getElementById('ptId').value = id;
    document.getElementById('btnGuardarProducto').textContent = 'Actualizar producto';
    document.getElementById('btnCancelarPt').style.display = 'inline-block';

    document.getElementById('ptCodigo').value = producto.codigo;
    document.getElementById('ptNombre').value = producto.nombre;
    if (producto.idTipoPrenda) document.getElementById('ptTipoPrenda').value = producto.idTipoPrenda;
    document.getElementById('ptModelo').value = producto.modelo ?? '';
    if (producto.idTalla) document.getElementById('ptTalla').value = producto.idTalla;
    if (producto.idColor) document.getElementById('ptColor').value = producto.idColor;
    if (producto.idUnidadMedida) document.getElementById('ptUnidad').value = producto.idUnidadMedida;
    actualizarTodosCatalogPickers();
    document.getElementById('ptStockActual').value = producto.stockActual;
    document.getElementById('ptStockMinimo').value = producto.stockMinimo;
    document.getElementById('ptCostoReferencial').value = producto.costoReferencial;
    document.getElementById('ptPrecioReferencial').value = producto.precioReferencial;
    document.getElementById('ptObservaciones').value = producto.observaciones ?? '';

    document.querySelector('[data-bs-target="#tab-productos"]')?.click();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  } catch (error) {
    showAlert('inventarioAlert', error.message, 'danger');
  }
}

async function cargarInsumos() {
  try {
    const insumos = await apiRequest('/Inventario/insumos');
    const tbody = document.querySelector('#insumosTable tbody');
    tbody.innerHTML = '';
    insumos.forEach(i => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${i.id}</td><td>${i.codigo}</td><td>${i.nombre}</td><td>${i.categoria ?? ''}</td><td>${i.unidadMedida ?? '-'}</td><td>${i.stockActual}</td><td>${i.stockMinimo}</td>
      <td>
        <div class="table-actions">
          <button class="btn btn-outline-primary btn-sm btn-icon" title="Ver" onclick="verInsumo(${i.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
          </button>
          <button class="btn btn-outline-info btn-sm btn-icon" title="Kardex" onclick="verKardexInsumo(${i.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line><polyline points="10 9 9 9 8 9"></polyline></svg>
          </button>
        </div>
      </td>`;
      tbody.appendChild(tr);
    });
  } catch (error) { showAlert('inventarioAlert', error.message, 'danger'); }
}

async function verInsumo(id) {
  try {
    const insumo = await apiRequest(`/Inventario/insumos/${id}`);
    showAlert('inventarioAlert', `Insumo: ${insumo.nombre} - Stock: ${insumo.stockActual} - Costo: ${money(insumo.costoReferencial)}`);
  } catch (error) {
    showAlert('inventarioAlert', error.message, 'danger');
  }
}

let kardexContexto = null;

function verKardexProducto(id) {
  kardexContexto = { tipo: 'producto', id };
  abrirKardex('Kardex de Producto Terminado');
  cargarKardexActual();
}

function verKardexInsumo(id) {
  kardexContexto = { tipo: 'insumo', id };
  abrirKardex('Kardex de Insumo');
  cargarKardexActual();
}

function abrirKardex(titulo) {
  document.getElementById('kardexModalLabel').textContent = titulo;
  document.getElementById('kardexDesde').value = '';
  document.getElementById('kardexHasta').value = '';
  const modal = new bootstrap.Modal(document.getElementById('kardexModal'));
  modal.show();
}

async function cargarKardexActual() {
  if (!kardexContexto) return;

  const desde = valueOf('kardexDesde');
  const hasta = valueOf('kardexHasta');
  const qs = new URLSearchParams();
  if (desde) qs.set('desde', desde);
  if (hasta) qs.set('hasta', hasta);
  const sufijo = qs.toString() ? `?${qs.toString()}` : '';

  const endpoint = kardexContexto.tipo === 'producto'
    ? `/Inventario/productos-terminados/kardex/${kardexContexto.id}${sufijo}`
    : `/Inventario/insumos/kardex/${kardexContexto.id}${sufijo}`;

  try {
    const kardex = await apiRequest(endpoint);
    const body = document.getElementById('kardexBody');

    if (!kardex.movimientos || kardex.movimientos.length === 0) {
      body.innerHTML = `
        <div class="text-center text-muted py-4">No hay movimientos para este período.</div>`;
      return;
    }

    const filas = kardex.movimientos.map(m => `
      <tr class="${m.tipoEfecto === 'SUMA' ? 'table-success' : m.tipoEfecto === 'RESTA' ? 'table-warning' : ''}">
        <td>${new Date(m.fecha).toLocaleString('es-GT')}</td>
        <td>${m.tipoMovimiento}</td>
        <td>${m.referencia ?? '-'}</td>
        <td>${m.motivo}</td>
        <td class="text-end text-success fw-bold">${m.entradas ? m.entradas : ''}</td>
        <td class="text-end text-danger fw-bold">${m.salidas ? m.salidas : ''}</td>
        <td class="text-end fw-bold">${m.saldo}</td>
      </tr>`).join('');

    body.innerHTML = `
      <div class="p-3 bg-light rounded-3 mb-3">
        <div class="row">
          <div class="col-md-4"><span class="text-muted small">Código:</span> <strong>${kardex.codigo}</strong></div>
          <div class="col-md-4"><span class="text-muted small">Nombre:</span> <strong>${kardex.nombre}</strong></div>
          <div class="col-md-4"><span class="text-muted small">Unidad:</span> ${kardex.unidadMedida ?? '-'}</div>
        </div>
        <div class="row mt-2">
          <div class="col-md-6"><span class="text-muted small">Stock actual:</span> <strong>${kardex.stockActual}</strong></div>
          <div class="col-md-6"><span class="text-muted small">Stock mínimo:</span> ${kardex.stockMinimo}</div>
        </div>
      </div>
      <div class="table-responsive">
        <table class="table table-sm table-bordered align-middle">
          <thead class="table-light">
            <tr>
              <th>Fecha</th>
              <th>Tipo</th>
              <th>Referencia</th>
              <th>Motivo</th>
              <th class="text-end">Entradas</th>
              <th class="text-end">Salidas</th>
              <th class="text-end">Saldo</th>
            </tr>
          </thead>
          <tbody>${filas}</tbody>
        </table>
      </div>`;
  } catch (error) {
    document.getElementById('kardexBody').innerHTML = `
      <div class="alert alert-danger mb-0">${error.message}</div>`;
  }
}

async function cargarAlertasStock() {
  try {
    const alertas = await apiRequest('/Inventario/alertas-stock');
    const body = document.getElementById('alertasStockBody');

    if (!alertas || alertas.length === 0) {
      body.innerHTML = `
        <div class="text-center text-muted py-4">
          <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mb-2 opacity-50"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>
          <div>Todos los productos e insumos tienen stock suficiente.</div>
        </div>`;
      const modal = new bootstrap.Modal(document.getElementById('alertasStockModal'));
      modal.show();
      return;
    }

    const filas = alertas.map(a => {
      const esProducto = a.tipo === 'Producto';
      const accion = esProducto
        ? `<button class="btn btn-outline-info btn-sm" onclick="verKardexProducto(${a.id})">Kardex</button>`
        : `<button class="btn btn-outline-info btn-sm" onclick="verKardexInsumo(${a.id})">Kardex</button>`;
      return `
        <tr>
          <td><span class="badge ${esProducto ? 'text-bg-primary' : 'text-bg-success'}">${a.tipo}</span></td>
          <td><strong>${a.codigo}</strong></td>
          <td>${a.nombre}</td>
          <td>${a.unidadMedida ?? '-'}</td>
          <td class="text-end text-danger fw-bold">${a.stockActual}</td>
          <td class="text-end">${a.stockMinimo}</td>
          <td class="text-end text-danger">-${a.faltante}</td>
          <td>${accion}</td>
        </tr>`;
    }).join('');

    body.innerHTML = `
      <div class="alert alert-warning py-2 small mb-3">
        Hay <strong>${alertas.length}</strong> ítems por debajo del stock mínimo.
      </div>
      <div class="table-responsive">
        <table class="table table-sm align-middle">
          <thead>
            <tr>
              <th>Tipo</th>
              <th>Código</th>
              <th>Nombre</th>
              <th>Unidad</th>
              <th class="text-end">Actual</th>
              <th class="text-end">Mínimo</th>
              <th class="text-end">Faltante</th>
              <th></th>
            </tr>
          </thead>
          <tbody>${filas}</tbody>
        </table>
      </div>`;

    const modal = new bootstrap.Modal(document.getElementById('alertasStockModal'));
    modal.show();
  } catch (error) {
    showAlert('inventarioAlert', error.message, 'danger');
  }
}
