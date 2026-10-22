const GUATEMALA_DATA = {
  departamentos: [
    'Alta Verapaz', 'Baja Verapaz', 'Chimaltenango', 'Chiquimula', 'El Progreso', 'Escuintla',
    'Guatemala', 'Huehuetenango', 'Izabal', 'Jalapa', 'Jutiapa', 'Petén', 'Quetzaltenango',
    'Quiché', 'Retalhuleu', 'Sacatepéquez', 'San Marcos', 'Santa Rosa', 'Sololá', 'Suchitepéquez',
    'Totonicapán', 'Zacapa'
  ],
  municipios: {
    'Alta Verapaz': ['Cobán', 'Santa Cruz Verapaz', 'San Cristóbal Verapaz', 'Tactic', 'Tamahú', 'Tucurú', 'Panzós', 'Senahú', 'San Pedro Carchá', 'San Juan Chamelco', 'Lanquín', 'Cahabón', 'Chisec', 'Chahal', 'Fray Bartolomé de las Casas', 'Santa Catalina La Tinta', 'Raxruhá'],
    'Baja Verapaz': ['Salamá', 'San Miguel Chicaj', 'Rabinal', 'Cubulco', 'Granados', 'Santa Cruz El Chol', 'San Jerónimo', 'Purulhá'],
    'Chimaltenango': ['Chimaltenango', 'San José Poaquil', 'San Martín Jilotepeque', 'San Juan Comalapa', 'Santa Apolonia', 'Tecpán Guatemala', 'Patzún', 'Pochuta', 'Patzicía', 'Santa Cruz Balanyá', 'Acatenango', 'Yepocapa', 'San Andrés Itzapa', 'Parramos', 'Zaragoza', 'El Tejar'],
    'Chiquimula': ['Chiquimula', 'San José La Arada', 'San Juan Ermita', 'Jocotán', 'Camotán', 'Olopa', 'Esquipulas', 'Concepción Las Minas', 'Quezaltepeque', 'San Jacinto', 'Ipala'],
    'El Progreso': ['Guastatoya', 'Morazán', 'San Agustín Acasaguastlán', 'San Cristóbal Acasaguastlán', 'El Jícaro', 'Sansare', 'Sanarate', 'San Antonio La Paz'],
    'Escuintla': ['Escuintla', 'Santa Lucía Cotzumalguapa', 'La Democracia', 'Siquinalá', 'Masagua', 'Tiquisate', 'La Gomera', 'Guanagazapa', 'San José', 'Iztapa', 'Palín', 'San Vicente Pacaya', 'Nueva Concepción', 'Sipacate'],
    'Guatemala': ['Guatemala', 'Santa Catarina Pinula', 'San José Pinula', 'San José del Golfo', 'Palencia', 'Chinautla', 'San Pedro Ayampuc', 'Mixco', 'San Pedro Sacatepéquez', 'San Juan Sacatepéquez', 'San Raymundo', 'Chuarrancho', 'Fraijanes', 'Amatitlán', 'Villa Nueva', 'Villa Canales', 'San Miguel Petapa'],
    'Huehuetenango': ['Huehuetenango', 'Chiantla', 'Malacatancito', 'Cuilco', 'Nentón', 'San Pedro Necta', 'Jacaltenango', 'San Pedro Soloma', 'San Ildefonso Ixtahuacán', 'Santa Bárbara', 'La Libertad', 'La Democracia', 'San Miguel Acatán', 'San Rafael La Independencia', 'Todos Santos Cuchumatán', 'San Juan Atitán', 'Santa Eulalia', 'San Mateo Ixtatán', 'Colotenango', 'San Sebastián Huehuetenango', 'Tectitán', 'Concepción Huista', 'San Juan Ixcoy', 'San Antonio Huista', 'San Sebastián Coatán', 'Santa Cruz Barillas', 'Aguacatán', 'San Rafael Petzal', 'San Gaspar Ixchil', 'Santiago Chimaltenango', 'Santa Ana Huista', 'Unión Cantinil', 'Petatán'],
    'Izabal': ['Puerto Barrios', 'Livingston', 'El Estor', 'Morales', 'Los Amates'],
    'Jalapa': ['Jalapa', 'San Pedro Pinula', 'San Luis Jilotepeque', 'San Manuel Chaparrón', 'San Carlos Alzatate', 'Monjas', 'Mataquescuintla'],
    'Jutiapa': ['Jutiapa', 'El Progreso', 'Santa Catarina Mita', 'Agua Blanca', 'Asunción Mita', 'Yupiltepeque', 'Atescatempa', 'Jerez', 'El Adelanto', 'Zapotitlán', 'Comapa', 'Jalpatagua', 'Conguaco', 'Moyuta', 'Pasaco', 'San José Acatempa', 'Quesada'],
    'Petén': ['Flores', 'San José', 'San Benito', 'San Andrés', 'La Libertad', 'San Francisco', 'Santa Ana', 'Dolores', 'San Luis', 'Sayaxché', 'Melchor de Mencos', 'Poptún', 'Las Cruces', 'El Chal'],
    'Quetzaltenango': ['Quetzaltenango', 'Salcajá', 'Olintepeque', 'San Carlos Sija', 'Sibilia', 'Cabricán', 'Cajolá', 'San Miguel Sigüilá', 'San Juan Ostuncalco', 'San Mateo', 'Concepción Chiquirichapa', 'San Martín Sacatepéquez', 'Almolonga', 'Cantel', 'Huitán', 'Zunil', 'Colomba', 'San Francisco La Unión', 'El Palmar', 'Coatepeque', 'Génova', 'Flores Costa Cuca', 'La Esperanza', 'Palestina de Los Altos'],
    'Quiché': ['Santa Cruz del Quiché', 'Chiché', 'Chinique', 'Zacualpa', 'Chajul', 'Chichicastenango', 'Patzité', 'San Antonio Ilotenango', 'San Pedro Jocopilas', 'Cunén', 'San Juan Cotzal', 'Joyabaj', 'Nebaj', 'San Andrés Sajcabajá', 'Uspantán', 'Sacapulas', 'San Bartolomé Jocotenango', 'Canillá', 'Chicamán', 'Ixcán', 'Pachalum'],
    'Retalhuleu': ['Retalhuleu', 'San Sebastián', 'Santa Cruz Muluá', 'San Martín Zapotitlán', 'San Felipe', 'San Andrés Villa Seca', 'Champerico', 'Nuevo San Carlos', 'El Asintal'],
    'Sacatepéquez': ['Antigua Guatemala', 'Jocotenango', 'Pastores', 'Sumpango', 'Santo Domingo Xenacoj', 'Santiago Sacatepéquez', 'San Bartolomé Milpas Altas', 'San Lucas Sacatepéquez', 'Santa Lucía Milpas Altas', 'Magdalena Milpas Altas', 'Santa María de Jesús', 'Ciudad Vieja', 'San Miguel Dueñas', 'Alotenango', 'San Antonio Aguas Calientes', 'Santa Catarina Barahona'],
    'San Marcos': ['San Marcos', 'San Pedro Sacatepéquez', 'San Antonio Sacatepéquez', 'Comitancillo', 'San Miguel Ixtahuacán', 'Concepción Tutuapa', 'Tacaná', 'Sibinal', 'Tajumulco', 'Tejutla', 'San Rafael Pie de la Cuesta', 'Nuevo Progreso', 'El Tumbador', 'El Rodeo', 'Malacatán', 'Catarina', 'Ayutla', 'Ocós', 'San Pablo', 'El Quetzal', 'La Reforma', 'Pajapita', 'Ixchiguán', 'San José Ojetenam', 'San Cristóbal Cucho', 'Sipacapa', 'Esquipulas Palo Gordo', 'Río Blanco', 'San Lorenzo', 'La Blanca'],
    'Santa Rosa': ['Cuilapa', 'Barberena', 'Santa Rosa de Lima', 'Casillas', 'San Rafael Las Flores', 'Oratorio', 'San Juan Tecuaco', 'Chiquimulilla', 'Taxisco', 'Santa María Ixhuatán', 'Guazacapán', 'Santa Cruz Naranjo', 'Pueblo Nuevo Viñas', 'Nueva Santa Rosa'],
    'Sololá': ['Sololá', 'San José Chacayá', 'Santa María Visitación', 'Santa Lucía Utatlán', 'Nahualá', 'Santa Catarina Ixtahuacán', 'Santa Clara La Laguna', 'Concepción', 'San Andrés Semetabaj', 'Panajachel', 'Santa Catarina Palopó', 'San Antonio Palopó', 'San Lucas Tolimán', 'Santa Cruz La Laguna', 'San Pablo La Laguna', 'San Marcos La Laguna', 'San Juan La Laguna', 'San Pedro La Laguna', 'Santiago Atitlán'],
    'Suchitepéquez': ['Mazatenango', 'Cuyotenango', 'San Francisco Zapotitlán', 'San Bernardino', 'San José El Ídolo', 'Santo Domingo Suchitepéquez', 'San Lorenzo', 'Samayac', 'San Pablo Jocopilas', 'San Antonio Suchitepéquez', 'San Miguel Panán', 'San Gabriel', 'Chicacao', 'Patulul', 'Santa Bárbara', 'San Juan Bautista', 'Santo Tomás La Unión', 'Zunilito', 'Pueblo Nuevo', 'Río Bravo', 'San José La Máquina'],
    'Totonicapán': ['Totonicapán', 'San Cristóbal Totonicapán', 'San Francisco El Alto', 'San Andrés Xecul', 'Momostenango', 'Santa María Chiquimula', 'Santa Lucía La Reforma', 'San Bartolo Aguas Calientes'],
    'Zacapa': ['Zacapa', 'Estanzuela', 'Río Hondo', 'Gualán', 'Teculután', 'Usumatlán', 'Cabañas', 'San Diego', 'La Unión', 'Huité']
  }
};

