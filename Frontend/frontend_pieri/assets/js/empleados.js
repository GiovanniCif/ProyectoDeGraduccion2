const EMPLEADO_CATALOGS = {
  departamentos: [
    { id: 1, nombre: 'Administración' }, { id: 2, nombre: 'Producción' }, { id: 3, nombre: 'Bodega' }, { id: 4, nombre: 'Recursos Humanos' }
  ],
  tiposPermiso: [
    { id: 1, nombre: 'Permiso' }, { id: 2, nombre: 'Ausencia' }, { id: 3, nombre: 'Incapacidad' }, { id: 4, nombre: 'Vacaciones' }
  ]
};

let editModeEmpleado = false;
let currentEmpleadoId = null;

document.addEventListener('DOMContentLoaded', async () => {
  document.getElementById('empFechaIngreso').value = todayISO();
  document.getElementById('asisFecha').value = todayISO();
  document.getElementById('permFechaInicio').value = todayISO();
  document.getElementById('permFechaFin').value = todayISO();

  await cargarCombosEmpleados();
  await cargarEmpleados();
  await cargarAsistencias();
  await cargarPermisos();

  document.getElementById('empleadoForm')?.addEventListener('submit', guardarEmpleado);
  document.getElementById('asistenciaForm')?.addEventListener('submit', guardarAsistencia);
  document.getElementById('permisoForm')?.addEventListener('submit', guardarPermiso);
  document.getElementById('btnRecargarEmpleados')?.addEventListener('click', cargarEmpleados);
  document.getElementById('btnRecargarAsistencias')?.addEventListener('click', cargarAsistencias);
  document.getElementById('btnRecargarPermisos')?.addEventListener('click', cargarPermisos);
  document.getElementById('btnCancelarEdicionEmp')?.addEventListener('click', resetFormEmpleado);
});

