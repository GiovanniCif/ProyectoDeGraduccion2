using SGE.PieriDicacio.API.DTOs.Inventario;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IInventarioService
    {
        Task<Result<List<ProductoTerminadoResponseDto>>> ObtenerProductosTerminadosAsync();
        Task<Result<ProductoTerminadoResponseDto>> CrearProductoTerminadoAsync(ProductoTerminadoCreateDto dto);
        Task<Result<ProductoTerminadoResponseDto>> ObtenerProductoPorId(int id);
        Task<Result<MovimientoProductoTerminadoResponseDto>> RegistrarMovimientoProductoTerminadoAsync(MovimientoProductoTerminadoCreateDto dto, int idUsuario);

        Task<Result<List<InsumoResponseDto>>> ObtenerInsumosAsync();
        Task<Result<InsumoResponseDto>> CrearInsumoAsync(InsumoCreateDto dto);
        Task<Result<InsumoResponseDto>> ObtenerInsumoPorId(int id);
        Task<Result<MovimientoInsumoResponseDto>> RegistrarMovimientoInsumoAsync(MovimientoInsumoCreateDto dto, int idUsuario);

        Task<Result<KardexProductoTerminadoDto>> ObtenerKardexProductoTerminadoAsync(int id, DateTime? desde, DateTime? hasta);
        Task<Result<KardexInsumoDto>> ObtenerKardexInsumoAsync(int id, DateTime? desde, DateTime? hasta);
        Task<Result<List<AlertaStockResponseDto>>> ObtenerAlertasStockAsync();

        Task<Result<bool>> DescontarStockPorEntregaAsync(int idPedido, int idUsuario);
    }
}
