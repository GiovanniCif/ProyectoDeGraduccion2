let chartPedidos = null;
let chartProductos = null;

document.addEventListener('DOMContentLoaded', async () => {
  try {
    const [dashboardData, pedidosData, productosData] = await Promise.all([
      apiRequest('/Reportes/dashboard'),
      apiRequest('/Reportes/pedidos').catch(() => []),
      apiRequest('/Reportes/productos-terminados').catch(() => [])
    ]);

    document.getElementById('kpiTotalClientes').textContent = dashboardData.totalClientes;
    document.getElementById('kpiTotalPedidos').textContent = dashboardData.totalPedidos;
    document.getElementById('kpiPedidosPendientes').textContent = dashboardData.pedidosPendientes;
    document.getElementById('kpiPedidosEntregados').textContent = dashboardData.pedidosEntregados;
    document.getElementById('kpiTotalCobrado').textContent = `Q${Number(dashboardData.totalCobrado).toFixed(2)}`;
    document.getElementById('kpiSaldoPendiente').textContent = `Q${Number(dashboardData.saldoPendiente).toFixed(2)}`;
    document.getElementById('kpiProductosTerminados').textContent = dashboardData.totalProductosTerminados;
    document.getElementById('kpiEmpleadosActivos').textContent = dashboardData.empleadosActivos;

    renderizarChartPedidos(pedidosData);
    renderizarChartProductos(productosData);
  } catch (error) {
    showAlert('dashboardAlert', error.message, 'danger');
  }
});

function renderizarChartPedidos(pedidos) {
  const ctx = document.getElementById('chartPedidos');
  if (!ctx) return;

  const conteoEstados = {};
  pedidos.forEach(p => {
    const estado = p.estadoPedido || 'Sin estado';
    conteoEstados[estado] = (conteoEstados[estado] || 0) + 1;
  });

  const labels = Object.keys(conteoEstados);
  const data = Object.values(conteoEstados);
  
  const colores = [
    '#7B1114',
    '#D96B10',
    '#E8A94E',
    '#5C6B70',
    '#8B9A4F',
    '#3D4F5F'
  ];

  if (chartPedidos) {
    chartPedidos.destroy();
  }

  chartPedidos = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: labels.length > 0 ? labels : ['Sin datos'],
      datasets: [{
        data: labels.length > 0 ? data : [1],
        backgroundColor: labels.length > 0 ? colores.slice(0, labels.length) : ['#CCCCCC'],
        borderWidth: 2,
        borderColor: '#ffffff'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            padding: 15,
            usePointStyle: true
          }
        }
      }
    }
  });
}

function renderizarChartProductos(productos) {
  const ctx = document.getElementById('chartProductos');
  if (!ctx) return;

  const productosActivos = productos.filter(p => p.estado).slice(0, 8);

  const labels = productosActivos.map(p => {
    const nombre = p.nombre || 'Producto';
    return nombre.length > 15 ? nombre.substring(0, 15) + '...' : nombre;
  });

  const stockData = productosActivos.map(p => Number(p.stockActual || 0));

  if (chartProductos) {
    chartProductos.destroy();
  }

  chartProductos = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: labels.length > 0 ? labels : ['Sin datos'],
      datasets: [{
        label: 'Stock Actual',
        data: labels.length > 0 ? stockData : [0],
        backgroundColor: labels.length > 0 
          ? labels.map((_, i) => i % 2 === 0 ? '#7B1114' : '#D96B10')
          : ['#CCCCCC'],
        borderRadius: 4
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      indexAxis: 'y',
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        x: {
          beginAtZero: true
        }
      }
    }
  });
}