async function cargarCombosEmpleados() {
  try {
    const [empleados, usuarios] = await Promise.all([
      apiRequest('/Empleados'), apiRequest('/Usuarios')
    ]);
    fillSelect('empDepartamento', EMPLEADO_CATALOGS.departamentos.map(x => ({ value: x.id, label: x.nombre })), 'Seleccione');
    fillSelect('asisEmpleado', empleados.filter(x => x.estado).map(x => ({ value: x.id, label: `${x.nombres} ${x.apellidos}` })), 'Seleccione');
    fillSelect('permEmpleado', empleados.filter(x => x.estado).map(x => ({ value: x.id, label: `${x.nombres} ${x.apellidos}` })), 'Seleccione');
    fillSelect('asisUsuario', usuarios.filter(x => x.estado).map(x => ({ value: x.id, label: x.username })), 'Seleccione');
    fillSelect('permTipo', EMPLEADO_CATALOGS.tiposPermiso.map(x => ({ value: x.id, label: x.nombre })), 'Seleccione');
    fillSelect('permAutorizadoPor', empleados.filter(x => x.estado).map(x => ({ value: `${x.nombres} ${x.apellidos}`, label: `${x.nombres} ${x.apellidos}` })), 'Seleccione autorizador');
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

function resetFormEmpleado() {
  document.getElementById('empleadoForm')?.reset();
  editModeEmpleado = false;
  currentEmpleadoId = null;
  document.getElementById('empleadoId').value = '';
  document.getElementById('empFechaIngreso').value = todayISO();
  document.getElementById('btnGuardarEmpleado').textContent = 'Guardar empleado';
  document.getElementById('btnCancelarEdicionEmp').style.display = 'none';
}

async function guardarEmpleado(e) {
  e.preventDefault();
  hideAlert('empleadosAlert');
  
  const payload = {
    idDepartamento: Number(valueOf('empDepartamento')), 
    nombres: valueOf('empNombres'), 
    apellidos: valueOf('empApellidos'),
    dpi: valueOf('empDpi'), 
    telefono: valueOf('empTelefono'), 
    correo: valueOf('empCorreo'), 
    direccion: valueOf('empDireccion'), 
    puesto: valueOf('empPuesto'),
    fechaIngreso: new Date(valueOf('empFechaIngreso')).toISOString()
  };

  try {
    if (editModeEmpleado && currentEmpleadoId) {
      await apiRequest(`/Empleados/${currentEmpleadoId}`, { 
        method: 'PUT', 
        body: JSON.stringify(payload) 
      });
      showAlert('empleadosAlert', 'Empleado actualizado correctamente.');
    } else {
      await apiRequest('/Empleados', { 
        method: 'POST', 
        body: JSON.stringify(payload) 
      });
      showAlert('empleadosAlert', 'Empleado guardado correctamente.');
    }
    
    resetFormEmpleado();
    await cargarEmpleados();
    await cargarCombosEmpleados();
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

async function guardarAsistencia(e) {
  e.preventDefault();
  hideAlert('empleadosAlert');
  const payload = {
    idEmpleado: Number(valueOf('asisEmpleado')), 
    fecha: new Date(valueOf('asisFecha')).toISOString(), 
    horaEntrada: timeValue('asisHoraEntrada'), 
    horaSalida: timeValue('asisHoraSalida'),
    observaciones: valueOf('asisObservaciones'), 
    idUsuarioRegistro: Number(valueOf('asisUsuario'))
  };
  try {
    await apiRequest('/Empleados/asistencias', { method: 'POST', body: JSON.stringify(payload) });
    showAlert('empleadosAlert', 'Asistencia registrada correctamente.');
    e.target.reset();
    document.getElementById('asisFecha').value = todayISO();
    await cargarAsistencias();
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

async function guardarPermiso(e) {
  e.preventDefault();
  hideAlert('empleadosAlert');
  const payload = {
    idEmpleado: Number(valueOf('permEmpleado')), 
    idTipoPermiso: Number(valueOf('permTipo')), 
    fechaInicio: new Date(valueOf('permFechaInicio')).toISOString(),
    fechaFin: new Date(valueOf('permFechaFin')).toISOString(), 
    motivo: valueOf('permMotivo'), 
    documentoUrl: valueOf('permDocumentoUrl'), 
    autorizadoPor: valueOf('permAutorizadoPor')
  };
  try {
    await apiRequest('/Empleados/permisos', { method: 'POST', body: JSON.stringify(payload) });
    showAlert('empleadosAlert', 'Permiso registrado correctamente.');
    e.target.reset();
    document.getElementById('permFechaInicio').value = todayISO();
    document.getElementById('permFechaFin').value = todayISO();
    await cargarPermisos();
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

async function cargarEmpleados() {
  try {
    const empleados = await apiRequest('/Empleados');
    const tbody = document.querySelector('#empleadosTable tbody');
    tbody.innerHTML = '';
    empleados.forEach(e => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${e.id}</td><td>${e.nombres} ${e.apellidos}</td><td>${e.departamento}</td><td>${e.puesto ?? ''}</td><td>${e.correo ?? ''}</td><td><span class="badge ${e.estado ? 'text-bg-success' : 'text-bg-secondary'}">${e.estado ? 'Activo' : 'Inactivo'}</span></td>
      <td>
        <div class="table-actions">
          <button class="btn btn-outline-primary btn-sm btn-icon" title="Editar" onclick="editarEmpleado(${e.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
          </button>
          <button class="btn btn-outline-${e.estado ? 'warning' : 'success'} btn-sm btn-icon" title="${e.estado ? 'Inactivar' : 'Activar'}" onclick="toggleEstadoEmpleado(${e.id})">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>
          </button>
        </div>
      </td>`;
      tbody.appendChild(tr);
    });
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

async function editarEmpleado(id) {
  try {
    const empleado = await apiRequest(`/Empleados/${id}`);
    
    editModeEmpleado = true;
    currentEmpleadoId = id;
    document.getElementById('empleadoId').value = id;
    document.getElementById('btnGuardarEmpleado').textContent = 'Actualizar empleado';
    document.getElementById('btnCancelarEdicionEmp').style.display = 'inline-block';

    document.getElementById('empDepartamento').value = empleado.idDepartamento;
    document.getElementById('empNombres').value = empleado.nombres;
    document.getElementById('empApellidos').value = empleado.apellidos;
    document.getElementById('empDpi').value = empleado.dpi ?? '';
    document.getElementById('empTelefono').value = empleado.telefono ?? '';
    document.getElementById('empCorreo').value = empleado.correo ?? '';
    document.getElementById('empDireccion').value = empleado.direccion ?? '';
    document.getElementById('empPuesto').value = empleado.puesto ?? '';
    
    if (empleado.fechaIngreso) {
      const fecha = new Date(empleado.fechaIngreso);
      const tzOffset = fecha.getTimezoneOffset() * 60000;
      document.getElementById('empFechaIngreso').value = new Date(fecha - tzOffset).toISOString().slice(0, 10);
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  } catch (error) {
    showAlert('empleadosAlert', error.message, 'danger');
  }
}

async function toggleEstadoEmpleado(id) {
  const confirmar = await showConfirm('¿Está seguro de cambiar el estado de este empleado?');
  if (!confirmar) return;

  try {
    await apiRequest(`/Empleados/${id}/estado`, {
      method: 'PATCH'
    });
    showAlert('empleadosAlert', 'Estado del empleado actualizado correctamente.');
    await cargarEmpleados();
    await cargarCombosEmpleados();
  } catch (error) {
    showAlert('empleadosAlert', error.message, 'danger');
  }
}

async function cargarAsistencias() {
  try {
    const asistencias = await apiRequest('/Empleados/asistencias');
    const tbody = document.querySelector('#asistenciasTable tbody');
    tbody.innerHTML = '';
    asistencias.forEach(a => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${a.id}</td><td>${a.empleado}</td><td>${formatDate(a.fecha)}</td><td>${a.horaEntrada ?? ''}</td><td>${a.horaSalida ?? ''}</td><td>${a.tardanzaMinutos}</td><td>${a.horasTrabajadas}</td>`;
      tbody.appendChild(tr);
    });
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

async function cargarPermisos() {
  try {
    const permisos = await apiRequest('/Empleados/permisos');
    const tbody = document.querySelector('#permisosTable tbody');
    tbody.innerHTML = '';
    permisos.forEach(p => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${p.id}</td><td>${p.empleado}</td><td>${p.tipoPermiso}</td><td>${formatDate(p.fechaInicio)}</td><td>${formatDate(p.fechaFin)}</td><td>${p.motivo}</td><td><span class="badge ${p.estado ? 'text-bg-success' : 'text-bg-secondary'}">${p.estado ? 'Activo' : 'Inactivo'}</span></td>`;
      tbody.appendChild(tr);
    });
  } catch (error) { showAlert('empleadosAlert', error.message, 'danger'); }
}

function timeValue(id) {
  const v = valueOf(id);
  return v ? `${v}:00` : null;
}
