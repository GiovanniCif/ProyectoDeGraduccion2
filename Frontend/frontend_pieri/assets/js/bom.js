let currentProductoId = null;
let bomDetalles = [];
let productosTerminados = [];
let insumos = [];
let unidadesMedida = [];

document.addEventListener('DOMContentLoaded', async () => {
  await cargarCombos();
  
  document.getElementById('btnCargarBOM')?.addEventListener('click', cargarBOM);
  document.getElementById('bomForm')?.addEventListener('submit', guardarBOMDetalle);
  document.getElementById('btnRecargarBOM')?.addEventListener('click', cargarBOM);
  document.getElementById('btnVerificarDisponibilidad')?.addEventListener('click', abrirModalVerificar);
  document.getElementById('btnEjecutarVerificacion')?.addEventListener('click', ejecutarVerificacion);
  
  document.getElementById('selectProductoTerminado')?.addEventListener('change', function() {
    if (this.value) {
      currentProductoId = Number(this.value);
    }
  });
  
  document.getElementById('bomInsumo')?.addEventListener('change', actualizarCostoUnitario);
  document.getElementById('bomCantidad')?.addEventListener('input', actualizarCostoUnitario);
  document.getElementById('bomDesperdicio')?.addEventListener('input', actualizarCostoUnitario);
});

async function cargarCombos() {
  try {
    const [productos, insumosList, unidades] = await Promise.all([
      apiRequest('/Inventario/productos-terminados').catch(() => []),
      apiRequest('/Inventario/insumos').catch(() => []),
      apiRequest('/Catalogos/unidades-medida').catch(() => [])
    ]);

    productosTerminados = productos || [];
    insumos = insumosList || [];
    unidadesMedida = unidades || [];

    fillSelect('selectProductoTerminado', 
      productosTerminados.map(p => ({ 
        value: p.id, 
        label: `${p.codigo} - ${p.nombre} (Stock: ${p.stockActual})` 
      })), 
      'Seleccione un producto terminado');

    fillSelect('bomInsumo', 
      insumos.map(i => ({ 
        value: i.id, 
        label: `${i.codigo} - ${i.nombre} (Stock: ${i.stockActual})` 
      })), 
      'Seleccione un insumo');

    fillSelect('bomUnidad', 
      unidadesMedida.map(u => ({ 
        value: u.id, 
        label: `${u.nombre} (${u.abreviatura || u.nombre})` 
      })), 
      'Sin unidad');

  } catch (error) {
    showAlert('bomAlert', 'Error al cargar los combos: ' + error.message, 'danger');
  }
}

function actualizarCostoUnitario() {
  const insumoId = Number(document.getElementById('bomInsumo')?.value);
  const cantidad = Number(document.getElementById('bomCantidad')?.value || 0);
  const desperdicio = Number(document.getElementById('bomDesperdicio')?.value || 0);

  const insumo = insumos.find(i => i.id === insumoId);
  if (insumo) {
    const costoUnitario = insumo.costoReferencial || 0;
    const cantidadConDesperdicio = cantidad * (1 + desperdicio / 100);
    const costoTotal = costoUnitario * cantidadConDesperdicio;
    
    document.getElementById('bomCosto').value = money(costoTotal);
  } else {
    document.getElementById('bomCosto').value = money(0);
  }
}

async function cargarBOM() {
  const selectProducto = document.getElementById('selectProductoTerminado');
  if (!selectProducto || !selectProducto.value) {
    showAlert('bomAlert', 'Por favor seleccione un producto terminado', 'warning');
    return;
  }

  currentProductoId = Number(selectProducto.value);
  document.getElementById('bomIdProductoTerminado').value = currentProductoId;

  try {
    const detalles = await apiRequest(`/BOM/por-producto/${currentProductoId}`);
    bomDetalles = detalles || [];
    renderizarBOM();
    
    document.getElementById('productoInfoSection').style.display = 'flex';
    hideAlert('bomAlert');
  } catch (error) {
    bomDetalles = [];
    renderizarBOM();
    document.getElementById('productoInfoSection').style.display = 'flex';
    console.warn('Error al cargar BOM (puede estar vacío):', error);
  }
}