let editMode = false;
let currentClienteId = null;

function cargarCombosClientes() {
  const deptoSelect = document.getElementById('departamento');
  if (!deptoSelect) return;
  deptoSelect.innerHTML = '<option value="">Seleccione departamento</option>';
  GUATEMALA_DATA.departamentos.forEach(d => {
    const opt = document.createElement('option');
    opt.value = d;
    opt.textContent = d;
    deptoSelect.appendChild(opt);
  });
}

document.addEventListener('DOMContentLoaded', () => {
  cargarCombosClientes();
  cargarClientes();

  document.getElementById('departamento')?.addEventListener('change', function() {
    const depto = this.value;
    const municipioSelect = document.getElementById('municipio');
    municipioSelect.innerHTML = depto
      ? '<option value="">Seleccione municipio</option>'
      : '<option value="">Seleccione primero un departamento</option>';
    municipioSelect.disabled = !depto;
    if (GUATEMALA_DATA.municipios[depto]) {
      GUATEMALA_DATA.municipios[depto].forEach(m => {
        const opt = document.createElement('option');
        opt.value = m;
        opt.textContent = m;
        municipioSelect.appendChild(opt);
      });
    }
  });

  document.getElementById('clienteForm')?.addEventListener('submit', async (e) => {
    e.preventDefault();
    hideAlert('clientesAlert');

    const payload = {
      tipoCliente: document.getElementById('tipoCliente').value,
      nombreRazonSocial: document.getElementById('nombreRazonSocial').value.trim(),
      nombreContacto: document.getElementById('nombreContacto').value.trim(),
      nit: document.getElementById('nit').value.trim(),
      telefonoPrincipal: document.getElementById('telefonoPrincipal').value.trim(),
      telefonoSecundario: document.getElementById('telefonoSecundario').value.trim(),
      correo: document.getElementById('correo').value.trim(),
      direccion: document.getElementById('direccion').value.trim(),
      municipio: document.getElementById('municipio').value.trim(),
      departamento: document.getElementById('departamento').value.trim(),
      observaciones: document.getElementById('observaciones').value.trim()
    };

    try {
      if (editMode && currentClienteId) {
        await apiRequest(`/Clientes/${currentClienteId}`, {
          method: 'PUT',
          body: JSON.stringify(payload)
        });
        showAlert('clientesAlert', 'Cliente actualizado correctamente.');
      } else {
        await apiRequest('/Clientes', {
          method: 'POST',
          body: JSON.stringify(payload)
        });
        showAlert('clientesAlert', 'Cliente guardado correctamente.');
      }

      resetFormCliente();
      await cargarClientes();
    } catch (error) {
      showAlert('clientesAlert', error.message, 'danger');
    }
  });

  document.getElementById('btnRecargarClientes')?.addEventListener('click', cargarClientes);
  document.getElementById('btnCancelarEdicion')?.addEventListener('click', resetFormCliente);

  const buscarInput = document.getElementById('buscarCliente');
  if (buscarInput) {
    buscarInput.addEventListener('keyup', function() {
      const filter = this.value.toLowerCase();
      const rows = document.querySelectorAll('#clientesTable tbody tr');
      rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(filter) ? '' : 'none';
      });
    });
  }
});

