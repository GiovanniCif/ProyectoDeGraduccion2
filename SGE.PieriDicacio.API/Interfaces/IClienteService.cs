using SGE.PieriDicacio.API.DTOs.Cliente;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IClienteService
    {
        Task<Result<List<ClienteResponseDto>>> ObtenerTodosAsync();
        Task<Result<ClienteResponseDto>> ObtenerPorIdAsync(int id);
        Task<Result<ClienteResponseDto>> CrearAsync(ClienteCreateDto dto);
        Task<Result<ClienteResponseDto>> ActualizarAsync(int id, ClienteUpdateDto dto);
        Task<Result<bool>> CambiarEstadoAsync(int id);
    }
}
