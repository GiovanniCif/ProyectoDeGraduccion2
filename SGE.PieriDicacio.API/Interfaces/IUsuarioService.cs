using SGE.PieriDicacio.API.DTOs.Usuario;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IUsuarioService
    {
        Task<Result<List<UsuarioResponseDto>>> ObtenerTodosAsync();
        Task<Result<UsuarioResponseDto>> ObtenerPorIdAsync(int id);
        Task<Result<UsuarioResponseDto>> CrearAsync(UsuarioCreateDto dto);
        Task<Result<UsuarioResponseDto>> ActualizarAsync(int id, UsuarioUpdateDto dto);
        Task<Result<bool>> CambiarEstadoAsync(int id);
    }
}
