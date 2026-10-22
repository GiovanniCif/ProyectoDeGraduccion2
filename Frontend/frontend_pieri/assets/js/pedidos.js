const PEDIDOS_CATALOGS = {
  estadosPedido: [
    { id: 1, nombre: 'Registrado' }, 
    { id: 2, nombre: 'En revisión' }, 
    { id: 3, nombre: 'En producción' }, 
    { id: 4, nombre: 'Listo' }, 
    { id: 5, nombre: 'Entregado' },
    { id: 6, nombre: 'Cancelado' },
    { id: 7, nombre: 'Suspendido' }
  ],
  departamentos: [
    { id: 1, nombre: 'Administración' }, 
    { id: 2, nombre: 'Producción' }, 
    { id: 3, nombre: 'Bodega' }, 
    { id: 4, nombre: 'Recursos Humanos' }
  ],
  tiposPrenda: [
    { id: 1, nombre: 'Camisa' }, 
    { id: 2, nombre: 'Pantalón' }, 
    { id: 3, nombre: 'Gabacha' }, 
    { id: 4, nombre: 'Uniforme completo' }
  ],
  tallas: [
    { id: 1, nombre: 'XS' }, 
    { id: 2, nombre: 'S' }, 
    { id: 3, nombre: 'M' }, 
    { id: 4, nombre: 'L' }, 
    { id: 5, nombre: 'XL' }
  ],
  colores: [
    { id: 1, nombre: 'Negro' }, 
    { id: 2, nombre: 'Blanco' }, 
    { id: 3, nombre: 'Azul' }, 
    { id: 4, nombre: 'Rojo' }, 
    { id: 5, nombre: 'Gris' }
  ]
};

let editModePedido = false;
let currentPedidoId = null;
let detallesPedido = [];
let currentPedidoDetalleIdCounter = 0;
let currentPedidoDetail = null;
let allPedidosData = [];
let productosTerminados = [];

document.addEventListener('DOMContentLoaded', async () => {
  document.getElementById('fechaPedido').value = todayISO();
  document.getElementById('fechaEntrega').value = todayISO();
  
  await cargarCombosPedidos();
  await cargarPedidos();
  await cargarKanban();

  document.getElementById('idCliente')?.addEventListener('change', actualizarResumenPedido);
  document.getElementById('idProductoTerminado')?.addEventListener('change', onProductoTerminadoChange);
  document.getElementById('cantidad')?.addEventListener('input', actualizarSubtotalDetalle);
  document.getElementById('precioUnitario')?.addEventListener('input', actualizarSubtotalDetalle);
  document.getElementById('descuento')?.addEventListener('input', actualizarResumenPedido);

  document.getElementById('btnAgregarDetalle')?.addEventListener('click', agregarDetalle);
  document.getElementById('pedidoForm')?.addEventListener('submit', guardarPedido);
  document.getElementById('btnRecargarPedidos')?.addEventListener('click', async () => {
    await cargarCombosPedidos();
    await cargarPedidos();
  });
  document.getElementById('btnRecargarKanban')?.addEventListener('click', cargarKanban);
  document.getElementById('btnCancelarEdicionPedido')?.addEventListener('click', resetPedidoForm);
  document.getElementById('btnVerAlertas')?.addEventListener('click', cargarAlertas);
  document.getElementById('btnRecargarAlertas')?.addEventListener('click', cargarAlertas);

  document.getElementById('tabKanbanBtn')?.addEventListener('click', () => {
    setTimeout(cargarKanban, 200);
  });

  const buscarInput = document.getElementById('buscarPedido');
  if (buscarInput) {
    buscarInput.addEventListener('keyup', function() {
      const filter = this.value.toLowerCase();
      const rows = document.querySelectorAll('#pedidosTable tbody tr');
      rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(filter) ? '' : 'none';
      });
    });
  }
});

function actualizarSubtotalDetalle() {
  const cantidad = Number(document.getElementById('cantidad').value || 0);
  const precio = Number(document.getElementById('precioUnitario').value || 0);
  const subtotal = cantidad * precio;
  document.getElementById('subtotalDetalle').textContent = money(subtotal);
}

function onProductoTerminadoChange() {
  const select = document.getElementById('idProductoTerminado');
  const idValue = select.value;

  if (!idValue) {
    document.getElementById('descripcionPrenda').value = '';
    document.getElementById('precioUnitario').value = '';
    actualizarSubtotalDetalle();
    return;
  }

  const id = Number(idValue);
  const producto = productosTerminados.find(p => p.id === id);

  if (producto) {
    if (producto.idTipoPrenda) {
      document.getElementById('idTipoPrenda').value = String(producto.idTipoPrenda);
    }
    if (producto.idTalla) {
      document.getElementById('idTalla').value = String(producto.idTalla);
    } else {
      document.getElementById('idTalla').value = '';
    }
    if (producto.idColor) {
      document.getElementById('idColor').value = String(producto.idColor);
    } else {
      document.getElementById('idColor').value = '';
    }
    document.getElementById('descripcionPrenda').value = producto.nombre || '';
    if (producto.precioReferencial && producto.precioReferencial > 0) {
      document.getElementById('precioUnitario').value = String(producto.precioReferencial);
      actualizarSubtotalDetalle();
    }
  }
}

function actualizarResumenPedido() {
  const subtotal = detallesPedido.reduce((sum, d) => sum + d.subtotal, 0);
  const descuento = Number(document.getElementById('descuento').value || 0);
  const total = subtotal - descuento;

  document.getElementById('subtotalPedido').textContent = money(subtotal);
  document.getElementById('descuentoPedido').textContent = money(descuento);
  document.getElementById('totalPedido').textContent = money(total);
}

