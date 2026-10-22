const PAYMENT_METHODS = [
  { id: 1, nombre: 'Efectivo' },
  { id: 2, nombre: 'Tarjeta' },
  { id: 3, nombre: 'Depósito' },
  { id: 4, nombre: 'Transferencia' }
];

let selectedFile = null;
let filePreviewUrl = null;
let currentPagoDetail = null;
let allPagosData = [];

document.addEventListener('DOMContentLoaded', async () => {
  console.log('✅ Módulo de pagos cargado');
  document.getElementById('fechaPago').value = todayISO();
  
  try {
    await cargarCombosPagos();
    console.log('✅ Combos cargados');
  } catch (error) {
    console.error('❌ Error cargando combos:', error);
    showAlert('pagosAlert', 'Error al cargar datos: ' + error.message, 'danger');
  }

  document.getElementById('pagoForm')?.addEventListener('submit', guardarPago);
  document.getElementById('btnVerResumen')?.addEventListener('click', async () => {
    console.log('📌 Botón Ver resumen clickeado');
    await cargarResumenPago();
  });
  document.getElementById('btnRecargarPagos')?.addEventListener('click', cargarPagosPedidoSeleccionado);
  document.getElementById('idPedido')?.addEventListener('change', async () => {
    console.log('📌 Selector de pedido cambiado');
    await cargarResumenPago();
    await cargarPagosPedidoSeleccionado();
  });

  document.getElementById('voucherUrl')?.addEventListener('input', actualizarPreviewVoucher);
  document.getElementById('btnPreviewVoucher')?.addEventListener('click', () => {
    const url = document.getElementById('voucherUrl')?.value?.trim();
    if (url && url.startsWith('http')) {
      window.open(url, '_blank');
    }
  });

  const dropzone = document.getElementById('dropzoneArea');
  const fileInput = document.getElementById('voucherFileInput');

  if (dropzone) {
    dropzone.addEventListener('click', () => {
      fileInput?.click();
    });

    dropzone.addEventListener('dragover', (e) => {
      e.preventDefault();
      dropzone.classList.add('dragover');
    });

    dropzone.addEventListener('dragleave', (e) => {
      e.preventDefault();
      dropzone.classList.remove('dragover');
    });

    dropzone.addEventListener('drop', (e) => {
      e.preventDefault();
      dropzone.classList.remove('dragover');
      
      const files = e.dataTransfer?.files;
      if (files && files.length > 0) {
        handleFileSelect(files[0]);
      }
    });
  }

  if (fileInput) {
    fileInput.addEventListener('change', (e) => {
      const files = e.target?.files;
      if (files && files.length > 0) {
        handleFileSelect(files[0]);
      }
    });
  }

  document.getElementById('btnRemoveFile')?.addEventListener('click', clearSelectedFile);

  document.querySelector('[data-bs-target="#tab-voucher-url"]')?.addEventListener('shown.bs.tab', () => {
    actualizarPreviewVoucher();
  });

  document.querySelector('[data-bs-target="#tab-voucher-file"]')?.addEventListener('shown.bs.tab', () => {
    if (selectedFile) {
      showFilePreview();
    }
  });
});

function handleFileSelect(file) {
  console.log('📁 Archivo seleccionado:', file.name, file.type, file.size);
  
  const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp', 'application/pdf'];
  const isImage = file.type.startsWith('image/');
  const isPdf = file.type === 'application/pdf';
  
  if (!isImage && !isPdf) {
    showAlert('pagosAlert', 'Tipo de archivo no permitido. Usa imágenes (JPG, PNG, GIF) o PDF.', 'warning');
    return;
  }

  const maxSize = 10 * 1024 * 1024;
  if (file.size > maxSize) {
    showAlert('pagosAlert', 'El archivo es muy grande. Máximo 10MB.', 'warning');
    return;
  }

  selectedFile = file;

  if (filePreviewUrl) {
    URL.revokeObjectURL(filePreviewUrl);
  }
  filePreviewUrl = URL.createObjectURL(file);

  showFilePreview();
  showAlert('pagosAlert', `Archivo seleccionado: ${file.name}`, 'info');
}

