let permisosCatalogo = [];
let permisosActuales = [];

document.addEventListener('DOMContentLoaded', () => {
  initPermisos();

  if (tienePermiso('Gestionar Usuarios')) {
    cargarRoles();
    cargarUsuarios();

    document.getElementById('usuarioForm')?.addEventListener('submit', guardarUsuario);
    document.getElementById('btnCancelar')?.addEventListener('click', resetFormulario);
    document.getElementById('btnRecargarUsuarios')?.addEventListener('click', cargarUsuarios);

    const buscarInput = document.getElementById('buscarUsuario');
    if (buscarInput) {
      buscarInput.addEventListener('keyup', function() {
        const filter = this.value.toLowerCase();
        const rows = document.querySelectorAll('#usuariosTable tbody tr');
        rows.forEach(row => {
          const text = row.textContent.toLowerCase();
          row.style.display = text.includes(filter) ? '' : 'none';
        });
      });
    }
  } else {
    document.getElementById('usuarioFormCard')?.classList.add('d-none');
    document.getElementById('usuariosListCard')?.classList.add('d-none');
  }
});

function initPermisos() {
  const puedeGestionar = tienePermiso('Gestionar Roles y Permisos');

  if (!puedeGestionar) {
    const noAutorizado = document.getElementById('permisosNoAutorizado');
    if (noAutorizado) noAutorizado.classList.remove('d-none');
    return;
  }

  const card = document.getElementById('permisosCard');
  if (card) card.classList.remove('d-none');

  document.getElementById('btnCargarPermisos')?.addEventListener('click', cargarPermisosRol);
  document.getElementById('btnGuardarPermisos')?.addEventListener('click', guardarPermisosRol);

  cargarCatalogoPermisos();
}

function tienePermiso(nombre) {
  return getPermissions().includes(nombre);
}

async function cargarCatalogoPermisos() {
  try {
    permisosCatalogo = await apiRequest('/Permisos');
    const roles = await apiRequest('/Roles');
    fillSelect('permisoRolSelect', roles.map(r => ({ value: r.id, label: r.nombre })), 'Seleccione rol');
  } catch (error) {
    showPermisoAlert(error.message, 'danger');
  }
}

async function cargarPermisosRol() {
  hidePermisoAlert();

  const rolId = valueOf('permisoRolSelect');
  if (!rolId) {
    showPermisoAlert('Debe seleccionar un rol.', 'warning');
    return;
  }

  const rol = await apiRequest(`/Roles/${rolId}`);
  const esAdmin = rol.nombre === 'Administrador';

  try {
    permisosActuales = await apiRequest(`/Roles/${rolId}/permisos`);
  } catch (error) {
    showPermisoAlert(error.message, 'danger');
    return;
  }

  const permisosActualesIds = permisosActuales.map(p => p.id);
  const grupos = {};
  permisosCatalogo.forEach(p => {
    if (!grupos[p.modulo]) grupos[p.modulo] = [];
    grupos[p.modulo].push(p);
  });

  const contenedor = document.getElementById('permisosLista');
  contenedor.innerHTML = '';

  Object.entries(grupos).forEach(([modulo, permisos]) => {
    const bloque = document.createElement('div');
    bloque.className = 'mb-3';
    bloque.innerHTML = `<h6 class="text-uppercase text-muted mb-2">${modulo}</h6>`;
    const fila = document.createElement('div');
    fila.className = 'row g-2';

    permisos.forEach(p => {
      const checked = permisosActualesIds.includes(p.id);
      const col = document.createElement('div');
      col.className = 'col-md-4 col-lg-3';
      col.innerHTML = `
        <div class="form-check form-switch border rounded-3 p-2 ps-4 mb-0 ${esAdmin ? 'opacity-50' : ''}" title="${p.descripcion ?? ''}">
          <input class="form-check-input permiso-check" type="checkbox" data-permiso-id="${p.id}" ${checked ? 'checked' : ''} ${esAdmin ? 'disabled' : ''}>
          <label class="form-check-label small">${p.nombre}</label>
        </div>
      `;
      fila.appendChild(col);
    });

    bloque.appendChild(fila);
    contenedor.appendChild(bloque);
  });

  const acciones = document.getElementById('permisosAcciones');
  if (esAdmin) {
    acciones.classList.add('d-none');
    showPermisoAlert('El rol Administrador no se puede modificar en permisos.', 'warning');
  } else {
    acciones.classList.remove('d-none');
  }

  document.getElementById('permisosVacio').classList.add('d-none');
  contenedor.classList.remove('d-none');
}

