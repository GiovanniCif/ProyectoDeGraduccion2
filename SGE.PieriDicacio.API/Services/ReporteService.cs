using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Reporte;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ReportePedidoDto>>> ObtenerReportePedidosAsync(DateTime? desde, DateTime? hasta, int? idEstado, string? prioridad)
        {
            var consulta = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.EstadoPedido)
                .AsQueryable();

            if (desde.HasValue)
                consulta = consulta.Where(p => p.FechaPedido >= desde.Value.Date);
            if (hasta.HasValue)
                consulta = consulta.Where(p => p.FechaPedido < hasta.Value.Date.AddDays(1));
            if (idEstado.HasValue)
                consulta = consulta.Where(p => p.IdEstadoPedido == idEstado.Value);
            if (!string.IsNullOrWhiteSpace(prioridad))
            {
                var p = prioridad.Trim().ToUpper();
                consulta = consulta.Where(x => x.Prioridad == p);
            }

            var reportes = await consulta
                .OrderByDescending(p => p.FechaPedido)
                .Select(p => new ReportePedidoDto
                {
                    Id = p.Id,
                    NumeroPedido = p.NumeroPedido,
                    Cliente = p.Cliente.NombreRazonSocial,
                    EstadoPedido = p.EstadoPedido.Nombre,
                    FechaPedido = p.FechaPedido,
                    FechaEntrega = p.FechaEntrega,
                    Prioridad = p.Prioridad,
                    Total = p.Total,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<ReportePedidoDto>>.Ok(reportes);
        }

        public async Task<Result<List<ReportePagoDto>>> ObtenerReportePagosAsync(DateTime? desde, DateTime? hasta, int? idMetodoPago)
        {
            var consulta = _context.Pagos
                .Include(p => p.Pedido)
                .Include(p => p.Cliente)
                .Include(p => p.MetodoPago)
                .Where(p => p.Estado)
                .AsQueryable();

            if (desde.HasValue)
                consulta = consulta.Where(p => p.FechaPago >= desde.Value.Date);
            if (hasta.HasValue)
                consulta = consulta.Where(p => p.FechaPago < hasta.Value.Date.AddDays(1));
            if (idMetodoPago.HasValue)
                consulta = consulta.Where(p => p.IdMetodoPago == idMetodoPago.Value);

            var reportes = await consulta
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new ReportePagoDto
                {
                    Id = p.Id,
                    NumeroPedido = p.Pedido.NumeroPedido,
                    Cliente = p.Cliente.NombreRazonSocial,
                    MetodoPago = p.MetodoPago.Nombre,
                    FechaPago = p.FechaPago,
                    Monto = p.Monto,
                    NumeroReferencia = p.NumeroReferencia,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<ReportePagoDto>>.Ok(reportes);
        }

        public async Task<Result<List<ReporteProductoTerminadoDto>>> ObtenerReporteProductosTerminadosAsync(int? idTipoPrenda, bool? soloStockBajo)
        {
            var consulta = _context.ProductosTerminados
                .Include(p => p.TipoPrenda)
                .Include(p => p.Talla)
                .Include(p => p.Color)
                .AsQueryable();

            if (idTipoPrenda.HasValue)
                consulta = consulta.Where(p => p.IdTipoPrenda == idTipoPrenda.Value);
            if (soloStockBajo == true)
                consulta = consulta.Where(p => p.StockActual < p.StockMinimo);

            var reportes = await consulta
                .OrderBy(p => p.Nombre)
                .Select(p => new ReporteProductoTerminadoDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    TipoPrenda = p.TipoPrenda != null ? p.TipoPrenda.Nombre : string.Empty,
                    Talla = p.Talla != null ? p.Talla.Nombre : null,
                    Color = p.Color != null ? p.Color.Nombre : null,
                    StockActual = p.StockActual,
                    StockMinimo = p.StockMinimo,
                    PrecioReferencial = p.PrecioReferencial,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<ReporteProductoTerminadoDto>>.Ok(reportes);
        }

        public async Task<Result<List<ReporteInsumoDto>>> ObtenerReporteInsumosAsync(string? categoria, bool? soloStockBajo)
        {
            var consulta = _context.Insumos
                .Include(i => i.UnidadMedida)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                var c = categoria.Trim();
                consulta = consulta.Where(i => i.Categoria != null && i.Categoria == c);
            }
            if (soloStockBajo == true)
                consulta = consulta.Where(i => i.StockActual < i.StockMinimo);

            var reportes = await consulta
                .OrderBy(i => i.Nombre)
                .Select(i => new ReporteInsumoDto
                {
                    Id = i.Id,
                    Codigo = i.Codigo,
                    Nombre = i.Nombre,
                    Categoria = i.Categoria,
                    UnidadMedida = i.UnidadMedida != null ? i.UnidadMedida.Nombre : string.Empty,
                    StockActual = i.StockActual,
                    StockMinimo = i.StockMinimo,
                    CostoReferencial = i.CostoReferencial,
                    Estado = i.Estado
                })
                .ToListAsync();

            return Result<List<ReporteInsumoDto>>.Ok(reportes);
        }

        public async Task<Result<List<ReporteAsistenciaDto>>> ObtenerReporteAsistenciasAsync(DateTime? desde, DateTime? hasta)
        {
            var consulta = _context.Asistencias
                .Include(a => a.Empleado)
                .AsQueryable();

            if (desde.HasValue)
                consulta = consulta.Where(a => a.Fecha >= desde.Value.Date);
            if (hasta.HasValue)
                consulta = consulta.Where(a => a.Fecha <= hasta.Value.Date);

            var reportes = await consulta
                .OrderByDescending(a => a.Fecha)
                .Select(a => new ReporteAsistenciaDto
                {
                    Id = a.Id,
                    Empleado = a.Empleado.Nombres + " " + a.Empleado.Apellidos,
                    Fecha = a.Fecha,
                    HoraEntrada = a.HoraEntrada,
                    HoraSalida = a.HoraSalida,
                    TardanzaMinutos = a.TardanzaMinutos,
                    HorasTrabajadas = a.HorasTrabajadas,
                    Observaciones = a.Observaciones
                })
                .ToListAsync();

            return Result<List<ReporteAsistenciaDto>>.Ok(reportes);
        }

        public async Task<Result<DashboardDto>> ObtenerDashboardAsync()
        {
            var totalClientes = await _context.Clientes.CountAsync(c => c.Estado);
            var totalPedidos = await _context.Pedidos.CountAsync(p => p.Estado);
            var pedidosPendientes = await _context.Pedidos
                .Include(p => p.EstadoPedido)
                .CountAsync(p => p.Estado && p.EstadoPedido.Nombre != "Entregado");
            var pedidosEntregados = await _context.Pedidos
                .Include(p => p.EstadoPedido)
                .CountAsync(p => p.Estado && p.EstadoPedido.Nombre == "Entregado");
            var totalCobrado = await _context.Pagos
                .Where(p => p.Estado)
                .SumAsync(p => (decimal?)p.Monto) ?? 0;
            var saldoPendiente = await _context.PedidosResumenPago
                .SumAsync(r => (decimal?)r.SaldoPendiente) ?? 0;
            var totalProductosTerminados = await _context.ProductosTerminados.CountAsync(p => p.Estado);
            var totalInsumos = await _context.Insumos.CountAsync(i => i.Estado);
            var asistenciasRegistradas = await _context.Asistencias.CountAsync();
            var empleadosActivos = await _context.Empleados.CountAsync(e => e.Estado);

            return Result<DashboardDto>.Ok(new DashboardDto
            {
                TotalClientes = totalClientes,
                TotalPedidos = totalPedidos,
                PedidosPendientes = pedidosPendientes,
                PedidosEntregados = pedidosEntregados,
                TotalCobrado = totalCobrado,
                SaldoPendiente = saldoPendiente,
                TotalProductosTerminados = totalProductosTerminados,
                TotalInsumos = totalInsumos,
                AsistenciasRegistradas = asistenciasRegistradas,
                EmpleadosActivos = empleadosActivos
            });
        }

        public async Task<Result<ExportResultDto>> ExportarPedidosAsync(DateTime? desde, DateTime? hasta, int? idEstado, string? prioridad)
        {
            var resultado = await ObtenerReportePedidosAsync(desde, hasta, idEstado, prioridad);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "N°", "Número", "Cliente", "Estado", "Fecha pedido", "Fecha entrega", "Prioridad", "Total (Q)" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.Id, r.NumeroPedido, r.Cliente, r.EstadoPedido, r.FechaPedido, r.FechaEntrega, r.Prioridad, r.Total });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Reporte de Pedidos", tabla, "Pedidos"));
        }

        public async Task<Result<ExportResultDto>> ExportarPagosAsync(DateTime? desde, DateTime? hasta, int? idMetodoPago)
        {
            var resultado = await ObtenerReportePagosAsync(desde, hasta, idMetodoPago);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "ID", "Pedido", "Cliente", "Método de pago", "Fecha pago", "Monto (Q)", "Referencia" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.Id, r.NumeroPedido, r.Cliente, r.MetodoPago, r.FechaPago, r.Monto, r.NumeroReferencia ?? "" });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Reporte de Pagos", tabla, "Pagos"));
        }

        public async Task<Result<ExportResultDto>> ExportarProductosTerminadosAsync(int? idTipoPrenda, bool? soloStockBajo)
        {
            var resultado = await ObtenerReporteProductosTerminadosAsync(idTipoPrenda, soloStockBajo);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "ID", "Código", "Nombre", "Tipo prenda", "Talla", "Color", "Stock actual", "Stock mínimo", "Precio (Q)" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.Id, r.Codigo, r.Nombre, r.TipoPrenda ?? "", r.Talla ?? "", r.Color ?? "", r.StockActual, r.StockMinimo, r.PrecioReferencial });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Reporte de Productos Terminados", tabla, "Productos"));
        }

        public async Task<Result<ExportResultDto>> ExportarInsumosAsync(string? categoria, bool? soloStockBajo)
        {
            var resultado = await ObtenerReporteInsumosAsync(categoria, soloStockBajo);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "ID", "Código", "Nombre", "Categoría", "Unidad", "Stock actual", "Stock mínimo", "Costo (Q)" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.Id, r.Codigo, r.Nombre, r.Categoria ?? "", r.UnidadMedida ?? "", r.StockActual, r.StockMinimo, r.CostoReferencial });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Reporte de Insumos", tabla, "Insumos"));
        }

        public async Task<Result<ExportResultDto>> ExportarAsistenciasAsync(DateTime? desde, DateTime? hasta)
        {
            var resultado = await ObtenerReporteAsistenciasAsync(desde, hasta);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "ID", "Empleado", "Fecha", "Hora entrada", "Hora salida", "Tardanza (min)", "Horas trabajadas", "Observaciones" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.Id, r.Empleado, r.Fecha, r.HoraEntrada.HasValue ? r.HoraEntrada.Value.ToString(@"hh\:mm") : "", r.HoraSalida.HasValue ? r.HoraSalida.Value.ToString(@"hh\:mm") : "", r.TardanzaMinutos, r.HorasTrabajadas, r.Observaciones ?? "" });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Reporte de Asistencias", tabla, "Asistencias"));
        }

        public async Task<Result<List<PagoPorClienteDto>>> ObtenerPagosPorClienteAsync(DateTime? desde, DateTime? hasta)
        {
            var consulta = _context.Pagos
                .Where(p => p.Estado)
                .AsQueryable();

            if (desde.HasValue)
                consulta = consulta.Where(p => p.FechaPago >= desde.Value.Date);
            if (hasta.HasValue)
                consulta = consulta.Where(p => p.FechaPago < hasta.Value.Date.AddDays(1));

            var reportes = await consulta
                .GroupBy(p => new { p.IdCliente, p.Cliente.NombreRazonSocial })
                .Select(g => new PagoPorClienteDto
                {
                    IdCliente = g.Key.IdCliente,
                    Cliente = g.Key.NombreRazonSocial,
                    CantidadPagos = g.Count(),
                    TotalPagado = g.Sum(p => p.Monto)
                })
                .OrderByDescending(x => x.TotalPagado)
                .ToListAsync();

            return Result<List<PagoPorClienteDto>>.Ok(reportes);
        }

        public async Task<Result<List<CuentaPorCobrarDto>>> ObtenerCuentasPorCobrarAsync()
        {
            var reportes = await _context.PedidosResumenPago
                .Where(r => r.SaldoPendiente > 0 && r.Pedido.Estado)
                .OrderByDescending(r => r.SaldoPendiente)
                .Select(r => new CuentaPorCobrarDto
                {
                    IdPedido = r.IdPedido,
                    NumeroPedido = r.Pedido.NumeroPedido,
                    Cliente = r.Pedido.Cliente.NombreRazonSocial,
                    TotalPedido = r.TotalPedido,
                    TotalPagado = r.TotalPagado,
                    SaldoPendiente = r.SaldoPendiente,
                    FechaEntrega = r.Pedido.FechaEntrega,
                    EstadoFinanciero = r.EstadoFinanciero.Nombre
                })
                .ToListAsync();

            return Result<List<CuentaPorCobrarDto>>.Ok(reportes);
        }

        public async Task<Result<List<IngresoPeriodoDto>>> ObtenerIngresosPorPeriodoAsync(DateTime? desde, DateTime? hasta)
        {
            var consulta = _context.Pagos
                .Where(p => p.Estado)
                .AsQueryable();

            if (desde.HasValue)
                consulta = consulta.Where(p => p.FechaPago >= desde.Value.Date);
            if (hasta.HasValue)
                consulta = consulta.Where(p => p.FechaPago < hasta.Value.Date.AddDays(1));

            var reportes = await consulta
                .GroupBy(p => p.FechaPago.Date)
                .Select(g => new IngresoPeriodoDto
                {
                    Fecha = g.Key,
                    CantidadPagos = g.Count(),
                    Monto = g.Sum(p => p.Monto)
                })
                .OrderBy(x => x.Fecha)
                .ToListAsync();

            return Result<List<IngresoPeriodoDto>>.Ok(reportes);
        }

        public async Task<Result<List<PagoVencidoDto>>> ObtenerPagosVencidosAsync()
        {
            var hoy = DateTime.Today;

            var reportes = await _context.PedidosResumenPago
                .Where(r => r.SaldoPendiente > 0 && r.Pedido.Estado && r.Pedido.FechaEntrega.Date < hoy)
                .OrderBy(r => r.Pedido.FechaEntrega)
                .Select(r => new PagoVencidoDto
                {
                    IdPedido = r.IdPedido,
                    NumeroPedido = r.Pedido.NumeroPedido,
                    Cliente = r.Pedido.Cliente.NombreRazonSocial,
                    EstadoPedido = r.Pedido.EstadoPedido.Nombre,
                    TotalPedido = r.TotalPedido,
                    TotalPagado = r.TotalPagado,
                    SaldoPendiente = r.SaldoPendiente,
                    FechaEntrega = r.Pedido.FechaEntrega,
                    DiasAtraso = EF.Functions.DateDiffDay(r.Pedido.FechaEntrega, hoy)
                })
                .ToListAsync();

            return Result<List<PagoVencidoDto>>.Ok(reportes);
        }

        public async Task<Result<ExportResultDto>> ExportarPagosPorClienteAsync(DateTime? desde, DateTime? hasta)
        {
            var resultado = await ObtenerPagosPorClienteAsync(desde, hasta);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "ID cliente", "Cliente", "Cantidad de pagos", "Total pagado (Q)" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.IdCliente, r.Cliente, r.CantidadPagos, r.TotalPagado });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Pagos por Cliente", tabla, "PagosCliente"));
        }

        public async Task<Result<ExportResultDto>> ExportarCuentasPorCobrarAsync()
        {
            var resultado = await ObtenerCuentasPorCobrarAsync();
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "Pedido", "Número", "Cliente", "Total (Q)", "Pagado (Q)", "Saldo (Q)", "Fecha entrega", "Estado financiero" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.IdPedido, r.NumeroPedido, r.Cliente, r.TotalPedido, r.TotalPagado, r.SaldoPendiente, r.FechaEntrega, r.EstadoFinanciero });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Cuentas por Cobrar", tabla, "CuentasCobrar"));
        }

        public async Task<Result<ExportResultDto>> ExportarIngresosPeriodoAsync(DateTime? desde, DateTime? hasta)
        {
            var resultado = await ObtenerIngresosPorPeriodoAsync(desde, hasta);
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "Fecha", "Cantidad de pagos", "Monto (Q)" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.Fecha, r.CantidadPagos, r.Monto });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Ingresos por Período", tabla, "IngresosPeriodo"));
        }

        public async Task<Result<ExportResultDto>> ExportarPagosVencidosAsync()
        {
            var resultado = await ObtenerPagosVencidosAsync();
            if (!resultado.Success)
                return Result<ExportResultDto>.Fail(resultado.Error ?? "Error al generar el reporte");

            var tabla = new List<object[]> { new object[] { "Pedido", "Número", "Cliente", "Estado", "Total (Q)", "Pagado (Q)", "Saldo (Q)", "Fecha entrega", "Días atraso" } };
            foreach (var r in resultado.Data!)
            {
                tabla.Add(new object[] { r.IdPedido, r.NumeroPedido, r.Cliente, r.EstadoPedido, r.TotalPedido, r.TotalPagado, r.SaldoPendiente, r.FechaEntrega, r.DiasAtraso });
            }

            return Result<ExportResultDto>.Ok(ExcelHelper.CrearLibro("Pagos Vencidos", tabla, "PagosVencidos"));
        }
    }
}
