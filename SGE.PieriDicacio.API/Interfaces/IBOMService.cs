using SGE.PieriDicacio.API.DTOs.BOM;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IBOMService
    {
        Task<Result<List<EstadoOrdenProduccionResponseDto>>> ObtenerEstadosOrdenProduccionAsync();
        
        Task<Result<List<BOMDetalleResponseDto>>> ObtenerBOMPorProductoAsync(int idProductoTerminado);
        Task<Result<BOMDetalleResponseDto>> ObtenerBOMPorIdAsync(int id);
        Task<Result<BOMDetalleResponseDto>> CrearBOMDetalleAsync(BOMDetalleCreateDto dto);
        Task<Result<BOMDetalleResponseDto>> ActualizarBOMDetalleAsync(int id, BOMDetalleUpdateDto dto);
        Task<Result> EliminarBOMDetalleAsync(int id);
        
        Task<Result<List<BOMDetalleResponseDto>>> VerificarDisponibilidadInsumosAsync(int idProductoTerminado, decimal cantidad);
    }
}
