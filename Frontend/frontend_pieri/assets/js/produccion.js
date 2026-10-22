let ordenesProduccion = [];
let productosTerminados = [];
let pedidos = [];
let estadosOrden = [];
let currentOrdenId = null;

document.addEventListener('DOMContentLoaded', async () => {
  await cargarCombos();
  await cargarOrdenes();

  document.getElementById('ordenProduccionForm')?.addEventListener('submit', crearOrdenProduccion);
  document.getElementById('btnRecargarOrdenes')?.addEventListener('click', cargarOrdenes);
  document.getElementById('btnVerificarOrden')?.addEventListener('click', verificarOrden);
  document.getElementById('btnVerificarAntesProducir')?.addEventListener('click', verificarAntesProducir);
  document.getElementById('btnEjecutarProduccion')?.addEventListener('click', ejecutarProduccion);
  document.getElementById('btnConfirmarCambiarEstado')?.addEventListener('click', confirmarCambiarEstado);
});

async function cargarCombos() {
  try {
    const [productos, pedidosList, estados] = await Promise.all([
      apiRequest('/Inventario/productos-terminados').catch(() => []),
      apiRequest('/Pedidos').catch(() => []),
      apiRequest('/BOM/estados-orden-produccion').catch(() => [])
    ]);

    productosTerminados = productos || [];
    pedidos = pedidosList || [];
    estadosOrden = estados || [];

    fillSelect('ordenProducto', 
      productosTerminados.map(p => ({ 
        value: p.id, 
        label: `${p.codigo} - ${p.nombre} (Stock: ${p.stockActual})` 
      })), 
      'Seleccione un producto');

    fillSelect('ordenPedido', 
      pedidos.map(p => ({ 
        value: p.id, 
        label: `${p.numeroPedido} | ${p.cliente} (${p.estadoPedido})` 
      })), 
      'Sin pedido');

    fillSelect('cambiarEstadoSelect', 
      estadosOrden.map(e => ({ 
        value: e.id, 
        label: e.nombre 
      })), 
      'Seleccione un estado');

  } catch (error) {
    console.warn('Error al cargar combos:', error);
  }
}

async function cargarOrdenes() {
  try {
    ordenesProduccion = await apiRequest('/Produccion');
    renderizarOrdenes();
    hideAlert('produccionAlert');
  } catch (error) {
    showAlert('produccionAlert', 'Error al cargar órdenes: ' + error.message, 'danger');
  }
}

