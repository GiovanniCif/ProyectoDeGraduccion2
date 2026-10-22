using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IBitacoraService
    {
        Task<Result<BitacoraResponseDto>> RegistrarAsync(BitacoraCreateDto dto, int idUsuario);
        Task<Result<List<BitacoraResponseDto>>> ObtenerTodosAsync();
    }
}