function showFilePreview() {
  const previewArea = document.getElementById('filePreviewArea');
  const dropzone = document.getElementById('dropzoneArea');
  const imagePreview = document.getElementById('imageFilePreview');
  const previewImg = document.getElementById('filePreviewImage');
  const voucherPreview = document.getElementById('voucherPreview');

  if (!selectedFile || !filePreviewUrl) return;

  if (previewArea) previewArea.style.display = 'block';
  if (dropzone) dropzone.style.display = 'none';

  document.getElementById('selectedFileName').textContent = selectedFile.name;
  document.getElementById('selectedFileSize').textContent = formatFileSize(selectedFile.size);

  const isImage = selectedFile.type.startsWith('image/');
  
  if (isImage && imagePreview && previewImg) {
    imagePreview.style.display = 'block';
    previewImg.src = filePreviewUrl;
  } else if (imagePreview) {
    imagePreview.style.display = 'none';
  }

  if (voucherPreview) {
    if (isImage) {
      voucherPreview.innerHTML = `<img src="${filePreviewUrl}" alt="Voucher" class="img-fluid rounded" style="max-height: 150px;">`;
    } else {
      voucherPreview.innerHTML = `
        <div class="text-center p-3">
          <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="text-muted mb-2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 9 21 9"></polyline></svg>
          <div class="fw-bold">${selectedFile.name}</div>
          <div class="text-muted small">${formatFileSize(selectedFile.size)}</div>
        </div>`;
    }
  }
}

function clearSelectedFile() {
  selectedFile = null;
  
  if (filePreviewUrl) {
    URL.revokeObjectURL(filePreviewUrl);
    filePreviewUrl = null;
  }

  const previewArea = document.getElementById('filePreviewArea');
  const dropzone = document.getElementById('dropzoneArea');
  const fileInput = document.getElementById('voucherFileInput');
  const voucherPreview = document.getElementById('voucherPreview');

  if (previewArea) previewArea.style.display = 'none';
  if (dropzone) dropzone.style.display = 'block';
  if (fileInput) fileInput.value = '';
  
  if (voucherPreview) {
    voucherPreview.innerHTML = '<span class="text-muted small">Sin previsualización</span>';
  }
}

