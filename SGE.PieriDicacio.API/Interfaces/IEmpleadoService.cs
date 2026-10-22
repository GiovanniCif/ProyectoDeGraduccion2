using SGE.PieriDicacio.API.DTOs.Empleado;
using SGE.PieriDicacio.API.DTOs.Reporte;
using SGE.PieriDicacio.API.Helpers;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IEmpleadoService
    {
        Task<Result<List<EmpleadoResponseDto>>> ObtenerEmpleadosAsync();
        Task<Result<EmpleadoResponseDto>> ObtenerEmpleadoPorIdAsync(int id);
        Task<Result<EmpleadoResponseDto>> CrearEmpleadoAsync(EmpleadoCreateDto dto);
        Task<Result<EmpleadoResponseDto>> ActualizarEmpleadoAsync(int id, EmpleadoUpdateDto dto);
        Task<Result<bool>> CambiarEstadoEmpleadoAsync(int id);

        Task<Result<List<AsistenciaResponseDto>>> ObtenerAsistenciasAsync();
        Task<Result<AsistenciaResponseDto>> RegistrarAsistenciaAsync(AsistenciaCreateDto dto, int idUsuarioRegistro);
        Task<Result<List<AsistenciaResumenDto>>> ObtenerResumenAsistenciasAsync(int anio, int mes);
        Task<Result<ExportResultDto>> ExportarResumenAsistenciasAsync(int anio, int mes);

        Task<Result<List<PermisoEmpleadoResponseDto>>> ObtenerPermisosAsync();
        Task<Result<PermisoEmpleadoResponseDto>> RegistrarPermisoAsync(PermisoEmpleadoCreateDto dto);
    }
}