function agregarDetalle() {
  const idProductoTerminado = parseNullableInt(document.getElementById('idProductoTerminado').value);
  const idTipoPrenda = Number(document.getElementById('idTipoPrenda').value);
  const descripcion = document.getElementById('descripcionPrenda').value.trim();
  const idTalla = parseNullableInt(document.getElementById('idTalla').value);
  const idColor = parseNullableInt(document.getElementById('idColor').value);
  const cantidad = Number(document.getElementById('cantidad').value);
  const precioUnitario = Number(document.getElementById('precioUnitario').value);
  const observaciones = document.getElementById('observacionesPedido').value.trim();

  if (!idTipoPrenda || cantidad <= 0 || precioUnitario < 0) {
    showAlert('pedidosAlert', 'Por favor complete los campos del detalle (tipo de prenda, cantidad y precio).', 'warning');
    return;
  }

  const subtotal = cantidad * precioUnitario;
  currentPedidoDetalleIdCounter++;

  const tipoPrendaNombre = PEDIDOS_CATALOGS.tiposPrenda.find(t => t.id === idTipoPrenda)?.nombre || 'N/A';
  const tallaNombre = idTalla ? PEDIDOS_CATALOGS.tallas.find(t => t.id === idTalla)?.nombre : 'N/A';
  const colorNombre = idColor ? PEDIDOS_CATALOGS.colores.find(c => c.id === idColor)?.nombre : 'N/A';
  const productoNombre = idProductoTerminado ? productosTerminados.find(p => p.id === idProductoTerminado)?.nombre : null;

  const detalle = {
    tempId: currentPedidoDetalleIdCounter,
    idProductoTerminado,
    idTipoPrenda,
    descripcionPrenda: descripcion,
    idTalla,
    idColor,
    cantidad,
    precioUnitario,
    subtotal,
    observaciones,
    tipoPrendaNombre,
    tallaNombre,
    colorNombre,
    productoNombre
  };

  detallesPedido.push(detalle);
  renderizarDetalles();
  actualizarResumenPedido();

  document.getElementById('idProductoTerminado').value = '';
  document.getElementById('descripcionPrenda').value = '';
  document.getElementById('idTalla').value = '';
  document.getElementById('idColor').value = '';
  document.getElementById('cantidad').value = '1';
  document.getElementById('precioUnitario').value = '';
  document.getElementById('subtotalDetalle').textContent = 'Q0.00';
}

function renderizarDetalles() {
  const tbody = document.querySelector('#detallesTable tbody');
  tbody.innerHTML = '';

  detallesPedido.forEach((d, index) => {
    const tr = document.createElement('tr');
    let prendaDisplay = d.tipoPrendaNombre;
    if (d.productoNombre) {
      prendaDisplay = `<strong>${d.productoNombre}</strong>`;
    }
    if (d.descripcionPrenda && !d.productoNombre) {
      prendaDisplay += ' - ' + d.descripcionPrenda;
    }
    tr.innerHTML = `
      <td>${prendaDisplay}</td>
      <td>${d.tallaNombre}</td>
      <td>${d.colorNombre}</td>
      <td>${d.cantidad}</td>
      <td>${money(d.precioUnitario)}</td>
      <td>${money(d.subtotal)}</td>
      <td>
        <button type="button" class="btn btn-outline-danger btn-sm btn-icon" onclick="eliminarDetalle(${d.tempId})">
          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
        </button>
      </td>`;
    tbody.appendChild(tr);
  });

  if (detallesPedido.length === 0) {
    tbody.innerHTML = '<tr><td colspan="7" class="text-muted text-center py-3">No hay prendas agregadas</td></tr>';
  }
}

function eliminarDetalle(tempId) {
  detallesPedido = detallesPedido.filter(d => d.tempId !== tempId);
  renderizarDetalles();
  actualizarResumenPedido();
}

function resetPedidoForm() {
  document.getElementById('pedidoForm')?.reset();
  editModePedido = false;
  currentPedidoId = null;
  detallesPedido = [];
  currentPedidoDetalleIdCounter = 0;
  
  document.getElementById('pedidoId').value = '';
  document.getElementById('fechaPedido').value = todayISO();
  document.getElementById('fechaEntrega').value = todayISO();
  document.getElementById('prioridad').value = 'MEDIA';
  document.getElementById('formTitlePedido').textContent = 'Nuevo pedido';
  document.getElementById('btnGuardarPedido').textContent = 'Guardar pedido';
  document.getElementById('btnCancelarEdicionPedido').style.display = 'none';
  
  renderizarDetalles();
  actualizarResumenPedido();
}