async function guardarPermisosRol() {
  hidePermisoAlert();

  const rolId = valueOf('permisoRolSelect');
  if (!rolId) return;

  const checks = document.querySelectorAll('.permiso-check:not([disabled])');
  const seleccionados = new Set();
  checks.forEach(c => {
    if (c.checked) seleccionados.add(Number(c.dataset.permisoId));
  });

  const actualesIds = new Set(permisosActuales.map(p => p.id));

  const porAsignar = [...seleccionados].filter(id => !actualesIds.has(id));
  const porQuitar = [...actualesIds].filter(id => !seleccionados.has(id));

  if (porAsignar.length === 0 && porQuitar.length === 0) {
    showPermisoAlert('No hay cambios para guardar.', 'info');
    return;
  }

  const total = porAsignar.length + porQuitar.length;
  let procesados = 0;

  try {
    for (const idPermiso of porAsignar) {
      await apiRequest(`/Roles/${rolId}/permisos`, {
        method: 'POST',
        body: JSON.stringify({ idPermiso })
      });
      procesados++;
      actualizarProgreso(total, procesados);
    }

    for (const idPermiso of porQuitar) {
      await apiRequest(`/Roles/${rolId}/permisos/${idPermiso}`, {
        method: 'DELETE'
      });
      procesados++;
      actualizarProgreso(total, procesados);
    }

    showPermisoAlert('Permisos actualizados correctamente.');
    cargarPermisosRol();
  } catch (error) {
    showPermisoAlert(error.message, 'danger');
  }
}

function actualizarProgreso(total, procesados) {
  const boton = document.getElementById('btnGuardarPermisos');
  if (boton && total > 0) {
    boton.textContent = `Guardando... (${procesados}/${total})`;
    boton.disabled = true;
  }
}

function showPermisoAlert(message, type = 'success') {
  const alert = document.getElementById('permisosAlert');
  if (!alert) return;
  alert.className = `alert alert-${type} alert-dismissible d-flex align-items-center`;
  alert.innerHTML = `
    <div>${message}</div>
    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Cerrar"></button>
  `;
  alert.classList.remove('d-none');
}

function hidePermisoAlert() {
  const alert = document.getElementById('permisosAlert');
  if (alert) alert.classList.add('d-none');
}

async function cargarRoles() {
  try {
    const roles = await apiRequest('/Roles');
    fillSelect('idRol', roles.filter(r => r.estado).map(r => ({ value: r.id, label: r.nombre })), 'Seleccione rol');
  } catch (error) {
    showAlert('usuariosAlert', error.message, 'danger');
  }
}

