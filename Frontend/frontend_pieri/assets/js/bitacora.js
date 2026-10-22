document.addEventListener('DOMContentLoaded', () => {
  cargarBitacora();

  document.getElementById('btnRecargarBitacora')?.addEventListener('click', cargarBitacora);

  const buscarInput = document.getElementById('buscarBitacora');
  if (buscarInput) {
    buscarInput.addEventListener('keyup', function () {
      const filter = this.value.toLowerCase();
      const rows = document.querySelectorAll('#bitacoraTable tbody tr');
      rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(filter) ? '' : 'none';
      });
    });
  }
});

async function cargarBitacora() {
  try {
    const eventos = await apiRequest('/Bitacora');
    const tbody = document.querySelector('#bitacoraTable tbody');
    tbody.innerHTML = '';

    if (!eventos || eventos.length === 0) {
      tbody.innerHTML = '<tr><td colspan="10" class="text-center text-muted py-4">No hay eventos registrados.</td></tr>';
      return;
    }

    eventos.forEach(e => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td class="text-nowrap">${formatFecha(e.fechaEvento)}</td>
        <td><span class="badge text-bg-info">${e.usuario}</span></td>
        <td>${e.modulo}</td>
        <td>${e.accion}</td>
        <td class="text-muted">${e.tablaAfectada}</td>
        <td>${e.idRegistroAfectado || '-'}</td>
        <td class="text-wrap" style="min-width:220px">${e.descripcion || '-'}</td>
        <td class="text-wrap text-muted" style="min-width:160px;max-width:240px">${e.datosAnteriores || '-'}</td>
        <td class="text-wrap text-muted" style="min-width:160px;max-width:240px">${e.datosNuevos || '-'}</td>
        <td>${e.ipEquipo || '-'}</td>
      `;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('bitacoraAlert', error.message, 'danger');
  }
}

function formatFecha(value) {
  if (!value) return '';
  const date = new Date(value);
  return date.toLocaleString('es-GT', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  });
}
