using SGE.PieriDicacio.API.DTOs.Pedido;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IPedidoService
    {
        Task<Result<List<PedidoResponseDto>>> ObtenerTodosAsync();
        Task<Result<PedidoResponseDto>> ObtenerPorIdAsync(int id);
        Task<Result<PedidoResponseDto>> CrearAsync(PedidoCreateDto dto);
        Task<Result<bool>> CambiarEstadoAsync(int idPedido, CambiarEstadoPedidoDto dto, int idUsuario);
        Task<Result<List<PedidoHistorialResponseDto>>> ObtenerHistorialAsync(int idPedido);
        Task<Result<List<PedidoAlertaResponseDto>>> ObtenerAlertasAsync();
    }
}