function resetFormCliente() {
  document.getElementById('clienteForm')?.reset();
  const municipioSelect = document.getElementById('municipio');
  municipioSelect.innerHTML = '<option value="">Seleccione primero un departamento</option>';
  municipioSelect.disabled = true;
  editMode = false;
  currentClienteId = null;
  document.getElementById('clienteId').value = '';
  document.getElementById('btnGuardarCliente').textContent = 'Guardar cliente';
  document.getElementById('btnCancelarEdicion').style.display = 'none';
  cargarCombosClientes();
}

async function cargarClientes() {
  try {
    const clientes = await apiRequest('/Clientes');
    const tbody = document.querySelector('#clientesTable tbody');
    tbody.innerHTML = '';

    clientes.forEach(cliente => {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${cliente.id}</td>
        <td>${cliente.tipoCliente}</td>
        <td>${cliente.nombreRazonSocial}</td>
        <td>${cliente.nombreContacto ?? ''}</td>
        <td>${cliente.nit ?? ''}</td>
        <td>${cliente.telefonoPrincipal ?? ''}</td>
        <td>${cliente.correo ?? ''}</td>
        <td><span class="badge ${cliente.estado ? 'text-bg-success' : 'text-bg-secondary'}">${cliente.estado ? 'Activo' : 'Inactivo'}</span></td>
        <td>
          <div class="table-actions">
            <button class="btn btn-outline-primary btn-sm btn-icon" title="Editar" onclick="editarCliente(${cliente.id})">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
            </button>
            <button class="btn btn-outline-${cliente.estado ? 'warning' : 'success'} btn-sm btn-icon" title="${cliente.estado ? 'Inactivar' : 'Activar'}" onclick="toggleEstadoCliente(${cliente.id})">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>
            </button>
          </div>
        </td>`;
      tbody.appendChild(tr);
    });
  } catch (error) {
    showAlert('clientesAlert', error.message, 'danger');
  }
}

async function editarCliente(id) {
  try {
    const cliente = await apiRequest(`/Clientes/${id}`);
    
    editMode = true;
    currentClienteId = id;
    document.getElementById('clienteId').value = id;
    document.getElementById('btnGuardarCliente').textContent = 'Actualizar cliente';
    document.getElementById('btnCancelarEdicion').style.display = 'inline-block';

    document.getElementById('tipoCliente').value = cliente.tipoCliente;
    document.getElementById('nombreRazonSocial').value = cliente.nombreRazonSocial;
    document.getElementById('nombreContacto').value = cliente.nombreContacto ?? '';
    document.getElementById('nit').value = cliente.nit ?? '';
    document.getElementById('telefonoPrincipal').value = cliente.telefonoPrincipal ?? '';
    document.getElementById('telefonoSecundario').value = cliente.telefonoSecundario ?? '';
    document.getElementById('correo').value = cliente.correo ?? '';
    document.getElementById('direccion').value = cliente.direccion ?? '';
    document.getElementById('observaciones').value = cliente.observaciones ?? '';

    if (cliente.departamento) {
      document.getElementById('departamento').value = cliente.departamento;
      document.getElementById('departamento').dispatchEvent(new Event('change'));
      setTimeout(() => {
        if (cliente.municipio) {
          document.getElementById('municipio').value = cliente.municipio;
        }
      }, 50);
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  } catch (error) {
    showAlert('clientesAlert', error.message, 'danger');
  }
}

async function toggleEstadoCliente(id) {
  const confirmar = await showConfirm('¿Está seguro de cambiar el estado de este cliente?');
  if (!confirmar) return;

  try {
    await apiRequest(`/Clientes/${id}/estado`, {
      method: 'PATCH'
    });
    showAlert('clientesAlert', 'Estado del cliente actualizado correctamente.');
    await cargarClientes();
  } catch (error) {
    showAlert('clientesAlert', error.message, 'danger');
  }
}