function renderizarOrdenes() {
  const tbody = document.querySelector('#ordenesTable tbody');
  if (!tbody) return;

  tbody.innerHTML = '';

  if (ordenesProduccion.length === 0) {
    tbody.innerHTML = '<tr><td colspan="9" class="text-center text-muted py-4">No hay órdenes de producción</td></tr>';
    return;
  }

  ordenesProduccion.forEach(orden => {
    const estadoClass = getEstadoClass(orden.estadoOrdenProduccion);
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${orden.id}</td>
      <td><strong>${orden.numeroOrden}</strong></td>
      <td>${orden.productoTerminado || 'N/A'}</td>
      <td><span class="badge text-bg-${estadoClass}">${orden.estadoOrdenProduccion || 'Pendiente'}</span></td>
      <td class="text-center">${orden.cantidadProgramada}</td>
      <td class="text-center text-success fw-bold">${orden.cantidadProducida}</td>
      <td class="text-center text-warning fw-bold">${orden.cantidadPendiente}</td>
      <td>${formatDate(orden.fechaCreacion)}</td>
      <td>
        <div class="table-actions">
          <button class="btn btn-outline-info btn-sm btn-icon" title="Ver detalles" onclick="verOrden(${orden.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
          </button>
          ${orden.cantidadPendiente > 0 && orden.estadoOrdenProduccion !== 'Completada' && orden.estadoOrdenProduccion !== 'Cancelada' ? `
          <button class="btn btn-outline-primary btn-sm btn-icon" title="Producir" onclick="abrirModalProducir(${orden.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>
          </button>
          ` : ''}
          ${orden.estadoOrdenProduccion !== 'Completada' && orden.estadoOrdenProduccion !== 'Cancelada' ? `
          <button class="btn btn-outline-warning btn-sm btn-icon" title="Cambiar estado" onclick="abrirModalCambiarEstado(${orden.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 4 23 10 17 10"></polyline><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"></path></svg>
          </button>
          ` : ''}
        </div>
      </td>`;
    tbody.appendChild(tr);
  });
}

function getEstadoClass(estado) {
  if (!estado) return 'secondary';
  const estadoLower = estado.toLowerCase();
  if (estadoLower.includes('pendiente')) return 'secondary';
  if (estadoLower.includes('proceso') || estadoLower.includes('producción')) return 'info';
  if (estadoLower.includes('completada')) return 'success';
  if (estadoLower.includes('cancelada')) return 'danger';
  return 'primary';
}

async function verificarOrden() {
  const productoId = Number(document.getElementById('ordenProducto')?.value);
  const cantidad = Number(document.getElementById('ordenCantidad')?.value);

  if (!productoId) {
    showAlert('produccionAlert', 'Por favor seleccione un producto terminado', 'warning');
    return;
  }

  if (!cantidad || cantidad <= 0) {
    showAlert('produccionAlert', 'Por favor ingrese una cantidad válida', 'warning');
    return;
  }

  try {
    const resultado = await apiRequest(`/BOM/verificar-disponibilidad/${productoId}?cantidad=${cantidad}`);
    
    if (resultado.length === 0) {
      showAlert('produccionAlert', 'Este producto no tiene BOM configurado. Por favor configure la lista de materiales primero.', 'warning');
      return;
    }

    let todoDisponible = true;
    let mensaje = `Verificación para producir ${cantidad} unidades:\n\n`;

    for (const item of resultado) {
      const cantidadConDesperdicio = item.cantidadRequerida * (1 + item.porcentajeDesperdicio / 100);
      const cantidadTotal = cantidadConDesperdicio * cantidad;
      const insumo = await apiRequest(`/Inventario/insumos/${item.idInsumo}`).catch(() => null);
      const stockActual = insumo?.stockActual || 0;
      const faltante = Math.max(0, cantidadTotal - stockActual);
      
      if (faltante > 0) {
        todoDisponible = false;
        mensaje += `❌ ${item.insumo}: Faltan ${faltante.toFixed(2)} (Disponible: ${stockActual.toFixed(2)})\n`;
      } else {
        mensaje += `✓ ${item.insumo}: Disponible (${stockActual.toFixed(2)})\n`;
      }
    }

    if (todoDisponible) {
      showAlert('produccionAlert', 'Todos los insumos están disponibles. Puede crear la orden de producción.', 'success');
    } else {
      showAlert('produccionAlert', mensaje, 'warning');
    }

  } catch (error) {
    showAlert('produccionAlert', 'Error al verificar: ' + error.message, 'danger');
  }
}

async function crearOrdenProduccion(e) {
  e.preventDefault();
  hideAlert('produccionAlert');

  const productoId = Number(document.getElementById('ordenProducto')?.value);
  const cantidad = Number(document.getElementById('ordenCantidad')?.value);
  const pedidoId = Number(document.getElementById('ordenPedido')?.value) || null;

  if (!productoId || !cantidad || cantidad <= 0) {
    showAlert('produccionAlert', 'Por favor complete los campos obligatorios', 'warning');
    return;
  }

  const payload = {
    idProductoTerminado: productoId,
    idPedido: pedidoId,
    cantidadProgramada: cantidad,
    observaciones: valueOf('ordenObservaciones')
  };

  try {
    await apiRequest('/Produccion', {
      method: 'POST',
      body: JSON.stringify(payload)
    });

    showAlert('produccionAlert', 'Orden de producción creada correctamente', 'success');
    document.getElementById('ordenProduccionForm')?.reset();
    await cargarOrdenes();
  } catch (error) {
    showAlert('produccionAlert', error.message, 'danger');
  }
}

async function verOrden(id) {
  const orden = ordenesProduccion.find(o => o.id === id);
  if (!orden) {
    showAlert('produccionAlert', 'Orden no encontrada', 'warning');
    return;
  }

  const estadoClass = getEstadoClass(orden.estadoOrdenProduccion);
  const body = document.getElementById('ordenDetailBody');
  
  body.innerHTML = `
    <div class="row">
      <div class="col-md-6 mb-4">
        <div class="p-3 bg-light rounded-3">
          <h6 class="text-muted text-uppercase small mb-2">Información de la Orden</h6>
          <table class="table table-sm table-borderless mb-0">
            <tr><td class="text-muted py-1" style="width: 40%;">Número:</td><td class="py-1"><strong>${orden.numeroOrden}</strong></td></tr>
            <tr><td class="text-muted py-1">Producto:</td><td class="py-1">${orden.productoTerminado || 'N/A'}</td></tr>
            <tr><td class="text-muted py-1">Estado:</td><td class="py-1"><span class="badge text-bg-${estadoClass}">${orden.estadoOrdenProduccion || 'Pendiente'}</span></td></tr>
            <tr><td class="text-muted py-1">Pedido:</td><td class="py-1">${orden.numeroPedido || 'Sin pedido'}</td></tr>
          </table>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="p-3 bg-light rounded-3">
          <h6 class="text-muted text-uppercase small mb-2">Cantidades</h6>
          <div class="row text-center">
            <div class="col-4">
              <div class="fs-4 fw-bold text-primary">${orden.cantidadProgramada}</div>
              <div class="text-muted small">Programada</div>
            </div>
            <div class="col-4">
              <div class="fs-4 fw-bold text-success">${orden.cantidadProducida}</div>
              <div class="text-muted small">Producida</div>
            </div>
            <div class="col-4">
              <div class="fs-4 fw-bold text-warning">${orden.cantidadPendiente}</div>
              <div class="text-muted small">Pendiente</div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-md-6">
        <div class="p-3 bg-light rounded-3">
          <h6 class="text-muted text-uppercase small mb-2">Fechas</h6>
          <table class="table table-sm table-borderless mb-0">
            <tr><td class="text-muted py-1" style="width: 40%;">Creación:</td><td class="py-1">${formatDate(orden.fechaCreacion)}</td></tr>
            <tr><td class="text-muted py-1">Inicio:</td><td class="py-1">${orden.fechaInicio ? formatDate(orden.fechaInicio) : '-'}</td></tr>
            <tr><td class="text-muted py-1">Fin:</td><td class="py-1">${orden.fechaFin ? formatDate(orden.fechaFin) : '-'}</td></tr>
          </table>
        </div>
      </div>
      <div class="col-md-6">
        ${orden.observaciones ? `
        <div class="p-3 bg-light rounded-3">
          <h6 class="text-muted text-uppercase small mb-2">Observaciones</h6>
          <p class="mb-0">${orden.observaciones}</p>
        </div>
        ` : ''}
      </div>
    </div>
  `;

  document.getElementById('ordenDetailNumero').textContent = orden.numeroOrden;
  const modal = new bootstrap.Modal(document.getElementById('ordenDetailModal'));
  modal.show();
}

function abrirModalProducir(id) {
  const orden = ordenesProduccion.find(o => o.id === id);
  if (!orden) return;

  currentOrdenId = id;
  document.getElementById('realizarOrdenId').value = id;
  document.getElementById('realizarOrdenInfo').value = `${orden.numeroOrden} - ${orden.productoTerminado}`;
  document.getElementById('realizarCantidadPendiente').value = `${orden.cantidadPendiente} unidades`;
  document.getElementById('realizarCantidad').value = orden.cantidadPendiente;
  document.getElementById('realizarObservaciones').value = '';

  const modal = new bootstrap.Modal(document.getElementById('realizarProduccionModal'));
  modal.show();
}

async function verificarAntesProducir() {
  const ordenId = Number(document.getElementById('realizarOrdenId')?.value);
  const cantidad = Number(document.getElementById('realizarCantidad')?.value);

  if (!ordenId || !cantidad || cantidad <= 0) {
    alert('Por favor ingrese una cantidad válida');
    return;
  }

  try {
    const resultado = await apiRequest(`/Produccion/${ordenId}/verificar-disponibilidad`);
    
    let mensaje = `Verificación para producir ${cantidad} unidades:\n\n`;
    let todoDisponible = true;

    for (const item of resultado) {
      const cantidadConDesperdicio = item.cantidadRequerida * (1 + item.porcentajeDesperdicio / 100);
      const cantidadTotal = cantidadConDesperdicio * cantidad;
      const insumo = await apiRequest(`/Inventario/insumos/${item.idInsumo}`).catch(() => null);
      const stockActual = insumo?.stockActual || 0;
      const faltante = Math.max(0, cantidadTotal - stockActual);
      
      if (faltante > 0) {
        todoDisponible = false;
        mensaje += `❌ ${item.insumo}: Faltan ${faltante.toFixed(2)}\n`;
      } else {
        mensaje += `✓ ${item.insumo}: Disponible\n`;
      }
    }

    alert(mensaje);

  } catch (error) {
    alert('Error al verificar: ' + error.message);
  }
}

async function ejecutarProduccion() {
  const ordenId = Number(document.getElementById('realizarOrdenId')?.value);
  const cantidad = Number(document.getElementById('realizarCantidad')?.value);

  if (!ordenId || !cantidad || cantidad <= 0) {
    showAlert('produccionAlert', 'Por favor ingrese una cantidad válida', 'warning');
    return;
  }

  const confirmar = confirm(`¿Está seguro de producir ${cantidad} unidades?\n\nEsta acción:\n• Descontará los insumos del inventario\n• Aumentará el stock del producto terminado`);
  
  if (!confirmar) return;

  const payload = {
    idOrdenProduccion: ordenId,
    cantidadProducir: cantidad,
    observaciones: valueOf('realizarObservaciones')
  };

  try {
    await apiRequest('/Produccion/realizar', {
      method: 'POST',
      body: JSON.stringify(payload)
    });

    bootstrap.Modal.getInstance(document.getElementById('realizarProduccionModal'))?.hide();
    showAlert('produccionAlert', `Producción ejecutada correctamente. Se produjeron ${cantidad} unidades.`, 'success');
    await cargarOrdenes();

  } catch (error) {
    showAlert('produccionAlert', error.message, 'danger');
  }
}

function abrirModalCambiarEstado(id) {
  const orden = ordenesProduccion.find(o => o.id === id);
  if (!orden) return;

  currentOrdenId = id;
  document.getElementById('cambiarEstadoOrdenId').value = id;
  document.getElementById('cambiarEstadoComentario').value = '';

  const modal = new bootstrap.Modal(document.getElementById('cambiarEstadoModal'));
  modal.show();
}

async function confirmarCambiarEstado() {
  const ordenId = Number(document.getElementById('cambiarEstadoOrdenId')?.value);
  const estadoId = Number(document.getElementById('cambiarEstadoSelect')?.value);

  if (!ordenId || !estadoId) {
    showAlert('produccionAlert', 'Por favor seleccione un estado', 'warning');
    return;
  }

  const payload = {
    idEstadoNuevo: estadoId,
    comentario: valueOf('cambiarEstadoComentario')
  };

  try {
    await apiRequest(`/Produccion/${ordenId}/estado`, {
      method: 'PATCH',
      body: JSON.stringify(payload)
    });

    bootstrap.Modal.getInstance(document.getElementById('cambiarEstadoModal'))?.hide();
    showAlert('produccionAlert', 'Estado actualizado correctamente', 'success');
    await cargarOrdenes();

  } catch (error) {
    showAlert('produccionAlert', error.message, 'danger');
  }
}
