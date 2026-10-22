using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequierePermiso("Ver Reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("pedidos")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.ReportePedidoDto>), 200)]
        public async Task<IActionResult> ObtenerReportePedidos(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int? idEstado,
            [FromQuery] string? prioridad)
        {
            var result = await _reporteService.ObtenerReportePedidosAsync(desde, hasta, idEstado, prioridad);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pedidos/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarPedidos(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int? idEstado,
            [FromQuery] string? prioridad)
        {
            var result = await _reporteService.ExportarPedidosAsync(desde, hasta, idEstado, prioridad);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pagos")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.ReportePagoDto>), 200)]
        public async Task<IActionResult> ObtenerReportePagos(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int? idMetodoPago)
        {
            var result = await _reporteService.ObtenerReportePagosAsync(desde, hasta, idMetodoPago);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pagos/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarPagos(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int? idMetodoPago)
        {
            var result = await _reporteService.ExportarPagosAsync(desde, hasta, idMetodoPago);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("productos-terminados")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.ReporteProductoTerminadoDto>), 200)]
        public async Task<IActionResult> ObtenerReporteProductosTerminados(
            [FromQuery] int? idTipoPrenda,
            [FromQuery] bool? soloStockBajo)
        {
            var result = await _reporteService.ObtenerReporteProductosTerminadosAsync(idTipoPrenda, soloStockBajo);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("productos-terminados/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarProductosTerminados(
            [FromQuery] int? idTipoPrenda,
            [FromQuery] bool? soloStockBajo)
        {
            var result = await _reporteService.ExportarProductosTerminadosAsync(idTipoPrenda, soloStockBajo);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("insumos")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.ReporteInsumoDto>), 200)]
        public async Task<IActionResult> ObtenerReporteInsumos(
            [FromQuery] string? categoria,
            [FromQuery] bool? soloStockBajo)
        {
            var result = await _reporteService.ObtenerReporteInsumosAsync(categoria, soloStockBajo);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("insumos/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarInsumos(
            [FromQuery] string? categoria,
            [FromQuery] bool? soloStockBajo)
        {
            var result = await _reporteService.ExportarInsumosAsync(categoria, soloStockBajo);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("asistencias")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.ReporteAsistenciaDto>), 200)]
        public async Task<IActionResult> ObtenerReporteAsistencias(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var result = await _reporteService.ObtenerReporteAsistenciasAsync(desde, hasta);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("asistencias/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarAsistencias(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var result = await _reporteService.ExportarAsistenciasAsync(desde, hasta);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pagos-por-cliente")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.PagoPorClienteDto>), 200)]
        public async Task<IActionResult> ObtenerPagosPorCliente(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var result = await _reporteService.ObtenerPagosPorClienteAsync(desde, hasta);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pagos-por-cliente/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarPagosPorCliente(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var result = await _reporteService.ExportarPagosPorClienteAsync(desde, hasta);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("cuentas-por-cobrar")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.CuentaPorCobrarDto>), 200)]
        public async Task<IActionResult> ObtenerCuentasPorCobrar()
        {
            var result = await _reporteService.ObtenerCuentasPorCobrarAsync();
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("cuentas-por-cobrar/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarCuentasPorCobrar()
        {
            var result = await _reporteService.ExportarCuentasPorCobrarAsync();
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("ingresos-periodo")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.IngresoPeriodoDto>), 200)]
        public async Task<IActionResult> ObtenerIngresosPorPeriodo(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var result = await _reporteService.ObtenerIngresosPorPeriodoAsync(desde, hasta);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("ingresos-periodo/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarIngresosPeriodo(
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var result = await _reporteService.ExportarIngresosPeriodoAsync(desde, hasta);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pagos-vencidos")]
        [ProducesResponseType(typeof(List<DTOs.Reporte.PagoVencidoDto>), 200)]
        public async Task<IActionResult> ObtenerPagosVencidos()
        {
            var result = await _reporteService.ObtenerPagosVencidosAsync();
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pagos-vencidos/exportar")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarPagosVencidos()
        {
            var result = await _reporteService.ExportarPagosVencidosAsync();
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DTOs.Reporte.DashboardDto), 200)]
        public async Task<IActionResult> ObtenerDashboard()
        {
            var result = await _reporteService.ObtenerDashboardAsync();
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }
    }
}