async function cargarUsuarios() {
  try {
    const usuarios = await apiRequest('/Usuarios');
    const tbody = document.querySelector('#usuariosTable tbody');
    tbody.innerHTML = '';

    usuarios.forEach(u => {
      const tr = document.createElement('tr');
      const estadoBadge = u.estado 
        ? '<span class="badge text-bg-success">Activo</span>' 
        : '<span class="badge text-bg-secondary">Inactivo</span>';
      
      tr.innerHTML = `
        <td>${u.id}</td>
        <td>${u.username}</td>
        <td>${u.nombres} ${u.apellidos}</td>
        <td><span class="badge text-bg-info">${u.rol}</span></td>
        <td>${u.correo}</td>
        <td>${u.telefono ?? '-'}</td>
        <td>${estadoBadge}</td>
        <td>
          <div class="d-flex gap-1">
            <button class="btn btn-sm btn-outline-primary" onclick="editarUsuario(${u.id})" title="Editar">
              Editar
            </button>
            <button class="btn btn-sm btn-outline-${u.estado ? 'warning' : 'success'}" onclick="cambiarEstadoUsuario(${u.id}, ${u.estado})" title="${u.estado ? 'Desactivar' : 'Activar'}">
              ${u.estado ? 'Desactivar' : 'Activar'}
            </button>
          </div>
        </td>
      `;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('usuariosAlert', error.message, 'danger');
  }
}

function resetFormulario() {
  document.getElementById('usuarioForm').reset();
  document.getElementById('usuarioId').value = '';
  document.getElementById('formTitulo').textContent = 'Nuevo Usuario';
  document.getElementById('btnGuardar').textContent = 'Guardar Usuario';
  
  const passwordField = document.getElementById('passwordField');
  passwordField.style.display = 'block';
  const passwordInput = document.getElementById('password');
  passwordInput.required = true;
}

async function editarUsuario(id) {
  try {
    const usuario = await apiRequest(`/Usuarios/${id}`);
    
    document.getElementById('usuarioId').value = usuario.id;
    document.getElementById('idRol').value = usuario.idRol;
    document.getElementById('nombres').value = usuario.nombres;
    document.getElementById('apellidos').value = usuario.apellidos;
    document.getElementById('username').value = usuario.username;
    document.getElementById('correo').value = usuario.correo;
    document.getElementById('telefono').value = usuario.telefono ?? '';
    
    document.getElementById('formTitulo').textContent = 'Editar Usuario';
    document.getElementById('btnGuardar').textContent = 'Actualizar Usuario';
    
    const passwordField = document.getElementById('passwordField');
    passwordField.style.display = 'none';
    const passwordInput = document.getElementById('password');
    passwordInput.required = false;
    
    window.scrollTo({ top: 0, behavior: 'smooth' });
  } catch (error) {
    showAlert('usuariosAlert', error.message, 'danger');
  }
}

async function guardarUsuario(e) {
  e.preventDefault();
  hideAlert('usuariosAlert');
  
  const usuarioId = document.getElementById('usuarioId').value;
  const isEdit = usuarioId && usuarioId !== '';
  
  const payload = {
    idRol: Number(document.getElementById('idRol').value),
    nombres: document.getElementById('nombres').value.trim(),
    apellidos: document.getElementById('apellidos').value.trim(),
    username: document.getElementById('username').value.trim(),
    correo: document.getElementById('correo').value.trim(),
    telefono: document.getElementById('telefono').value.trim() || null
  };
  
  if (!isEdit) {
    payload.password = document.getElementById('password').value;
  }
  
  try {
    if (isEdit) {
      await apiRequest(`/Usuarios/${usuarioId}`, {
        method: 'PUT',
        body: JSON.stringify(payload)
      });
      showAlert('usuariosAlert', 'Usuario actualizado correctamente.');
    } else {
      await apiRequest('/Usuarios', {
        method: 'POST',
        body: JSON.stringify(payload)
      });
      showAlert('usuariosAlert', 'Usuario creado correctamente.');
    }
    
    resetFormulario();
    cargarUsuarios();
  } catch (error) {
    showAlert('usuariosAlert', error.message, 'danger');
  }
}

async function cambiarEstadoUsuario(id, estadoActual) {
  const accion = estadoActual ? 'desactivar' : 'activar';
  const confirmar = confirm(`¿Está seguro de ${accion} este usuario?`);
  
  if (!confirmar) return;
  
  try {
    await apiRequest(`/Usuarios/${id}/estado`, {
      method: 'PATCH'
    });
    
    const mensaje = estadoActual ? 'Usuario desactivado correctamente.' : 'Usuario activado correctamente.';
    showAlert('usuariosAlert', mensaje);
    cargarUsuarios();
  } catch (error) {
    showAlert('usuariosAlert', error.message, 'danger');
  }
}