function renderizarBOM() {
  const tbody = document.querySelector('#bomTable tbody');
  if (!tbody) return;

  tbody.innerHTML = '';
  let costoTotalGeneral = 0;

  if (bomDetalles.length === 0) {
    tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-4">No hay insumos en la lista de materiales</td></tr>';
    document.getElementById('costoTotalBOM').textContent = money(0);
    return;
  }

  bomDetalles.forEach((d, index) => {
    const insumo = insumos.find(i => i.id === d.idInsumo);
    const costoUnitario = insumo?.costoReferencial || 0;
    const cantidadConDesperdicio = d.cantidadRequerida * (1 + d.porcentajeDesperdicio / 100);
    const costoTotal = costoUnitario * cantidadConDesperdicio;
    costoTotalGeneral += costoTotal;

    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${index + 1}</td>
      <td>${d.insumo || 'N/A'}</td>
      <td>${d.insumoCodigo || '-'}</td>
      <td class="text-end">${d.cantidadRequerida} ${d.unidadMedidaAbreviatura ? `(${d.unidadMedidaAbreviatura})` : ''}</td>
      <td class="text-center">${d.porcentajeDesperdicio}%</td>
      <td class="text-end">${money(costoUnitario)}</td>
      <td class="text-end fw-bold">${money(costoTotal)}</td>
      <td>
        <div class="table-actions">
          <button class="btn btn-outline-danger btn-sm btn-icon" title="Eliminar" onclick="eliminarBOMDetalle(${d.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
          </button>
        </div>
      </td>`;
    tbody.appendChild(tr);
  });

  document.getElementById('costoTotalBOM').textContent = money(costoTotalGeneral);
}

async function guardarBOMDetalle(e) {
  e.preventDefault();
  hideAlert('bomAlert');

  if (!currentProductoId) {
    showAlert('bomAlert', 'Por favor seleccione un producto terminado', 'warning');
    return;
  }

  const idInsumo = Number(document.getElementById('bomInsumo')?.value);
  if (!idInsumo) {
    showAlert('bomAlert', 'Por favor seleccione un insumo', 'warning');
    return;
  }

  const cantidad = Number(document.getElementById('bomCantidad')?.value);
  if (cantidad <= 0) {
    showAlert('bomAlert', 'La cantidad debe ser mayor a 0', 'warning');
    return;
  }

  const yaExiste = bomDetalles.some(d => d.idInsumo === idInsumo);
  if (yaExiste) {
    showAlert('bomAlert', 'Este insumo ya está en la lista de materiales', 'warning');
    return;
  }

  const payload = {
    idProductoTerminado: currentProductoId,
    idInsumo: idInsumo,
    cantidadRequerida: cantidad,
    idUnidadMedida: Number(document.getElementById('bomUnidad')?.value) || null,
    porcentajeDesperdicio: Number(document.getElementById('bomDesperdicio')?.value || 0),
    observaciones: valueOf('bomObservaciones')
  };

  try {
    await apiRequest('/BOM', {
      method: 'POST',
      body: JSON.stringify(payload)
    });

    showAlert('bomAlert', 'Insumo agregado al BOM correctamente');
    document.getElementById('bomForm')?.reset();
    document.getElementById('bomDesperdicio').value = '0';
    await cargarBOM();
  } catch (error) {
    showAlert('bomAlert', error.message, 'danger');
  }
}

async function eliminarBOMDetalle(id) {
  const confirmar = confirm('¿Está seguro de eliminar este insumo del BOM?');
  if (!confirmar) return;

  try {
    await apiRequest(`/BOM/${id}`, { method: 'DELETE' });
    showAlert('bomAlert', 'Insumo eliminado del BOM');
    await cargarBOM();
  } catch (error) {
    showAlert('bomAlert', error.message, 'danger');
  }
}

function abrirModalVerificar() {
  if (!currentProductoId) {
    showAlert('bomAlert', 'Por favor seleccione un producto y cargue su BOM', 'warning');
    return;
  }

  if (bomDetalles.length === 0) {
    showAlert('bomAlert', 'El producto no tiene insumos en su BOM', 'warning');
    return;
  }

  document.getElementById('resultadoVerificacion').classList.add('d-none');
  const modal = new bootstrap.Modal(document.getElementById('verificarStockModal'));
  modal.show();
}

async function ejecutarVerificacion() {
  const cantidad = Number(document.getElementById('cantidadVerificar')?.value);
  if (cantidad <= 0) {
    alert('Por favor ingrese una cantidad válida');
    return;
  }

  try {
    const resultado = await apiRequest(`/BOM/verificar-disponibilidad/${currentProductoId}?cantidad=${cantidad}`);
    
    const tbody = document.querySelector('#tablaVerificacion tbody');
    tbody.innerHTML = '';
    let todoDisponible = true;

    resultado.forEach(item => {
      const insumo = insumos.find(i => i.id === item.idInsumo);
      const cantidadConDesperdicio = item.cantidadRequerida * (1 + item.porcentajeDesperdicio / 100);
      const cantidadTotalNecesaria = cantidadConDesperdicio * cantidad;
      const stockActual = insumo?.stockActual || 0;
      const faltante = Math.max(0, cantidadTotalNecesaria - stockActual);
      const disponible = faltante <= 0;

      if (!disponible) todoDisponible = false;

      const tr = document.createElement('tr');
      tr.className = disponible ? 'table-success' : 'table-danger';
      tr.innerHTML = `
        <td>${item.insumo || 'N/A'}</td>
        <td class="text-end">${cantidadTotalNecesaria.toFixed(2)}</td>
        <td class="text-end">${stockActual.toFixed(2)}</td>
        <td class="text-end text-danger fw-bold">${faltante > 0 ? faltante.toFixed(2) : '-'}</td>
        <td class="text-center">
          <span class="badge ${disponible ? 'text-bg-success' : 'text-bg-danger'}">
            ${disponible ? '✓ Disponible' : '✗ Insuficiente'}
          </span>
        </td>`;
      tbody.appendChild(tr);
    });

    document.getElementById('resultadoVerificacion').classList.remove('d-none');
    
    if (todoDisponible) {
      showAlert('bomAlert', `Todos los insumos están disponibles para producir ${cantidad} unidades`, 'success');
    } else {
      showAlert('bomAlert', `Hay insumos insuficientes para producir ${cantidad} unidades. Ver detalles en el modal.`, 'warning');
    }

  } catch (error) {
    alert('Error al verificar disponibilidad: ' + error.message);
  }
}
