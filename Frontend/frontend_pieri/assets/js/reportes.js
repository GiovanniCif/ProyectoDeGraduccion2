const PRIORIDADES = ['BAJA', 'MEDIA', 'ALTA'];
const PAYMENT_METHODS = [
  { id: 1, nombre: 'Efectivo' },
  { id: 2, nombre: 'Tarjeta' },
  { id: 3, nombre: 'Depósito' },
  { id: 4, nombre: 'Transferencia' }
];

document.addEventListener('DOMContentLoaded', async () => {
  await cargarCombosReportes();
  cargarSelectAnioMes();

  await Promise.all([
    cargarReportePedidos(),
    cargarReportePagos(),
    cargarReporteProductos(),
    cargarReporteInsumos(),
    cargarReporteAsistencias(),
    cargarResumenAsistencias(),
    cargarPagosCliente(),
    cargarCuentasCobrar(),
    cargarIngresos(),
    cargarPagosVencidos()
  ]);

  document.getElementById('btnFiltrarPedidos')?.addEventListener('click', cargarReportePedidos);
  document.getElementById('btnExportarPedidos')?.addEventListener('click', () => exportarReporte('pedidos', '/Reportes/pedidos/exportar', obtenerParamsPedidos()));
  document.getElementById('btnFiltrarPagos')?.addEventListener('click', cargarReportePagos);
  document.getElementById('btnExportarPagos')?.addEventListener('click', () => exportarReporte('pagos', '/Reportes/pagos/exportar', obtenerParamsPagos()));
  document.getElementById('btnFiltrarProductos')?.addEventListener('click', cargarReporteProductos);
  document.getElementById('btnExportarProductos')?.addEventListener('click', () => exportarReporte('productos', '/Reportes/productos-terminados/exportar', obtenerParamsProductos()));
  document.getElementById('btnFiltrarInsumos')?.addEventListener('click', cargarReporteInsumos);
  document.getElementById('btnExportarInsumos')?.addEventListener('click', () => exportarReporte('insumos', '/Reportes/insumos/exportar', obtenerParamsInsumos()));
  document.getElementById('btnFiltrarAsistencias')?.addEventListener('click', cargarReporteAsistencias);
  document.getElementById('btnExportarAsistencias')?.addEventListener('click', () => exportarReporte('asistencias', '/Reportes/asistencias/exportar', obtenerParamsAsistencias()));
  document.getElementById('btnFiltrarResumenAsistencias')?.addEventListener('click', cargarResumenAsistencias);
  document.getElementById('btnExportarResumenAsistencias')?.addEventListener('click', () => exportarReporte('resumenAsistencias', '/Empleados/asistencias/resumen/exportar', obtenerParamsResumenAsistencias()));
  document.getElementById('btnFiltrarPagosCliente')?.addEventListener('click', cargarPagosCliente);
  document.getElementById('btnExportarPagosCliente')?.addEventListener('click', () => exportarReporte('pagosCliente', '/Reportes/pagos-por-cliente/exportar', obtenerParamsPagosCliente()));
  document.getElementById('btnFiltrarCuentasCobrar')?.addEventListener('click', cargarCuentasCobrar);
  document.getElementById('btnExportarCuentasCobrar')?.addEventListener('click', () => exportarReporte('cuentasCobrar', '/Reportes/cuentas-por-cobrar/exportar', new URLSearchParams()));
  document.getElementById('btnFiltrarIngresos')?.addEventListener('click', cargarIngresos);
  document.getElementById('btnExportarIngresos')?.addEventListener('click', () => exportarReporte('ingresos', '/Reportes/ingresos-periodo/exportar', obtenerParamsIngresos()));
  document.getElementById('btnFiltrarPagosVencidos')?.addEventListener('click', cargarPagosVencidos);
  document.getElementById('btnExportarPagosVencidos')?.addEventListener('click', () => exportarReporte('pagosVencidos', '/Reportes/pagos-vencidos/exportar', new URLSearchParams()));
});

