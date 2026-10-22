const API_CONFIG = {
  baseUrl: 'https://localhost:7020/api'
};

function getToken() {
  return localStorage.getItem('token');
}

function setToken(token) {
  localStorage.setItem('token', token);
}

function clearSession() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
}

function setUser(user) {
  localStorage.setItem('user', JSON.stringify(user));
}

function getUser() {
  const raw = localStorage.getItem('user');
  return raw ? JSON.parse(raw) : null;
}

const PAGE_PERMISSIONS = {
  'dashboard.html': ['Ver Reportes'],
  'clientes.html': ['Ver Clientes', 'Gestionar Clientes'],
  'pedidos.html': ['Ver Pedidos', 'Crear Pedidos', 'Cambiar Estado Pedido'],
  'pagos.html': ['Ver Pagos', 'Registrar Pagos'],
  'inventario.html': ['Ver Inventario', 'Gestionar Inventario'],
  'bom.html': ['Gestionar Lista de Materiales'],
  'produccion.html': ['Ver Producción', 'Gestionar Producción'],
  'empleados.html': ['Ver Empleados', 'Gestionar Empleados', 'Registrar Asistencia'],
  'usuarios.html': ['Gestionar Usuarios', 'Gestionar Roles y Permisos'],
  'reportes.html': ['Ver Reportes'],
  'bitacora.html': ['Ver Bitácora']
};

function getPermissions(token = getToken()) {
  if (!token) return [];

  try {
    const encodedPayload = token.split('.')[1];
    if (!encodedPayload) return [];

    const normalized = encodedPayload.replace(/-/g, '+').replace(/_/g, '/');
    const padding = '='.repeat((4 - normalized.length % 4) % 4);
    const payload = JSON.parse(atob(normalized + padding));
    const permissions = payload.Permiso;

    if (Array.isArray(permissions)) return permissions;
    return typeof permissions === 'string' ? [permissions] : [];
  } catch {
    return [];
  }
}

function hasAnyPermission(requiredPermissions, permissions = getPermissions()) {
  return requiredPermissions.some(permission => permissions.includes(permission));
}

function canAccessPage(pageName, permissions = getPermissions()) {
  const requiredPermissions = PAGE_PERMISSIONS[pageName];
  return !requiredPermissions || hasAnyPermission(requiredPermissions, permissions);
}

function getDefaultPage(permissions = getPermissions()) {
  return Object.keys(PAGE_PERMISSIONS).find(page => canAccessPage(page, permissions)) || 'index.html';
}

async function apiRequest(endpoint, options = {}) {
  const token = getToken();
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {})
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_CONFIG.baseUrl}${endpoint}`, {
    ...options,
    headers
  });

  let data = null;
  try {
    data = await response.json();
  } catch {
    data = null;
  }

  if (!response.ok) {
    const errorMessage = extraerMensajeError(data);
    throw new Error(errorMessage);
  }

  return data;
}

function extraerMensajeError(data) {
  if (!data) return 'Ocurrió un error en la solicitud.';

  if (data.mensaje) return data.mensaje;

  if (data.errores && Array.isArray(data.errores)) {
    return data.errores.join('. ');
  }

  if (data.errors && typeof data.errors === 'object') {
    const mensajes = Object.values(data.errors).flat().filter(Boolean);
    if (mensajes.length > 0) return mensajes.join('. ');
  }

  if (data.title) return data.title;

  if (typeof data === 'string') return data;

  return 'Ocurrió un error en la solicitud.';
}

function showAlert(elementId, message, type = 'success') {
  const alert = document.getElementById(elementId);
  if (!alert) return;

  alert.className = `alert alert-${type} alert-dismissible d-flex align-items-center`;
  alert.role = 'alert';

  const iconos = {
    success: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="me-2 flex-shrink-0"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path><polyline points="22 4 12 14.01 9 11.01"></polyline></svg>',
    danger: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="me-2 flex-shrink-0"><circle cx="12" cy="12" r="10"></circle><line x1="15" y1="9" x2="9" y2="15"></line><line x1="9" y1="9" x2="15" y2="15"></line></svg>',
    warning: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="me-2 flex-shrink-0"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>',
    info: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="me-2 flex-shrink-0"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>'
  };

  const icono = iconos[type] || '';

  alert.innerHTML = `
    ${icono}
    <div>${message}</div>
    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Cerrar"></button>
  `;

  alert.classList.remove('d-none');
}

function hideAlert(elementId) {
  const alert = document.getElementById(elementId);
  if (!alert) return;
  alert.classList.add('d-none');
}

function requireAuth() {
  const token = getToken();
  if (!token) {
    window.location.href = 'index.html';
    return false;
  }

  return true;
}


function fillSelect(elementId, options, placeholder = "Seleccione") {
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

function valueOf(elementId) {
  const el = document.getElementById(elementId);
  return el ? el.value.trim() : "";
}

function money(value) {
  return `Q${Number(value || 0).toFixed(2)}`;
}

function showConfirm(message) {
  return new Promise((resolve) => {
    const result = confirm(message);
    resolve(result);
  });
}

function parseNullableInt(value) {
  return value && value.trim() !== '' ? Number(value) : null;
}

function formatDate(value) {
  if (!value) return '';
  const date = new Date(value);
  return date.toLocaleDateString('es-GT');
}

function todayISO() {
  const now = new Date();
  const tzOffset = now.getTimezoneOffset() * 60000;
  return new Date(now - tzOffset).toISOString().slice(0, 10);
}

function fillSelectMultiple(elementId, options, placeholder = "Seleccione") {
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

function getFormData(formElement) {
  const formData = new FormData(formElement);
  const data = {};
  formData.forEach((value, key) => {
    data[key] = value;
  });
  return data;
}
