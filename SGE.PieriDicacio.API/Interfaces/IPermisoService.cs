using SGE.PieriDicacio.API.DTOs.Permiso;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IPermisoService
    {
        Task<Result<List<PermisoResponseDto>>> ObtenerPermisosAsync();
        Task<Result<List<PermisoResponseDto>>> ObtenerPermisosPorRolAsync(int rolId);
        Task<Result<bool>> AsignarPermisoAsync(int rolId, AsignarPermisoRolDto dto);
        Task<Result<bool>> QuitarPermisoAsync(int rolId, int idPermiso);
    }
}
