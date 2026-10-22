using SGE.PieriDicacio.API.DTOs.Reporte;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IReporteService
    {
        Task<Result<List<ReportePedidoDto>>> ObtenerReportePedidosAsync(DateTime? desde, DateTime? hasta, int? idEstado, string? prioridad);
        Task<Result<List<ReportePagoDto>>> ObtenerReportePagosAsync(DateTime? desde, DateTime? hasta, int? idMetodoPago);
        Task<Result<List<ReporteProductoTerminadoDto>>> ObtenerReporteProductosTerminadosAsync(int? idTipoPrenda, bool? soloStockBajo);
        Task<Result<List<ReporteInsumoDto>>> ObtenerReporteInsumosAsync(string? categoria, bool? soloStockBajo);
        Task<Result<List<ReporteAsistenciaDto>>> ObtenerReporteAsistenciasAsync(DateTime? desde, DateTime? hasta);
        Task<Result<DashboardDto>> ObtenerDashboardAsync();

        Task<Result<ExportResultDto>> ExportarPedidosAsync(DateTime? desde, DateTime? hasta, int? idEstado, string? prioridad);
        Task<Result<ExportResultDto>> ExportarPagosAsync(DateTime? desde, DateTime? hasta, int? idMetodoPago);
        Task<Result<ExportResultDto>> ExportarProductosTerminadosAsync(int? idTipoPrenda, bool? soloStockBajo);
        Task<Result<ExportResultDto>> ExportarInsumosAsync(string? categoria, bool? soloStockBajo);
        Task<Result<ExportResultDto>> ExportarAsistenciasAsync(DateTime? desde, DateTime? hasta);

        Task<Result<List<PagoPorClienteDto>>> ObtenerPagosPorClienteAsync(DateTime? desde, DateTime? hasta);
        Task<Result<List<CuentaPorCobrarDto>>> ObtenerCuentasPorCobrarAsync();
        Task<Result<List<IngresoPeriodoDto>>> ObtenerIngresosPorPeriodoAsync(DateTime? desde, DateTime? hasta);
        Task<Result<List<PagoVencidoDto>>> ObtenerPagosVencidosAsync();

        Task<Result<ExportResultDto>> ExportarPagosPorClienteAsync(DateTime? desde, DateTime? hasta);
        Task<Result<ExportResultDto>> ExportarCuentasPorCobrarAsync();
        Task<Result<ExportResultDto>> ExportarIngresosPeriodoAsync(DateTime? desde, DateTime? hasta);
        Task<Result<ExportResultDto>> ExportarPagosVencidosAsync();
    }
}
