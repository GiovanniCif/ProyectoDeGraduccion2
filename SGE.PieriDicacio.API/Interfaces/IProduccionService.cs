using SGE.PieriDicacio.API.DTOs.BOM;
using SGE.PieriDicacio.API.DTOs.Produccion;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IProduccionService
    {
        Task<Result<List<OrdenProduccionResponseDto>>> ObtenerOrdenesProduccionAsync();
        Task<Result<OrdenProduccionResponseDto>> ObtenerOrdenPorIdAsync(int id);
        Task<Result<OrdenProduccionResponseDto>> CrearOrdenProduccionAsync(OrdenProduccionCreateDto dto, int idUsuario);
        Task<Result<OrdenProduccionResponseDto>> CambiarEstadoOrdenAsync(int id, int idEstadoNuevo, int idUsuario, string? comentario);
        
        Task<Result<List<BOMDetalleResponseDto>>> VerificarDisponibilidadParaProduccionAsync(int idOrdenProduccion);
        Task<Result<OrdenProduccionResponseDto>> RealizarProduccionAsync(RealizarProduccionDto dto, int idUsuario);
    }
}
