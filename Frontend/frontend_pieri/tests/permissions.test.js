const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

function encodePayload(payload) {
  return Buffer.from(JSON.stringify(payload)).toString('base64url');
}

function loadApiHelpers() {
  const storage = new Map();
  const context = {
    atob: value => Buffer.from(value, 'base64').toString('binary'),
    fetch: async () => { throw new Error('fetch no debe ejecutarse en estas pruebas'); },
    FormData,
    localStorage: {
      getItem: key => storage.get(key) ?? null,
      setItem: (key, value) => storage.set(key, value),
      removeItem: key => storage.delete(key)
    },
    window: { location: { href: '' } },
    confirm: () => true,
    console
  };

  vm.createContext(context);
  const apiPath = path.join(__dirname, '..', 'assets', 'js', 'api.js');
  vm.runInContext(fs.readFileSync(apiPath, 'utf8'), context);
  return context;
}

test('lee correctamente un único permiso del JWT', () => {
  const api = loadApiHelpers();
  const token = `header.${encodePayload({ Permiso: 'Ver Reportes' })}.signature`;
  assert.deepEqual(Array.from(api.getPermissions(token)), ['Ver Reportes']);
});

test('lee correctamente varios permisos del JWT', () => {
  const api = loadApiHelpers();
  const token = `header.${encodePayload({ Permiso: ['Ver Clientes', 'Ver Reportes'] })}.signature`;
  assert.deepEqual(Array.from(api.getPermissions(token)), ['Ver Clientes', 'Ver Reportes']);
});

test('selecciona una página inicial permitida para cada rol', () => {
  const api = loadApiHelpers();
  assert.equal(api.getDefaultPage(['Ver Reportes']), 'dashboard.html');
  assert.equal(api.getDefaultPage(['Ver Pedidos']), 'pedidos.html');
  assert.equal(api.getDefaultPage(['Ver Inventario']), 'inventario.html');
  assert.equal(api.getDefaultPage(['Ver Empleados']), 'empleados.html');
  assert.equal(api.getDefaultPage([]), 'index.html');
});

test('Reportes y Bitácora dependen de sus permisos respectivos', () => {
  const api = loadApiHelpers();
  assert.equal(api.canAccessPage('reportes.html', ['Ver Reportes']), true);
  assert.equal(api.canAccessPage('reportes.html', ['Ver Clientes']), false);
  assert.equal(api.canAccessPage('bitacora.html', ['Ver Bitácora']), true);
  assert.equal(api.canAccessPage('bitacora.html', ['Ver Reportes']), false);
});

test('el formulario de clientes muestra Departamento antes de Municipio', () => {
  const htmlPath = path.join(__dirname, '..', 'clientes.html');
  const html = fs.readFileSync(htmlPath, 'utf8');
  assert.ok(html.indexOf('id="departamento"') < html.indexOf('id="municipio"'));
  assert.match(html, /id="municipio" disabled/);
});

test('la página de Usuarios conserva los enlaces de Reportes y Bitácora', () => {
  const htmlPath = path.join(__dirname, '..', 'usuarios.html');
  const html = fs.readFileSync(htmlPath, 'utf8');
  assert.match(html, /href="reportes\.html"/);
  assert.match(html, /href="bitacora\.html"/);
});
