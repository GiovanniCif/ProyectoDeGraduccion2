using SGE.PieriDicacio.API.DTOs.Rol;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IRolService
    {
        Task<Result<List<RolResponseDto>>> ObtenerTodosAsync();
        Task<Result<RolResponseDto>> ObtenerPorIdAsync(int id);
        Task<Result<RolResponseDto>> CrearAsync(RolCreateDto dto);
        Task<Result<RolResponseDto>> ActualizarAsync(int id, RolUpdateDto dto);
        Task<Result<bool>> CambiarEstadoAsync(int id);
    }
}