async function cargarCombosPedidos() {
  try {
    const [clientes, usuarios, productos, estados, departamentos] = await Promise.all([
      apiRequest('/Clientes'), 
      apiRequest('/Usuarios'),
      apiRequest('/Inventario/productos-terminados').catch(() => []),
      apiRequest('/Catalogos/estados-pedido').catch(() => PEDIDOS_CATALOGS.estadosPedido),
      apiRequest('/Catalogos/departamentos').catch(() => PEDIDOS_CATALOGS.departamentos)
    ]);

    if (estados && estados.length > 0) PEDIDOS_CATALOGS.estadosPedido = estados;
    if (departamentos && departamentos.length > 0) PEDIDOS_CATALOGS.departamentos = departamentos;

    productosTerminados = productos || [];
    
    fillSelect('idCliente', clientes.filter(c => c.estado).map(c => ({ value: c.id, label: `${c.nombreRazonSocial} (${c.tipoCliente})` })), 'Seleccione cliente');
    fillSelect('idEstadoPedido', PEDIDOS_CATALOGS.estadosPedido.map(x => ({ value: x.id, label: x.nombre })), 'Seleccione estado');
    fillSelect('idDepartamentoResponsable', PEDIDOS_CATALOGS.departamentos.map(x => ({ value: x.id, label: x.nombre })), 'Sin departamento');
    fillSelect('idUsuarioResponsable', usuarios.filter(u => u.estado).map(u => ({ value: u.id, label: `${u.username} | ${u.rol}` })), 'Sin usuario');
    
    const productoOptions = productosTerminados
      .filter(p => p.estado)
      .map(p => {
        let label = p.nombre;
        if (p.codigo) label = `${p.codigo} - ${label}`;
        if (p.stockActual !== undefined) label += ` (Stock: ${p.stockActual})`;
        return { value: p.id, label };
      });
    fillSelect('idProductoTerminado', productoOptions, 'Seleccione producto o use los campos abajo');
    
    fillSelect('idTipoPrenda', PEDIDOS_CATALOGS.tiposPrenda.map(x => ({ value: x.id, label: x.nombre })), 'Seleccione prenda');
    fillSelect('idTalla', PEDIDOS_CATALOGS.tallas.map(x => ({ value: x.id, label: x.nombre })), 'Sin talla');
    fillSelect('idColor', PEDIDOS_CATALOGS.colores.map(x => ({ value: x.id, label: x.nombre })), 'Sin color');
    document.getElementById('idEstadoPedido').value = '1';
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

async function guardarPedido(e) {
  e.preventDefault();
  hideAlert('pedidosAlert');

  if (detallesPedido.length === 0) {
    showAlert('pedidosAlert', 'Debe agregar al menos una prenda al pedido.', 'warning');
    return;
  }

  const payload = {
    idCliente: Number(document.getElementById('idCliente').value),
    idEstadoPedido: Number(document.getElementById('idEstadoPedido').value),
    idDepartamentoResponsable: parseNullableInt(document.getElementById('idDepartamentoResponsable').value),
    idUsuarioResponsable: parseNullableInt(document.getElementById('idUsuarioResponsable').value),
    fechaPedido: new Date(document.getElementById('fechaPedido').value).toISOString(),
    fechaEntrega: new Date(document.getElementById('fechaEntrega').value).toISOString(),
    prioridad: document.getElementById('prioridad').value,
    descuento: Number(document.getElementById('descuento').value || 0),
    observaciones: document.getElementById('observacionesPedido').value.trim(),
     detalles: detallesPedido.map(d => ({
       idProductoTerminado: d.idProductoTerminado,
       idTipoPrenda: d.idTipoPrenda,
       descripcionPrenda: d.descripcionPrenda,
       idTalla: d.idTalla,
       idColor: d.idColor,
       cantidad: d.cantidad,
       precioUnitario: d.precioUnitario,
       observaciones: d.observaciones
     }))
  };

  try {
    await apiRequest('/Pedidos', { method: 'POST', body: JSON.stringify(payload) });
    showAlert('pedidosAlert', 'Pedido guardado correctamente.');
    
    resetPedidoForm();
    await cargarCombosPedidos();
    await cargarPedidos();
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

async function cargarPedidos() {
  try {
    const pedidos = await apiRequest('/Pedidos');
    allPedidosData = pedidos;
    const tbody = document.querySelector('#pedidosTable tbody');
    tbody.innerHTML = '';

    pedidos.forEach(pedido => {
      const estadoColor = getEstadoColor(pedido.idEstadoPedido);
      const prioridadClass = `priority-${pedido.prioridad}`;
      
      const tr = document.createElement('tr');
      tr.style.cursor = 'pointer';
      tr.dataset.pedidoId = pedido.id;
      tr.classList.add('hover-row');
      tr.innerHTML = `
        <td>${pedido.id}</td>
        <td><strong>${pedido.numeroPedido}</strong></td>
        <td>${pedido.cliente}</td>
        <td><span class="badge text-bg-${estadoColor}">${pedido.estadoPedido}</span></td>
        <td>${formatDate(pedido.fechaPedido)}</td>
        <td>${formatDate(pedido.fechaEntrega)}</td>
        <td><span class="kanban-priority-badge ${prioridadClass}">${pedido.prioridad}</span></td>
        <td>${money(pedido.total)}</td>
        <td>
          <div class="table-actions">
            <button class="btn btn-outline-primary btn-sm btn-icon" title="Ver" onclick="event.stopPropagation(); verPedido(${pedido.id})">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
            </button>
            <button class="btn btn-outline-info btn-sm btn-icon" title="Cambiar estado" onclick="event.stopPropagation(); cambiarEstadoPedidoModal(${pedido.id}, ${pedido.idEstadoPedido})">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 4 23 10 17 10"></polyline><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"></path></svg>
            </button>
            <button class="btn btn-outline-secondary btn-sm btn-icon" title="Historial" onclick="event.stopPropagation(); verHistorialPedido(${pedido.id})">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
            </button>
          </div>
        </td>`;
      tr.addEventListener('click', () => abrirDetallePedido(pedido.id));
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

function getEstadoColor(idEstado) {
  switch(idEstado) {
    case 1: return 'primary';
    case 2: return 'warning';
    case 3: return 'info';
    case 4: return 'success';
    case 5: return 'secondary';
    case 6: return 'danger';
    case 7: return 'warning';
    default: return 'primary';
  }
}

async function verPedido(id) {
  try {
    const pedido = await apiRequest(`/Pedidos/${id}`);
    const estadoColor = getEstadoColor(pedido.idEstadoPedido);
    
    showAlert('pedidosAlert', 
      `Pedido ${pedido.numeroPedido} | Cliente: ${pedido.cliente} | Estado: ${pedido.estadoPedido} | Total: ${money(pedido.total)}`, 
      'info');
      
    document.getElementById('tabListaBtn').click();
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

async function cargarAlertas() {
  try {
    const alertas = await apiRequest('/Pedidos/alertas');
    const body = document.getElementById('pedidoAlertasBody');

    if (!alertas || alertas.length === 0) {
      body.innerHTML = `
        <div class="text-center text-muted py-4">
          <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mb-2 opacity-50"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>
          <div>No hay pedidos vencidos ni suspendidos.</div>
        </div>`;
      const modal = new bootstrap.Modal(document.getElementById('pedidoAlertasModal'));
      modal.show();
      return;
    }

    const filas = alertas.map(a => {
      const color = a.tipoAlerta === 'Vencido' ? 'danger' : 'warning';
      const estadoColor = getEstadoColor(a.idEstadoPedido);
      const atraso = a.diasAtraso > 0
        ? `<span class="badge text-bg-${a.tipoAlerta === 'Vencido' ? 'danger' : 'secondary'}">${a.diasAtraso} día(s) de atraso</span>`
        : '';
      return `
        <tr>
          <td><strong>${a.numeroPedido}</strong></td>
          <td>${a.cliente}</td>
          <td><span class="badge text-bg-${estadoColor}">${a.estadoPedido}</span></td>
          <td>${formatDate(a.fechaEntrega)}</td>
          <td>${money(a.total)}</td>
          <td><span class="badge text-bg-${color}">${a.tipoAlerta}</span> ${atraso}</td>
          <td>
            <button class="btn btn-outline-secondary btn-sm" onclick="verHistorialPedido(${a.id})">Historial</button>
          </td>
        </tr>`;
    }).join('');

    body.innerHTML = `
      <div class="alert alert-info py-2 small mb-3">
        Se encontraron <strong>${alertas.length}</strong> pedidos que requieren atención.
      </div>
      <div class="table-responsive">
        <table class="table table-sm align-middle">
          <thead>
            <tr>
              <th>Número</th>
              <th>Cliente</th>
              <th>Estado</th>
              <th>Entrega</th>
              <th>Total</th>
              <th>Alerta</th>
              <th></th>
            </tr>
          </thead>
          <tbody>${filas}</tbody>
        </table>
      </div>`;

    const modal = new bootstrap.Modal(document.getElementById('pedidoAlertasModal'));
    modal.show();
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

async function verHistorialPedido(id) {
  try {
    const historial = await apiRequest(`/Pedidos/${id}/historial`);
    const body = document.getElementById('pedidoHistorialBody');

    if (!historial || historial.length === 0) {
      body.innerHTML = '<div class="text-center text-muted py-4">Este pedido aún no tiene cambios de estado registrados.</div>';
    } else {
      const items = historial.map(h => {
        const de = h.estadoAnterior ? `<span class="badge text-bg-secondary">${h.estadoAnterior}</span>` : '<span class="badge text-bg-light text-muted">(ninguno)</span>';
        const a = `<span class="badge text-bg-${getEstadoColor(h.idEstadoNuevo)}">${h.estadoNuevo}</span>`;
        const comentario = h.comentario ? `<div class="small text-muted mt-1"><em>${h.comentario}</em></div>` : '';
        const departamento = h.departamento ? ` · <span class="small text-muted">${h.departamento}</span>` : '';
        return `
          <div class="d-flex">
            <div class="me-3 d-flex flex-column align-items-center">
              <div class="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center" style="width: 32px; height: 32px;">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
              </div>
            </div>
            <div class="flex-grow-1 pb-3">
              <div class="d-flex justify-content-between">
                <div>
                  ${de} → ${a}
                  ${departamento}
                </div>
                <span class="small text-muted text-nowrap ms-2">${new Date(h.fechaCambio).toLocaleString('es-GT')}</span>
              </div>
              <div class="small text-muted mt-1">Por: ${h.usuario || 'Desconocido'}</div>
              ${comentario}
            </div>
          </div>`;
      }).join('');

      body.innerHTML = `<div>${items}</div>`;
    }

    const numero = historial && historial.length > 0 ? historial[0].numeroPedido : `#${id}`;
    document.getElementById('pedidoHistorialModalLabel').textContent = `Historial del pedido ${numero}`;

    const modal = new bootstrap.Modal(document.getElementById('pedidoHistorialModal'));
    modal.show();
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

async function cambiarEstadoPedidoModal(id, estadoActual) {
  const estadoNombre = PEDIDOS_CATALOGS.estadosPedido.find(e => e.id === estadoActual)?.nombre || 'Desconocido';
  
  let opciones = PEDIDOS_CATALOGS.estadosPedido
    .filter(e => e.id !== estadoActual)
    .map(e => `${e.id}) ${e.nombre}`)
    .join('\n');

  const nuevoEstadoStr = prompt(
    `Estado actual: ${estadoNombre}\n\nSeleccione nuevo estado (ingrese el número):\n${opciones}`,
    ''
  );

  if (!nuevoEstadoStr) return;

  const nuevoEstado = Number(nuevoEstadoStr);
  if (!PEDIDOS_CATALOGS.estadosPedido.find(e => e.id === nuevoEstado)) {
    showAlert('pedidosAlert', 'Estado no válido.', 'warning');
    return;
  }

  const comentario = prompt('Comentario (opcional):', '');

  try {
    await apiRequest(`/Pedidos/${id}/estado`, {
      method: 'PATCH',
      body: JSON.stringify({
        idEstadoNuevo: nuevoEstado,
        comentario: comentario || null
      })
    });
    showAlert('pedidosAlert', 'Estado del pedido actualizado correctamente.');
    await cargarPedidos();
    await cargarKanban();
  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

async function cargarKanban() {
  try {
    const pedidos = await apiRequest('/Pedidos');
    
    ['col-registrado', 'col-revision', 'col-produccion', 'col-listo', 'col-entregado'].forEach(col => {
      document.getElementById(col).innerHTML = '';
    });

    document.getElementById('count-registrado').textContent = '0';
    document.getElementById('count-revision').textContent = '0';
    document.getElementById('count-produccion').textContent = '0';
    document.getElementById('count-listo').textContent = '0';
    document.getElementById('count-entregado').textContent = '0';

    const counts = { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 };

    pedidos.forEach(pedido => {
      const columna = getColumnaPorEstado(pedido.idEstadoPedido);
      const el = crearTarjetaKanban(pedido);
      document.getElementById(columna)?.appendChild(el);
      counts[pedido.idEstadoPedido]++;
    });

    document.getElementById('count-registrado').textContent = counts[1];
    document.getElementById('count-revision').textContent = counts[2];
    document.getElementById('count-produccion').textContent = counts[3];
    document.getElementById('count-listo').textContent = counts[4];
    document.getElementById('count-entregado').textContent = counts[5];

  } catch (error) {
    console.error('Error cargando kanban:', error);
  }
}

function getColumnaPorEstado(idEstado) {
  switch(idEstado) {
    case 1: return 'col-registrado';
    case 2: return 'col-revision';
    case 3: return 'col-produccion';
    case 4: return 'col-listo';
    case 5: return 'col-entregado';
    default: return 'col-registrado';
  }
}

function crearTarjetaKanban(pedido) {
  const prioridadClass = `priority-${pedido.prioridad}`;
  
  const card = document.createElement('div');
  card.className = 'kanban-card';
  card.draggable = true;
  card.dataset.pedidoId = pedido.id;
  card.dataset.estadoActual = pedido.idEstadoPedido;
  
  card.innerHTML = `
    <div class="kanban-card-id">#${pedido.numeroPedido}</div>
    <span class="kanban-priority-badge ${prioridadClass}">${pedido.prioridad}</span>
    <div class="kanban-card-title">${pedido.cliente}</div>
    <div class="kanban-card-footer">
      <span class="kanban-card-client">Entrega: ${formatDate(pedido.fechaEntrega)}</span>
      <span class="kanban-card-amount">${money(pedido.total)}</span>
    </div>`;

  card.addEventListener('dragstart', handleDragStart);
  card.addEventListener('dragend', handleDragEnd);

  return card;
}

let draggedCard = null;
let originalColumn = null;

function handleDragStart(e) {
  draggedCard = e.target;
  originalColumn = e.target.closest('.kanban-column');
  e.target.classList.add('dragging');
  e.dataTransfer.effectAllowed = 'move';
  e.dataTransfer.setData('text/plain', e.target.dataset.pedidoId);
}

function handleDragEnd(e) {
  e.target.classList.remove('dragging');
  draggedCard = null;
  originalColumn = null;
}

function handleDragOver(e) {
  e.preventDefault();
  e.dataTransfer.dropEffect = 'move';
  const column = e.target.closest('.kanban-column');
  if (column) {
    column.classList.add('drag-over');
  }
}

function handleDragLeave(e) {
  const column = e.target.closest('.kanban-column');
  if (column && !column.contains(e.relatedTarget)) {
    column.classList.remove('drag-over');
  }
}

async function handleDrop(e, nuevoEstadoId) {
  e.preventDefault();
  
  const column = e.target.closest('.kanban-column');
  if (column) {
    column.classList.remove('drag-over');
  }

  if (!draggedCard) return;

  const pedidoId = Number(draggedCard.dataset.pedidoId);
  const estadoActual = Number(draggedCard.dataset.estadoActual);

  if (estadoActual === nuevoEstadoId) {
    return;
  }

  const estadoNuevoNombre = PEDIDOS_CATALOGS.estadosPedido.find(e => e.id === nuevoEstadoId)?.nombre;
  const estadoActualNombre = PEDIDOS_CATALOGS.estadosPedido.find(e => e.id === estadoActual)?.nombre;

  const confirmar = await showConfirm(
    `¿Cambiar el estado del pedido de "${estadoActualNombre}" a "${estadoNuevoNombre}"?`
  );

  if (!confirmar) {
    return;
  }

  try {
    await apiRequest(`/Pedidos/${pedidoId}/estado`, {
      method: 'PATCH',
      body: JSON.stringify({
        idEstadoNuevo: nuevoEstadoId,
        comentario: `Cambiado de ${estadoActualNombre} a ${estadoNuevoNombre} desde tablero Kanban`
      })
    });

    showAlert('pedidosAlert', `Pedido movido a: ${estadoNuevoNombre}`);
    await cargarKanban();
    await cargarPedidos();

  } catch (error) {
    showAlert('pedidosAlert', error.message, 'danger');
  }
}

function fillSelect(elementId, options, placeholder) {
  const select = document.getElementById(elementId);
  if (!select) return;
  select.innerHTML = `<option value="">${placeholder}</option>`;
  options.forEach(option => {
    const opt = document.createElement("option");
    opt.value = option.value;
    opt.textContent = option.label;
    select.appendChild(opt);
  });
}

async function abrirDetallePedido(pedidoId) {
  try {
    const pedido = allPedidosData.find(p => p.id === pedidoId);
    if (!pedido) {
      const pedidoFull = await apiRequest(`/Pedidos/${pedidoId}`);
      currentPedidoDetail = pedidoFull;
    } else {
      currentPedidoDetail = pedido;
    }

    if (!currentPedidoDetail) {
      showAlert('pedidosAlert', 'No se encontró el pedido seleccionado', 'warning');
      return;
    }

    const p = currentPedidoDetail;
    const estadoColor = getEstadoColor(p.idEstadoPedido);
    const departamentoNombre = p.idDepartamentoResponsable 
      ? PEDIDOS_CATALOGS.departamentos.find(d => d.id === p.idDepartamentoResponsable)?.nombre || 'N/A'
      : 'Sin departamento';

    let detallesHtml = '';
    if (p.detalles && p.detalles.length > 0) {
      detallesHtml = p.detalles.map((d, i) => {
        let tipoDisplay = d.tipoPrenda || PEDIDOS_CATALOGS.tiposPrenda.find(t => t.id === d.idTipoPrenda)?.nombre || 'N/A';
        if (d.productoTerminado) {
          tipoDisplay = `<strong>${d.productoTerminado}</strong>`;
        }
        const talla = d.talla || (d.idTalla ? PEDIDOS_CATALOGS.tallas.find(t => t.id === d.idTalla)?.nombre : 'N/A');
        const color = d.color || (d.idColor ? PEDIDOS_CATALOGS.colores.find(c => c.id === d.idColor)?.nombre : 'N/A');
        
        return `
          <tr>
            <td>${i + 1}</td>
            <td>${tipoDisplay}</td>
            <td>${d.descripcionPrenda ?? ''}</td>
            <td>${talla}</td>
            <td>${color}</td>
            <td class="text-center">${d.cantidad}</td>
            <td class="text-end">${money(d.precioUnitario)}</td>
            <td class="text-end"><strong>${money(d.subtotal ?? (d.cantidad * d.precioUnitario))}</strong></td>
          </tr>`;
      }).join('');
    } else {
      detallesHtml = '<tr><td colspan="8" class="text-center text-muted py-3">No hay detalles de prendas</td></tr>';
    }

    let subtotalCalc = 0;
    if (p.detalles && p.detalles.length > 0) {
      subtotalCalc = p.detalles.reduce((sum, d) => sum + (d.subtotal ?? (d.cantidad * d.precioUnitario)), 0);
    } else {
      subtotalCalc = p.total + (p.descuento || 0);
    }

    const body = document.getElementById('pedidoDetailBody');
    body.innerHTML = `
      <div class="row">
        <div class="col-md-6 mb-4">
          <div class="p-3 bg-light rounded-3">
            <h6 class="text-muted text-uppercase small mb-2">Información del Pedido</h6>
            <table class="table table-sm table-borderless mb-0">
              <tr><td class="text-muted py-1" style="width: 35%;">Número:</td><td class="py-1"><strong>${p.numeroPedido}</strong></td></tr>
              <tr><td class="text-muted py-1">Estado:</td><td class="py-1"><span class="badge text-bg-${estadoColor}">${p.estadoPedido}</span></td></tr>
              <tr><td class="text-muted py-1">Cliente:</td><td class="py-1">${p.cliente}</td></tr>
              <tr><td class="text-muted py-1">Prioridad:</td><td class="py-1"><span class="kanban-priority-badge priority-${p.prioridad}">${p.prioridad}</span></td></tr>
            </table>
          </div>
        </div>

        <div class="col-md-6 mb-4">
          <div class="p-3 bg-light rounded-3">
            <h6 class="text-muted text-uppercase small mb-2">Fechas y Responsable</h6>
            <table class="table table-sm table-borderless mb-0">
              <tr><td class="text-muted py-1" style="width: 35%;">Fecha pedido:</td><td class="py-1">${formatDate(p.fechaPedido)}</td></tr>
              <tr><td class="text-muted py-1">Fecha entrega:</td><td class="py-1">${formatDate(p.fechaEntrega)}</td></tr>
              <tr><td class="text-muted py-1">Departamento:</td><td class="py-1">${departamentoNombre}</td></tr>
              <tr><td class="text-muted py-1">Estado:</td><td class="py-1"><span class="badge ${p.estado ? 'text-bg-success' : 'text-bg-secondary'}">${p.estado ? 'Activo' : 'Inactivo'}</span></td></tr>
            </table>
          </div>
        </div>
      </div>

      <div class="mb-4">
        <h6 class="text-muted text-uppercase small mb-2">Resumen Financiero</h6>
        <div class="row">
          <div class="col-md-4">
            <div class="p-3 bg-light rounded-3 text-center">
              <div class="text-muted small">Subtotal</div>
              <div class="fs-5 fw-bold">${money(subtotalCalc)}</div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="p-3 bg-light rounded-3 text-center">
              <div class="text-muted small">Descuento</div>
              <div class="fs-5 fw-bold text-danger">${money(p.descuento || 0)}</div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="p-3 rounded-3 text-center" style="background: linear-gradient(135deg, #7B1114 0%, #D96B10 100%); color: white;">
              <div class="opacity-75 small">Total</div>
              <div class="fs-5 fw-bold">${money(p.total)}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="mb-4">
        <h6 class="text-muted text-uppercase small mb-2">Detalle de Prendas</h6>
        <div class="table-responsive">
          <table class="table table-sm">
            <thead>
              <tr>
                <th>#</th>
                <th>Tipo</th>
                <th>Descripción</th>
                <th>Talla</th>
                <th>Color</th>
                <th class="text-center">Cant</th>
                <th class="text-end">P.Unit</th>
                <th class="text-end">Subtotal</th>
              </tr>
            </thead>
            <tbody>
              ${detallesHtml}
            </tbody>
          </table>
        </div>
      </div>

      ${p.observaciones ? `
      <div class="mb-3">
        <h6 class="text-muted text-uppercase small mb-2">Observaciones</h6>
        <div class="p-3 bg-light rounded-3">
          <p class="mb-0">${p.observaciones}</p>
        </div>
      </div>
      ` : ''}
    `;

    const modal = new bootstrap.Modal(document.getElementById('pedidoDetailModal'));
    modal.show();
  } catch (error) {
    showAlert('pedidosAlert', `Error al cargar detalle: ${error.message}`, 'danger');
  }
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('btnDescargarPDFPedido')?.addEventListener('click', descargarPDFPedido);
});

async function descargarPDFPedido() {
  if (!currentPedidoDetail) {
    showAlert('pedidosAlert', 'No hay pedido seleccionado para descargar', 'warning');
    return;
  }

  try {
    showAlert('pedidosAlert', 'Generando PDF...', 'info');
    
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();
    
    const pageWidth = doc.internal.pageSize.getWidth();
    let yPos = 20;

    try {
      const logoImg = new Image();
      logoImg.crossOrigin = 'anonymous';
      
      await new Promise((resolve) => {
        logoImg.onload = resolve;
        logoImg.onerror = resolve;
        logoImg.src = 'assets/img/logo.png';
      });

      if (logoImg.complete && logoImg.naturalWidth > 0) {
        const imgWidth = 35;
        const imgHeight = (logoImg.naturalHeight / logoImg.naturalWidth) * imgWidth;
        doc.addImage(logoImg, 'PNG', 15, 15, imgWidth, imgHeight);
        yPos = 15 + imgHeight + 10;
      }
    } catch (logoError) {
      console.warn('No se pudo cargar el logo:', logoError);
    }

    doc.setFontSize(20);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(123, 17, 20);
    doc.text('Pedido', pageWidth / 2, yPos, { align: 'center' });
    
    yPos += 5;
    doc.setDrawColor(217, 107, 16);
    doc.setLineWidth(0.5);
    doc.line(20, yPos, pageWidth - 20, yPos);
    yPos += 10;

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(100, 100, 100);
    doc.text(`Fecha de emisión: ${new Date().toLocaleDateString('es-GT')}`, pageWidth - 20, 15, { align: 'right' });
    doc.text(`PIERI D'CACIO`, pageWidth - 20, 21, { align: 'right' });

    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(60, 60, 60);
    doc.text('Información del Pedido', 20, yPos);
    yPos += 7;

    doc.setFillColor(245, 245, 245);
    doc.roundedRect(20, yPos, pageWidth - 40, 30, 3, 3, 'F');
    
    yPos += 7;
    const p = currentPedidoDetail;
    
    doc.setFontSize(10);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(80, 80, 80);
    doc.text('Número:', 25, yPos);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(40, 40, 40);
    doc.text(p.numeroPedido, 60, yPos);
    
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(80, 80, 80);
    doc.text('Estado:', 110, yPos);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(40, 40, 40);
    doc.text(p.estadoPedido, 140, yPos);
    
    yPos += 7;
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(80, 80, 80);
    doc.text('Cliente:', 25, yPos);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(40, 40, 40);
    doc.text(p.cliente.length > 30 ? p.cliente.substring(0, 30) + '...' : p.cliente, 60, yPos);
    
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(80, 80, 80);
    doc.text('Prioridad:', 110, yPos);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(40, 40, 40);
    doc.text(p.prioridad, 140, yPos);
    
    yPos += 7;
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(80, 80, 80);
    doc.text('Fecha pedido:', 25, yPos);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(40, 40, 40);
    doc.text(formatDate(p.fechaPedido), 60, yPos);
    
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(80, 80, 80);
    doc.text('Fecha entrega:', 110, yPos);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(40, 40, 40);
    doc.text(formatDate(p.fechaEntrega), 140, yPos);

    yPos += 10;
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(60, 60, 60);
    doc.text('Resumen Financiero', 20, yPos);
    yPos += 7;

    let subtotalCalc = 0;
    if (p.detalles && p.detalles.length > 0) {
      subtotalCalc = p.detalles.reduce((sum, d) => sum + (d.subtotal ?? (d.cantidad * d.precioUnitario)), 0);
    } else {
      subtotalCalc = p.total + (p.descuento || 0);
    }

    const colWidth = (pageWidth - 50) / 3;
    
    doc.setFillColor(245, 245, 245);
    doc.roundedRect(25, yPos, colWidth, 18, 2, 2, 'F');
    doc.roundedRect(25 + colWidth + 5, yPos, colWidth, 18, 2, 2, 'F');
    doc.setFillColor(123, 17, 20);
    doc.setTextColor(255, 255, 255);
    doc.roundedRect(25 + (colWidth + 5) * 2, yPos, colWidth, 18, 2, 2, 'F');

    doc.setFontSize(9);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(100, 100, 100);
    doc.text('Subtotal', 25 + colWidth/2, yPos + 6, { align: 'center' });
    doc.text('Descuento', 25 + colWidth/2 + colWidth + 5, yPos + 6, { align: 'center' });
    doc.setTextColor(255, 255, 255);
    doc.text('Total', 25 + colWidth/2 + (colWidth + 5) * 2, yPos + 6, { align: 'center' });

    doc.setFontSize(14);
    doc.setFont('helvetica', 'bold');
    doc.setTextColor(40, 40, 40);
    doc.text(money(subtotalCalc), 25 + colWidth/2, yPos + 13, { align: 'center' });
    doc.setTextColor(163, 32, 32);
    doc.text(money(p.descuento || 0), 25 + colWidth/2 + colWidth + 5, yPos + 13, { align: 'center' });
    doc.setTextColor(255, 255, 255);
    doc.text(money(p.total), 25 + colWidth/2 + (colWidth + 5) * 2, yPos + 13, { align: 'center' });

    yPos += 25;

    if (p.detalles && p.detalles.length > 0) {
      doc.setFontSize(12);
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(60, 60, 60);
      doc.text('Detalle de Prendas', 20, yPos);
      yPos += 7;

      doc.setFillColor(230, 230, 230);
      doc.roundedRect(20, yPos, pageWidth - 40, 7, 2, 2, 'F');
      
      yPos += 5;
      doc.setFontSize(8);
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(60, 60, 60);
      doc.text('#', 25, yPos);
      doc.text('Tipo', 35, yPos);
      doc.text('Talla', 70, yPos);
      doc.text('Color', 90, yPos);
      doc.text('Cant', 110, yPos, { align: 'center' });
      doc.text('P.Unit', 130, yPos, { align: 'right' });
      doc.text('Subtotal', 165, yPos, { align: 'right' });

      yPos += 4;
      doc.setDrawColor(200, 200, 200);
      doc.setLineWidth(0.3);
      doc.line(20, yPos, pageWidth - 20, yPos);
      yPos += 5;

       p.detalles.forEach((d, i) => {
         if (yPos > 260) {
           doc.addPage();
           yPos = 20;
         }

         let tipoPrenda = d.tipoPrenda || PEDIDOS_CATALOGS.tiposPrenda.find(t => t.id === d.idTipoPrenda)?.nombre || 'N/A';
         if (d.productoTerminado) {
           tipoPrenda = d.productoTerminado;
         }
         const talla = d.talla || (d.idTalla ? PEDIDOS_CATALOGS.tallas.find(t => t.id === d.idTalla)?.nombre : 'N/A');
         const color = d.color || (d.idColor ? PEDIDOS_CATALOGS.colores.find(c => c.id === d.idColor)?.nombre : 'N/A');
         const subtotal = d.subtotal ?? (d.cantidad * d.precioUnitario);

        doc.setFontSize(8);
        doc.setFont('helvetica', 'normal');
        doc.setTextColor(40, 40, 40);
        doc.text(String(i + 1), 25, yPos);
        doc.text(tipoPrenda.length > 20 ? tipoPrenda.substring(0, 20) + '...' : tipoPrenda, 35, yPos);
        doc.text(talla, 70, yPos);
        doc.text(color, 90, yPos);
        doc.text(String(d.cantidad), 110, yPos, { align: 'center' });
        doc.text(money(d.precioUnitario), 130, yPos, { align: 'right' });
        doc.setFont('helvetica', 'bold');
        doc.text(money(subtotal), 165, yPos, { align: 'right' });

        yPos += 6;
      });
    }

    yPos += 5;
    if (yPos > 260) {
      doc.addPage();
      yPos = 20;
    }

    if (p.observaciones) {
      doc.setFontSize(10);
      doc.setFont('helvetica', 'bold');
      doc.setTextColor(60, 60, 60);
      doc.text('Observaciones', 20, yPos);
      yPos += 5;

      doc.setFillColor(245, 245, 245);
      doc.roundedRect(20, yPos, pageWidth - 40, 15, 2, 2, 'F');
      
      yPos += 7;
      doc.setFontSize(9);
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(40, 40, 40);
      
      const obsLines = doc.splitTextToSize(p.observaciones, pageWidth - 50);
      doc.text(obsLines, 25, yPos);
    }

    const pageHeight = doc.internal.pageSize.getHeight();
    yPos = pageHeight - 30;
    doc.setDrawColor(200, 200, 200);
    doc.setLineWidth(0.3);
    doc.line(20, yPos, pageWidth - 20, yPos);
    
    yPos += 5;
    doc.setFontSize(8);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(120, 120, 120);
    doc.text('PIERI D\'CACIO - Sistema de Gestión Empresarial', pageWidth / 2, yPos, { align: 'center' });
    
    yPos += 4;
    doc.text(`Documento generado: ${new Date().toLocaleString('es-GT')}`, pageWidth / 2, yPos, { align: 'center' });

    const fileName = `Pedido_${p.numeroPedido}.pdf`;
    doc.save(fileName);
    
    hideAlert('pedidosAlert');
    showAlert('pedidosAlert', 'PDF generado correctamente.', 'success');
    
  } catch (error) {
    console.error('Error generando PDF:', error);
    showAlert('pedidosAlert', `Error al generar el PDF: ${error.message}`, 'danger');
  }
}