async function cargarCombosReportes() {
  try {
    const [estados, tiposPrenda] = await Promise.all([
      apiRequest('/Catalogos/estados-pedido').catch(() => []),
      apiRequest('/Catalogos/tipos-prenda').catch(() => [])
    ]);

    fillSelect('pedEstado', (estados || []).map(e => ({ value: e.id, label: e.nombre })), 'Todos los estados');
    fillSelect('pedPrioridad', PRIORIDADES.map(p => ({ value: p, label: p })), 'Todas las prioridades');
    fillSelect('pagMetodo', PAYMENT_METHODS.map(m => ({ value: m.id, label: m.nombre })), 'Todos los métodos');
    fillSelect('prodTipoPrenda', (tiposPrenda || []).map(t => ({ value: t.id, label: t.nombre })), 'Todos los tipos');
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

function obtenerParamsPedidos() {
  const params = new URLSearchParams();
  if (valueOf('pedDesde')) params.set('desde', valueOf('pedDesde'));
  if (valueOf('pedHasta')) params.set('hasta', valueOf('pedHasta'));
  if (valueOf('pedEstado')) params.set('idEstado', valueOf('pedEstado'));
  if (valueOf('pedPrioridad')) params.set('prioridad', valueOf('pedPrioridad'));
  return params;
}

function obtenerParamsPagos() {
  const params = new URLSearchParams();
  if (valueOf('pagDesde')) params.set('desde', valueOf('pagDesde'));
  if (valueOf('pagHasta')) params.set('hasta', valueOf('pagHasta'));
  if (valueOf('pagMetodo')) params.set('idMetodoPago', valueOf('pagMetodo'));
  return params;
}

function obtenerParamsProductos() {
  const params = new URLSearchParams();
  if (valueOf('prodTipoPrenda')) params.set('idTipoPrenda', valueOf('prodTipoPrenda'));
  if (valueOf('prodSoloStockBajo')) params.set('soloStockBajo', valueOf('prodSoloStockBajo'));
  return params;
}

function obtenerParamsInsumos() {
  const params = new URLSearchParams();
  if (valueOf('insCategoria')) params.set('categoria', valueOf('insCategoria'));
  if (valueOf('insSoloStockBajo')) params.set('soloStockBajo', valueOf('insSoloStockBajo'));
  return params;
}

function obtenerParamsAsistencias() {
  const params = new URLSearchParams();
  if (valueOf('asistDesde')) params.set('desde', valueOf('asistDesde'));
  if (valueOf('asistHasta')) params.set('hasta', valueOf('asistHasta'));
  return params;
}

function obtenerParamsResumenAsistencias() {
  const params = new URLSearchParams();
  params.set('anio', valueOf('resAnio') || new Date().getFullYear());
  params.set('mes', valueOf('resMes') || (new Date().getMonth() + 1));
  return params;
}

function obtenerParamsPagosCliente() {
  const params = new URLSearchParams();
  if (valueOf('pgcDesde')) params.set('desde', valueOf('pgcDesde'));
  if (valueOf('pgcHasta')) params.set('hasta', valueOf('pgcHasta'));
  return params;
}

function obtenerParamsIngresos() {
  const params = new URLSearchParams();
  if (valueOf('ingDesde')) params.set('desde', valueOf('ingDesde'));
  if (valueOf('ingHasta')) params.set('hasta', valueOf('ingHasta'));
  return params;
}

function cargarSelectAnioMes() {
  const ahora = new Date();
  const anioActual = ahora.getFullYear();
  const mesActual = ahora.getMonth() + 1;

  const anios = [];
  for (let a = anioActual - 4; a <= anioActual; a++) {
    anios.push({ value: a, label: a });
  }
  fillSelect('resAnio', anios);

  const meses = [
    { value: 1, label: 'Enero' }, { value: 2, label: 'Febrero' }, { value: 3, label: 'Marzo' },
    { value: 4, label: 'Abril' }, { value: 5, label: 'Mayo' }, { value: 6, label: 'Junio' },
    { value: 7, label: 'Julio' }, { value: 8, label: 'Agosto' }, { value: 9, label: 'Septiembre' },
    { value: 10, label: 'Octubre' }, { value: 11, label: 'Noviembre' }, { value: 12, label: 'Diciembre' }
  ];
  fillSelect('resMes', meses);

  document.getElementById('resAnio').value = anioActual;
  document.getElementById('resMes').value = mesActual;
}

function construirUrl(endpoint, params) {
  const qs = params.toString();
  return qs ? `${endpoint}?${qs}` : endpoint;
}

function renderVacio(tbodyId, mensaje = 'No hay registros para los filtros seleccionados.') {
  const tbody = document.querySelector(`#${tbodyId} tbody`);
  tbody.innerHTML = `<tr><td colspan="20" class="text-center text-muted py-4">${mensaje}</td></tr>`;
}

async function cargarReportePedidos() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/pedidos', obtenerParamsPedidos()));
    const tbody = document.querySelector('#pedidosTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('pedidosTable');
      return;
    }

    datos.forEach(p => {
      const tr = document.createElement('tr');
      const prioridadClass = `priority-${p.prioridad}`;
      tr.innerHTML = `
        <td>${p.id}</td>
        <td><strong>${p.numeroPedido}</strong></td>
        <td>${p.cliente}</td>
        <td><span class="badge text-bg-secondary">${p.estadoPedido}</span></td>
        <td>${formatDate(p.fechaPedido)}</td>
        <td>${formatDate(p.fechaEntrega)}</td>
        <td><span class="kanban-priority-badge ${prioridadClass}">${p.prioridad}</span></td>
        <td class="text-end fw-bold">${money(p.total)}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarReportePagos() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/pagos', obtenerParamsPagos()));
    const tbody = document.querySelector('#pagosTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('pagosTable');
      return;
    }

    datos.forEach(p => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${p.id}</td>
        <td><strong>${p.numeroPedido}</strong></td>
        <td>${p.cliente}</td>
        <td><span class="badge text-bg-info">${p.metodoPago}</span></td>
        <td>${formatDate(p.fechaPago)}</td>
        <td class="text-end fw-bold">${money(p.monto)}</td>
        <td>${p.numeroReferencia ?? '-'}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarReporteProductos() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/productos-terminados', obtenerParamsProductos()));
    const tbody = document.querySelector('#productosTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('productosTable');
      return;
    }

    datos.forEach(p => {
      const tr = document.createElement('tr');
      const stockBajo = p.stockActual < p.stockMinimo;
      tr.innerHTML = `
        <td>${p.id}</td>
        <td><strong>${p.codigo}</strong></td>
        <td>${p.nombre}</td>
        <td>${p.tipoPrenda ?? '-'}</td>
        <td>${p.talla ?? '-'}</td>
        <td>${p.color ?? '-'}</td>
        <td class="text-end ${stockBajo ? 'text-danger fw-bold' : ''}">${p.stockActual}</td>
        <td class="text-end">${p.stockMinimo}</td>
        <td class="text-end">${money(p.precioReferencial)}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarReporteInsumos() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/insumos', obtenerParamsInsumos()));
    const tbody = document.querySelector('#insumosTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('insumosTable');
      return;
    }

    datos.forEach(i => {
      const tr = document.createElement('tr');
      const stockBajo = i.stockActual < i.stockMinimo;
      tr.innerHTML = `
        <td>${i.id}</td>
        <td><strong>${i.codigo}</strong></td>
        <td>${i.nombre}</td>
        <td>${i.categoria ?? '-'}</td>
        <td>${i.unidadMedida ?? '-'}</td>
        <td class="text-end ${stockBajo ? 'text-danger fw-bold' : ''}">${i.stockActual}</td>
        <td class="text-end">${i.stockMinimo}</td>
        <td class="text-end">${money(i.costoReferencial)}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarReporteAsistencias() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/asistencias', obtenerParamsAsistencias()));
    const tbody = document.querySelector('#asistenciasTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('asistenciasTable');
      return;
    }

    datos.forEach(a => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${a.id}</td>
        <td><strong>${a.empleado}</strong></td>
        <td>${formatDate(a.fecha)}</td>
        <td>${a.horaEntrada ?? '-'}</td>
        <td>${a.horaSalida ?? '-'}</td>
        <td class="text-end ${a.tardanzaMinutos > 0 ? 'text-danger fw-bold' : ''}">${a.tardanzaMinutos}</td>
        <td class="text-end">${a.horasTrabajadas}</td>
        <td>${a.observaciones ?? '-'}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarResumenAsistencias() {
  try {
    const datos = await apiRequest(construirUrl('/Empleados/asistencias/resumen', obtenerParamsResumenAsistencias()));
    const tbody = document.querySelector('#resumenAsistenciasTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('resumenAsistenciasTable');
      return;
    }

    datos.forEach(a => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${a.idEmpleado}</td>
        <td><strong>${a.empleado}</strong></td>
        <td>${a.departamento ?? '-'}</td>
        <td>${a.puesto ?? '-'}</td>
        <td class="text-end">${a.diasHabiles}</td>
        <td class="text-end">${a.diasTrabajados}</td>
        <td class="text-end ${a.faltas > 0 ? 'text-danger fw-bold' : ''}">${a.faltas}</td>
        <td class="text-end ${a.tardanzas > 0 ? 'text-danger fw-bold' : ''}">${a.tardanzas}</td>
        <td class="text-end ${a.minutosTardanza > 0 ? 'text-danger' : ''}">${a.minutosTardanza}</td>
        <td class="text-end">${a.totalHoras}</td>
        <td class="text-end ${a.horasExtras > 0 ? 'text-success fw-bold' : ''}">${a.horasExtras}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarPagosCliente() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/pagos-por-cliente', obtenerParamsPagosCliente()));
    const tbody = document.querySelector('#pagosClienteTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('pagosClienteTable');
      return;
    }

    datos.forEach(p => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${p.idCliente}</td>
        <td><strong>${p.cliente}</strong></td>
        <td class="text-end">${p.cantidadPagos}</td>
        <td class="text-end fw-bold">${money(p.totalPagado)}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarCuentasCobrar() {
  try {
    const datos = await apiRequest('/Reportes/cuentas-por-cobrar');
    const tbody = document.querySelector('#cuentasCobrarTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('cuentasCobrarTable');
      return;
    }

    datos.forEach(c => {
      const tr = document.createElement('tr');
      const saldo = c.saldoPendiente;
      const claseEstado = saldo <= 0
        ? 'text-bg-success'
        : (c.fechaEntrega && new Date(c.fechaEntrega) < new Date() ? 'text-bg-danger' : 'text-bg-warning');
      tr.innerHTML = `
        <td>${c.idPedido}</td>
        <td><strong>${c.numeroPedido}</strong></td>
        <td>${c.cliente}</td>
        <td class="text-end">${money(c.totalPedido)}</td>
        <td class="text-end">${money(c.totalPagado)}</td>
        <td class="text-end fw-bold ${saldo > 0 ? 'text-danger' : ''}">${money(saldo)}</td>
        <td>${formatDate(c.fechaEntrega)}</td>
        <td><span class="badge ${claseEstado}">${c.estadoFinanciero}</span></td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarIngresos() {
  try {
    const datos = await apiRequest(construirUrl('/Reportes/ingresos-periodo', obtenerParamsIngresos()));
    const tbody = document.querySelector('#ingresosTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('ingresosTable');
      return;
    }

    let total = 0;
    datos.forEach(i => {
      total += i.monto;
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${formatDate(i.fecha)}</td>
        <td class="text-end">${i.cantidadPagos}</td>
        <td class="text-end fw-bold">${money(i.monto)}</td>`;
      tbody.appendChild(tr);
    });

    const trTotal = document.createElement('tr');
    trTotal.innerHTML = `
      <td colspan="2" class="text-end fw-bold">Total</td>
      <td class="text-end fw-bold">${money(total)}</td>`;
    tbody.appendChild(trTotal);
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function cargarPagosVencidos() {
  try {
    const datos = await apiRequest('/Reportes/pagos-vencidos');
    const tbody = document.querySelector('#pagosVencidosTable tbody');
    tbody.innerHTML = '';

    if (!datos || datos.length === 0) {
      renderVacio('pagosVencidosTable');
      return;
    }

    datos.forEach(p => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${p.idPedido}</td>
        <td><strong>${p.numeroPedido}</strong></td>
        <td>${p.cliente}</td>
        <td><span class="badge text-bg-secondary">${p.estadoPedido}</span></td>
        <td class="text-end">${money(p.totalPedido)}</td>
        <td class="text-end">${money(p.totalPagado)}</td>
        <td class="text-end fw-bold text-danger">${money(p.saldoPendiente)}</td>
        <td>${formatDate(p.fechaEntrega)}</td>
        <td class="text-end fw-bold text-danger">${p.diasAtraso}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  }
}

async function exportarReporte(nombre, endpoint, params) {
  const btn = document.getElementById(`btnExportar${capitalizar(nombre)}`);
  if (btn) {
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Generando...';
  }

  try {
    const url = `${API_CONFIG.baseUrl}${construirUrl(endpoint, params)}`;
    const token = getToken();

    const response = await fetch(url, {
      headers: token ? { Authorization: `Bearer ${token}` } : {}
    });

    if (!response.ok) {
      let mensaje = 'Ocurrió un error al generar el archivo.';
      try {
        const data = await response.json();
        mensaje = extraerMensajeError(data);
      } catch {
        // no json
      }
      throw new Error(mensaje);
    }

    const blob = await response.blob();
    const disposicion = response.headers.get('Content-Disposition') || '';
    const coincidencia = disposicion.match(/filename="?([^";]+)"?/);
    const nombreArchivo = coincidencia ? coincidencia[1] : `${nombre}_${new Date().toISOString().slice(0, 19).replace(/[:T]/g, '_')}.xlsx`;

    const urlDescarga = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = urlDescarga;
    a.download = nombreArchivo;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(urlDescarga);

    showAlert('reportesAlert', 'Archivo de Excel generado correctamente.');
  } catch (error) {
    showAlert('reportesAlert', error.message, 'danger');
  } finally {
    if (btn) {
      btn.disabled = false;
      btn.innerHTML = `
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="me-1"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
        Exportar Excel`;
    }
  }
}

function capitalizar(texto) {
  return texto.charAt(0).toUpperCase() + texto.slice(1);
}