function formatFileSize(bytes) {
  if (bytes === 0) return '0 Bytes';
  
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function getVoucherAbsoluteUrl(voucherUrl) {
  if (!voucherUrl) return '';

  if (voucherUrl.startsWith('/uploads/')) {
    return `${API_CONFIG.baseUrl.replace(/\/api\/?$/, '')}${voucherUrl}`;
  }

  try {
    return new URL(voucherUrl, window.location.href).href;
  } catch {
    return voucherUrl;
  }
}

function isVoucherImageUrl(voucherUrl) {
  if (!voucherUrl) return false;
  const cleanUrl = voucherUrl.split(/[?#]/)[0];
  return /\.(jpg|jpeg|png|gif|webp)$/i.test(cleanUrl);
}

async function cargarImagenVoucherParaPdf(voucherUrl) {
  const absoluteUrl = getVoucherAbsoluteUrl(voucherUrl);
  const response = await fetch(absoluteUrl);

  if (!response.ok) {
    throw new Error(`No se pudo descargar la imagen (${response.status})`);
  }

  const blob = await response.blob();
  if (!blob.type.startsWith('image/')) {
    throw new Error('El voucher guardado no es una imagen');
  }

  const objectUrl = URL.createObjectURL(blob);

  try {
    const image = await new Promise((resolve, reject) => {
      const img = new Image();
      img.onload = () => resolve(img);
      img.onerror = () => reject(new Error('No se pudo leer la imagen del voucher'));
      img.src = objectUrl;
    });

    const maxCanvasSize = 2200;
    const resizeScale = Math.min(1, maxCanvasSize / Math.max(image.naturalWidth, image.naturalHeight));
    const canvas = document.createElement('canvas');
    canvas.width = Math.max(1, Math.round(image.naturalWidth * resizeScale));
    canvas.height = Math.max(1, Math.round(image.naturalHeight * resizeScale));

    const context = canvas.getContext('2d');
    if (!context) {
      throw new Error('No se pudo preparar la imagen para el PDF');
    }

    context.fillStyle = '#ffffff';
    context.fillRect(0, 0, canvas.width, canvas.height);
    context.drawImage(image, 0, 0, canvas.width, canvas.height);

    return {
      dataUrl: canvas.toDataURL('image/jpeg', 0.9),
      width: canvas.width,
      height: canvas.height,
      absoluteUrl
    };
  } finally {
    URL.revokeObjectURL(objectUrl);
  }
}

async function cargarCombosPagos() {
  try {
    const [pedidos, usuarios] = await Promise.all([
      apiRequest('/Pedidos'),
      apiRequest('/Usuarios')
    ]);

    console.log('📦 Pedidos recibidos:', pedidos.length);
    console.log('👥 Usuarios recibidos:', usuarios.length);

    fillSelect('idPedido', pedidos.map(p => ({ value: p.id, label: `${p.numeroPedido} | ${p.cliente} | Q${Number(p.total).toFixed(2)}` })), 'Seleccione un pedido');
    fillSelect('idUsuario', usuarios.filter(u => u.estado).map(u => ({ value: u.id, label: `${u.username} | ${u.nombres} ${u.apellidos}` })), 'Seleccione un usuario');
    fillSelect('idMetodoPago', PAYMENT_METHODS.map(m => ({ value: m.id, label: m.nombre })), 'Seleccione método');
  } catch (error) {
    console.error('❌ Error en cargarCombosPagos:', error);
    showAlert('pagosAlert', 'Error al cargar combos: ' + error.message, 'danger');
  }
}

async function uploadFile(file) {
  console.log('📤 Subiendo archivo al servidor:', file.name);
  
  const formData = new FormData();
  formData.append('file', file);

  const token = getToken();
  const response = await fetch(`${API_CONFIG.baseUrl}/Upload/voucher`, {
    method: 'POST',
    headers: token ? { 'Authorization': `Bearer ${token}` } : {},
    body: formData
  });

  let data = null;
  try {
    data = await response.json();
  } catch {
    data = null;
  }

  if (!response.ok) {
    const errorMessage = data?.mensaje || 'Error al subir el archivo';
    throw new Error(errorMessage);
  }

  console.log('✅ Archivo subido:', data);
  return data;
}

async function guardarPago(e) {
  e.preventDefault();
  hideAlert('pagosAlert');

  const idPedido = Number(document.getElementById('idPedido').value);
  console.log('💸 Guardando pago para pedido:', idPedido);
  
  if (!idPedido) {
    showAlert('pagosAlert', 'Por favor seleccione un pedido.', 'warning');
    return;
  }

  let voucherUrl = document.getElementById('voucherUrl')?.value?.trim() || null;
  
  if (selectedFile) {
    try {
      showAlert('pagosAlert', 'Subiendo voucher al servidor...', 'info');
      const uploadResult = await uploadFile(selectedFile);
      
      if (uploadResult && uploadResult.success && uploadResult.url) {
        voucherUrl = uploadResult.url;
        console.log('✅ Voucher subido, URL:', voucherUrl);
      } else {
        throw new Error(uploadResult?.errorMessage || 'No se pudo subir el archivo');
      }
    } catch (uploadError) {
      showAlert('pagosAlert', `Error al subir el voucher: ${uploadError.message}`, 'danger');
      return;
    }
  }

  const payload = {
    idPedido: idPedido,
    idMetodoPago: Number(document.getElementById('idMetodoPago').value),
    fechaPago: new Date(document.getElementById('fechaPago').value).toISOString(),
    monto: Number(document.getElementById('monto').value),
    numeroReferencia: document.getElementById('numeroReferencia').value.trim() || null,
    observaciones: document.getElementById('observacionesPago').value.trim() || null,
    voucherUrl: voucherUrl,
    idUsuario: Number(document.getElementById('idUsuario').value)
  };

  console.log('📤 Payload:', payload);

  try {
    const resultado = await apiRequest('/Pagos', {
      method: 'POST',
      body: JSON.stringify(payload)
    });

    console.log('✅ Pago guardado:', resultado);
    showAlert('pagosAlert', 'Pago registrado correctamente.' + (voucherUrl ? ' Voucher guardado en servidor.' : ''));
    document.getElementById('monto').value = '';
    document.getElementById('numeroReferencia').value = '';
    document.getElementById('observacionesPago').value = '';
    document.getElementById('voucherUrl').value = '';
    clearSelectedFile();
    
    await cargarResumenPago();
    await cargarPagosPedidoSeleccionado();
  } catch (error) {
    console.error('❌ Error guardando pago:', error);
    showAlert('pagosAlert', error.message, 'danger');
  }
}

async function cargarResumenPago() {
  const idPedidoSelect = document.getElementById('idPedido');
  const idPedido = Number(idPedidoSelect?.value);
  
  console.log('📊 Cargando resumen para pedido:', idPedido);
  hideAlert('pagosAlert');

  const vacioEl = document.getElementById('resumenPagoVacio');
  const boxEl = document.getElementById('resumenPagoBox');

  if (!idPedido || idPedido <= 0) {
    console.log('⚠️ No hay pedido seleccionado');
    vacioEl?.classList.remove('d-none');
    vacioEl.textContent = 'Selecciona un pedido y presiona "Ver resumen".';
    boxEl?.classList.add('d-none');
    return;
  }

  try {
    console.log('🔍 Haciendo request a:', `/Pagos/pedido/${idPedido}/resumen`);
    const resumen = await apiRequest(`/Pagos/pedido/${idPedido}/resumen`);
    
    console.log('✅ Resumen recibido:', resumen);
    
    vacioEl?.classList.add('d-none');
    boxEl?.classList.remove('d-none');
    
    document.getElementById('resNumeroPedido').textContent = resumen.numeroPedido || '-';
    document.getElementById('resTotalPedido').textContent = money(resumen.totalPedido);
    document.getElementById('resTotalPagado').textContent = money(resumen.totalPagado);
    document.getElementById('resSaldoPendiente').textContent = money(resumen.saldoPendiente);
    document.getElementById('resEstadoFinanciero').textContent = resumen.estadoFinanciero || '-';

  } catch (error) {
    console.error('❌ Error cargando resumen:', error);
    console.error('❌ Error message:', error.message);
    
    vacioEl?.classList.remove('d-none');
    vacioEl.textContent = `Error: ${error.message}`;
    boxEl?.classList.add('d-none');
    showAlert('pagosAlert', error.message, 'danger');
  }
}

async function cargarPagosPedidoSeleccionado() {
  const idPedido = Number(document.getElementById('idPedido')?.value);
  const tbody = document.querySelector('#pagosTable tbody');
  tbody.innerHTML = '';
  
  if (!idPedido) {
    tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-4">Seleccione un pedido para ver sus pagos</td></tr>';
    return;
  }

  try {
    const pagos = await apiRequest(`/Pagos/pedido/${idPedido}`);
    console.log('💰 Pagos recibidos:', pagos.length);
    
    if (pagos.length === 0) {
      tbody.innerHTML = '<tr><td colspan="8" class="text-center text-muted py-4">No hay pagos registrados para este pedido</td></tr>';
      return;
    }

     allPagosData = pagos;
     pagos.forEach(p => {
       const tr = document.createElement('tr');
       tr.style.cursor = 'pointer';
       tr.dataset.pagoId = p.id;
       tr.classList.add('hover-row');
       tr.innerHTML = `
         <td>${p.id}</td>
         <td><strong>${p.numeroPedido}</strong></td>
         <td>${p.cliente}</td>
         <td><span class="badge text-bg-info">${p.metodoPago}</span></td>
         <td>${formatDate(p.fechaPago)}</td>
         <td><strong class="text-primary">${money(p.monto)}</strong></td>
         <td>${p.numeroReferencia ?? '-'}</td>
         <td>${p.usuario ?? '-'}</td>`;
       tr.addEventListener('click', () => abrirDetallePago(p.id));
       tbody.appendChild(tr);
     });
  } catch (error) {
    console.error('❌ Error cargando pagos:', error);
    tbody.innerHTML = `<tr><td colspan="8" class="text-center text-danger py-4">Error: ${error.message}</td></tr>`;
    showAlert('pagosAlert', error.message, 'danger');
  }
}

function fillSelect(elementId, options, placeholder) {
  const select = document.getElementById(elementId);
  if (!select) {
    console.warn(`⚠️ No se encontró el elemento: ${elementId}`);
    return;
  }
  select.innerHTML = `<option value="">${placeholder}</option>`;
  options.forEach(option => {
    const opt = document.createElement("option");
    opt.value = option.value;
    opt.textContent = option.label;
    select.appendChild(opt);
  });
}

function actualizarPreviewVoucher() {
  const url = document.getElementById('voucherUrl')?.value?.trim();
  const previewEl = document.getElementById('voucherPreview');
  const urlTab = document.querySelector('[data-bs-target="#tab-voucher-url"]');
  
  if (urlTab && !urlTab.classList.contains('active')) {
    return;
  }
  
  if (!previewEl) return;

  if (!url) {
    previewEl.innerHTML = '<span class="text-muted small">Sin previsualización</span>';
    return;
  }

  const isLocalUpload = url.startsWith('/uploads/');
  const isFullUrl = url.startsWith('http');
  
  if (!isLocalUpload && !isFullUrl) {
    previewEl.innerHTML = '<span class="text-warning small">URL no válida</span>';
    return;
  }

  const isImage = isVoucherImageUrl(url);
  
  if (isImage) {
    const imgSrc = getVoucherAbsoluteUrl(url);
    
    previewEl.innerHTML = `<img src="${imgSrc}" alt="Voucher" class="img-fluid rounded" style="max-height: 150px; object-fit: contain;" onerror="this.parentElement.innerHTML='<span class=\\'text-danger small\\'>No se pudo cargar la imagen</span>'">`;
  } 
  else if (url.includes('drive.google.com')) {
    previewEl.innerHTML = `
      <div class="text-center p-2">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="#4285f4" class="mb-2"><path d="M21.35 11.1h-9.17l2.85-4.92a5.95 5.95 0 0 1 6.32 4.92z"/><path d="M12.18 12.27l2.85-4.92h-5.7L3.63 12.27l2.85 4.92h5.7l-.01-.02z"/><path d="M2.65 12.9a5.95 5.95 0 0 1 0-1.8l2.85 4.92-2.85-3.12z"/><path d="M9.33 17.19l-2.85-4.92H3.63l5.7 4.92z"/><path d="M12.18 12.27l-2.85 4.92h5.7l2.85-4.92h-5.7z"/><path d="M21.35 12.9a5.95 5.95 0 0 1 0 1.8l-2.85-4.92 2.85 3.12z"/></svg>
        <div class="small text-muted mt-1">Google Drive</div>
        <a href="${url}" target="_blank" class="small d-block mt-1">Abrir en nueva pestaña →</a>
      </div>`;
  }
  else if (url.includes('dropbox.com')) {
    previewEl.innerHTML = `
      <div class="text-center p-2">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="#0061ff" class="mb-2"><path d="M12 14.85l-6.35-4.07L0 14.85l6.35 4.07L12 14.85zm6.35-4.07L12 14.85l6.35 4.07L24 14.85l-5.65-4.07zM6.35 0L0 4.07l6.35 4.07L12 4.07 6.35 0zm5.65 4.07L6.35 8.14 12 12.21l5.65-4.07L12 4.07z"/></svg>
        <div class="small text-muted mt-1">Dropbox</div>
        <a href="${url}" target="_blank" class="small d-block mt-1">Abrir en nueva pestaña →</a>
      </div>`;
  }
  else {
     previewEl.innerHTML = `
       <div class="text-center p-2">
         <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="text-muted mb-2"><path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"></path><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"></path></svg>
         <div class="small text-muted mt-1">Enlace externo</div>
         <a href="${url}" target="_blank" class="small d-block mt-1">Abrir en nueva pestaña →</a>
       </div>`;
   }
 }

function abrirDetallePago(pagoId) {
   const pago = allPagosData.find(p => p.id === pagoId);
   if (!pago) {
     showAlert('pagosAlert', 'No se encontró el pago seleccionado', 'warning');
     return;
   }

   currentPagoDetail = pago;
   const body = document.getElementById('pagoDetailBody');

   const voucherContent = generarVoucherPreview(pago.voucherUrl);

   body.innerHTML = `
     <div class="row">
       <div class="col-md-6 mb-4">
         <div class="p-3 bg-light rounded-3">
           <h6 class="text-muted text-uppercase small mb-2">Información del Pago</h6>
           <table class="table table-sm table-borderless mb-0">
             <tr><td class="text-muted py-1" style="width: 40%;">ID Pago:</td><td class="py-1"><strong>#${pago.id}</strong></td></tr>
             <tr><td class="text-muted py-1">Fecha:</td><td class="py-1">${formatDate(pago.fechaPago)}</td></tr>
             <tr><td class="text-muted py-1">Método:</td><td class="py-1"><span class="badge text-bg-info">${pago.metodoPago}</span></td></tr>
             <tr><td class="text-muted py-1">Monto:</td><td class="py-1"><strong class="text-primary fs-5">${money(pago.monto)}</strong></td></tr>
             <tr><td class="text-muted py-1">Referencia:</td><td class="py-1">${pago.numeroReferencia ?? '<span class="text-muted">N/A</span>'}</td></tr>
           </table>
         </div>
       </div>
       <div class="col-md-6 mb-4">
         <div class="p-3 bg-light rounded-3">
           <h6 class="text-muted text-uppercase small mb-2">Información del Pedido</h6>
           <table class="table table-sm table-borderless mb-0">
             <tr><td class="text-muted py-1" style="width: 40%;">Pedido:</td><td class="py-1"><strong>${pago.numeroPedido}</strong></td></tr>
             <tr><td class="text-muted py-1">Cliente:</td><td class="py-1">${pago.cliente}</td></tr>
             <tr><td class="text-muted py-1">Registrado por:</td><td class="py-1">${pago.usuario ?? '<span class="text-muted">N/A</span>'}</td></tr>
             <tr><td class="text-muted py-1">Estado:</td><td class="py-1"><span class="badge ${pago.estado ? 'text-bg-success' : 'text-bg-secondary'}">${pago.estado ? 'Activo' : 'Inactivo'}</span></td></tr>
           </table>
         </div>
       </div>
     </div>

     ${pago.observaciones ? `
     <div class="mb-4">
       <h6 class="text-muted text-uppercase small mb-2">Observaciones</h6>
       <div class="p-3 bg-light rounded-3">
         <p class="mb-0">${pago.observaciones}</p>
       </div>
     </div>
     ` : ''}

     <div class="mb-2">
       <h6 class="text-muted text-uppercase small mb-2">Voucher / Comprobante</h6>
       <div class="border rounded-3 bg-light p-4 text-center" style="min-height: 150px;">
         ${voucherContent}
       </div>
     </div>
   `;

   const modal = new bootstrap.Modal(document.getElementById('pagoDetailModal'));
   modal.show();
 }

function generarVoucherPreview(voucherUrl) {
   if (!voucherUrl) {
     return '<span class="text-muted">Sin voucher adjunto</span>';
   }

   const isLocalUpload = voucherUrl.startsWith('/uploads/');
   const isPdf = voucherUrl.toLowerCase().endsWith('.pdf');
   const isImage = isVoucherImageUrl(voucherUrl);

   if (isImage) {
     const imgSrc = getVoucherAbsoluteUrl(voucherUrl);
     return `<img src="${imgSrc}" alt="Voucher" class="img-fluid rounded" style="max-height: 300px; object-fit: contain;" onerror="this.parentElement.innerHTML='<span class=\\'text-danger\\'>No se pudo cargar la imagen</span>'">
       <div class="mt-2"><a href="${imgSrc}" target="_blank" class="small text-primary">Abrir imagen en nueva pestaña →</a></div>`;
   }

   if (isPdf) {
     let pdfSrc = voucherUrl;
     if (isLocalUpload) {
       pdfSrc = `${API_CONFIG.baseUrl.replace('/api', '')}${voucherUrl}`;
     }
     return `
       <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="#dc2626" class="mb-2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><path d="M12 16.5v-3.5"></path><path d="M9 13h6"></path><path d="M8.5 13v3.5"></path><path d="M15.5 13v3.5"></path></svg>
       <div class="fw-bold mb-1">Documento PDF</div>
       <a href="${pdfSrc}" target="_blank" class="btn btn-sm btn-outline-primary mt-2">Abrir PDF</a>`;
   }

   if (voucherUrl.includes('drive.google.com')) {
     return `
       <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="#4285f4" class="mb-2"><path d="M21.35 11.1h-9.17l2.85-4.92a5.95 5.95 0 0 1 6.32 4.92z"/><path d="M12.18 12.27l2.85-4.92h-5.7L3.63 12.27l2.85 4.92h5.7l-.01-.02z"/><path d="M2.65 12.9a5.95 5.95 0 0 1 0-1.8l2.85 4.92-2.85-3.12z"/><path d="M9.33 17.19l-2.85-4.92H3.63l5.7 4.92z"/><path d="M12.18 12.27l-2.85 4.92h5.7l2.85-4.92h-5.7z"/><path d="M21.35 12.9a5.95 5.95 0 0 1 0 1.8l-2.85-4.92 2.85 3.12z"/></svg>
       <div class="fw-bold mb-1">Google Drive</div>
       <a href="${voucherUrl}" target="_blank" class="btn btn-sm btn-outline-primary mt-2">Abrir en Drive</a>`;
   }

   if (voucherUrl.includes('dropbox.com')) {
     return `
       <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="#0061ff" class="mb-2"><path d="M12 14.85l-6.35-4.07L0 14.85l6.35 4.07L12 14.85zm6.35-4.07L12 14.85l6.35 4.07L24 14.85l-5.65-4.07zM6.35 0L0 4.07l6.35 4.07L12 4.07 6.35 0zm5.65 4.07L6.35 8.14 12 12.21l5.65-4.07L12 4.07z"/></svg>
       <div class="fw-bold mb-1">Dropbox</div>
       <a href="${voucherUrl}" target="_blank" class="btn btn-sm btn-outline-primary mt-2">Abrir en Dropbox</a>`;
   }

   return `
     <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="text-muted mb-2"><path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"></path><path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"></path></svg>
     <div class="fw-bold mb-1">Enlace externo</div>
     <a href="${voucherUrl}" target="_blank" class="btn btn-sm btn-outline-primary mt-2">Abrir enlace</a>`;
 }

document.addEventListener('DOMContentLoaded', () => {
   document.getElementById('btnDescargarPDF')?.addEventListener('click', descargarPDFPago);
 });

async function descargarPDFPago() {
   if (!currentPagoDetail) {
     showAlert('pagosAlert', 'No hay pago seleccionado para descargar', 'warning');
     return;
   }

   try {
     showAlert('pagosAlert', 'Generando PDF...', 'info');
     
     const { jsPDF } = window.jspdf;
     const doc = new jsPDF();
     
     const pageWidth = doc.internal.pageSize.getWidth();
     const pageHeight = doc.internal.pageSize.getHeight();
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
     doc.text('Comprobante de Pago', pageWidth / 2, yPos, { align: 'center' });
     
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
     doc.text('Información del Pago', 20, yPos);
     yPos += 7;

     doc.setFillColor(245, 245, 245);
     doc.roundedRect(20, yPos, pageWidth - 40, 35, 3, 3, 'F');
     
     yPos += 7;
     doc.setFontSize(10);
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('ID Pago:', 25, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(`#${currentPagoDetail.id}`, 60, yPos);
     
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Fecha:', 110, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(formatDate(currentPagoDetail.fechaPago), 140, yPos);
     
     yPos += 7;
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Método:', 25, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(currentPagoDetail.metodoPago, 60, yPos);
     
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Monto:', 110, yPos);
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(123, 17, 20);
     doc.text(money(currentPagoDetail.monto), 140, yPos);
     
     yPos += 7;
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Referencia:', 25, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(currentPagoDetail.numeroReferencia ?? 'N/A', 60, yPos);

     yPos += 12;
     doc.setFontSize(12);
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(60, 60, 60);
     doc.text('Información del Pedido', 20, yPos);
     yPos += 7;

     doc.setFillColor(245, 245, 245);
     doc.roundedRect(20, yPos, pageWidth - 40, 25, 3, 3, 'F');
     
     yPos += 7;
     doc.setFontSize(10);
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Pedido:', 25, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(currentPagoDetail.numeroPedido, 60, yPos);
     
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Cliente:', 110, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(currentPagoDetail.cliente, 140, yPos);
     
     yPos += 7;
     doc.setFont('helvetica', 'bold');
     doc.setTextColor(80, 80, 80);
     doc.text('Registrado por:', 25, yPos);
     doc.setFont('helvetica', 'normal');
     doc.setTextColor(40, 40, 40);
     doc.text(currentPagoDetail.usuario ?? 'N/A', 60, yPos);

     if (currentPagoDetail.observaciones) {
       yPos += 12;
       doc.setFontSize(12);
       doc.setFont('helvetica', 'bold');
       doc.setTextColor(60, 60, 60);
       doc.text('Observaciones', 20, yPos);
       yPos += 7;

       doc.setFillColor(245, 245, 245);
       doc.roundedRect(20, yPos, pageWidth - 40, 20, 3, 3, 'F');
       
       yPos += 7;
       doc.setFontSize(10);
       doc.setFont('helvetica', 'normal');
       doc.setTextColor(40, 40, 40);
       const observaciones = currentPagoDetail.observaciones.length > 80 
         ? currentPagoDetail.observaciones.substring(0, 80) + '...' 
         : currentPagoDetail.observaciones;
       doc.text(observaciones, 25, yPos);
     }

     if (currentPagoDetail.voucherUrl) {
       const voucherUrl = currentPagoDetail.voucherUrl;
       const absoluteVoucherUrl = getVoucherAbsoluteUrl(voucherUrl);
       let voucherImage = null;

       if (isVoucherImageUrl(voucherUrl)) {
         try {
           voucherImage = await cargarImagenVoucherParaPdf(voucherUrl);
         } catch (voucherError) {
           console.warn('No se pudo incrustar la imagen del voucher:', voucherError);
         }
       }

       let imageWidth = 0;
       let imageHeight = 0;

       if (voucherImage) {
         const maxImageWidth = pageWidth - 40;
         const maxImageHeight = 125;
         const imageScale = Math.min(
           maxImageWidth / voucherImage.width,
           maxImageHeight / voucherImage.height
         );
         imageWidth = voucherImage.width * imageScale;
         imageHeight = voucherImage.height * imageScale;
       }

       const requiredHeight = 7 + (voucherImage ? imageHeight + 12 : 18);
       if (yPos + 12 + requiredHeight > pageHeight - 35) {
         doc.addPage();
         yPos = 20;
       } else {
         yPos += 12;
       }

       doc.setFontSize(12);
       doc.setFont('helvetica', 'bold');
       doc.setTextColor(60, 60, 60);
       doc.text('Voucher / Comprobante', 20, yPos);
       yPos += 7;

       if (voucherImage) {
         const imageX = (pageWidth - imageWidth) / 2;
         doc.setDrawColor(220, 220, 220);
         doc.setLineWidth(0.3);
         doc.roundedRect(imageX - 2, yPos - 2, imageWidth + 4, imageHeight + 4, 2, 2, 'S');
         doc.addImage(voucherImage.dataUrl, 'JPEG', imageX, yPos, imageWidth, imageHeight);
         yPos += imageHeight + 7;

         doc.setFontSize(8);
         doc.setFont('helvetica', 'normal');
         doc.setTextColor(0, 0, 180);
         doc.textWithLink('Abrir imagen original', 20, yPos, { url: voucherImage.absoluteUrl });
       } else {
         doc.setFontSize(9);
         doc.setFont('helvetica', 'normal');
         doc.setTextColor(90, 90, 90);
         const description = isVoucherImageUrl(voucherUrl)
           ? 'No fue posible incrustar la imagen. Puede consultarla en el enlace:'
           : 'El voucher adjunto puede consultarse en el enlace:';
         doc.text(description, 20, yPos);
         yPos += 5;

         doc.setTextColor(0, 0, 200);
         const urlToShow = voucherUrl.length > 70
           ? voucherUrl.substring(0, 70) + '...'
           : voucherUrl;
         doc.textWithLink(urlToShow, 20, yPos, { url: absoluteVoucherUrl });
       }
     }

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

     const fileName = `Comprobante_Pago_${currentPagoDetail.id}_${currentPagoDetail.numeroPedido}.pdf`;
     doc.save(fileName);
     
     hideAlert('pagosAlert');
     showAlert('pagosAlert', 'PDF generado correctamente.', 'success');
     
   } catch (error) {
     console.error('Error generando PDF:', error);
     showAlert('pagosAlert', `Error al generar el PDF: ${error.message}`, 'danger');
   }
 }
