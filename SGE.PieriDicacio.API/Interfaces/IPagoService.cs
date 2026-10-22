using SGE.PieriDicacio.API.DTOs.Pago;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IPagoService
    {
        Task<Result<PagoResponseDto>> RegistrarPagoAsync(PagoCreateDto dto, int idUsuario);
        Task<Result<List<PagoResponseDto>>> ObtenerPagosPorPedidoAsync(int idPedido);
        Task<Result<PedidoResumenPagoResponseDto>> ObtenerResumenPagoAsync(int idPedido);
    }
}
