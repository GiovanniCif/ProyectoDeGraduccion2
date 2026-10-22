using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadosController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        [HttpGet]
        [RequierePermiso("Ver Empleados")]
        [ProducesResponseType(typeof(List<DTOs.Empleado.EmpleadoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerEmpleados()
        {
            var result = await _empleadoService.ObtenerEmpleadosAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [RequierePermiso("Ver Empleados")]
        [ProducesResponseType(typeof(DTOs.Empleado.EmpleadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _empleadoService.ObtenerEmpleadoPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Gestionar Empleados")]
        [ProducesResponseType(typeof(DTOs.Empleado.EmpleadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CrearEmpleado([FromBody] DTOs.Empleado.EmpleadoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _empleadoService.CrearEmpleadoAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPut("{id:int}")]
        [RequierePermiso("Gestionar Empleados")]
        [ProducesResponseType(typeof(DTOs.Empleado.EmpleadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ActualizarEmpleado(int id, [FromBody] DTOs.Empleado.EmpleadoUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _empleadoService.ActualizarEmpleadoAsync(id, dto);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPatch("{id:int}/estado")]
        [RequierePermiso("Gestionar Empleados")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> CambiarEstadoEmpleado(int id)
        {
            var result = await _empleadoService.CambiarEstadoEmpleadoAsync(id);
            return result.Success
                ? Ok(new { mensaje = "Estado del empleado actualizado correctamente" })
                : NotFound(new { mensaje = result.Error });
        }

        [HttpGet("asistencias")]
        [RequierePermiso("Ver Empleados")]
        [ProducesResponseType(typeof(List<DTOs.Empleado.AsistenciaResponseDto>), 200)]
        public async Task<IActionResult> ObtenerAsistencias()
        {
            var result = await _empleadoService.ObtenerAsistenciasAsync();
            return Ok(result.Data);
        }

        [HttpPost("asistencias")]
        [RequierePermiso("Registrar Asistencia")]
        [ProducesResponseType(typeof(DTOs.Empleado.AsistenciaResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegistrarAsistencia([FromBody] DTOs.Empleado.AsistenciaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _empleadoService.RegistrarAsistenciaAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("permisos")]
        [RequierePermiso("Ver Empleados")]
        [ProducesResponseType(typeof(List<DTOs.Empleado.PermisoEmpleadoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerPermisos()
        {
            var result = await _empleadoService.ObtenerPermisosAsync();
            return Ok(result.Data);
        }

        [HttpGet("asistencias/resumen")]
        [RequierePermiso("Ver Empleados")]
        [ProducesResponseType(typeof(List<DTOs.Empleado.AsistenciaResumenDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ObtenerResumenAsistencias([FromQuery] int anio, [FromQuery] int mes)
        {
            var result = await _empleadoService.ObtenerResumenAsistenciasAsync(anio, mes);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("asistencias/resumen/exportar")]
        [RequierePermiso("Ver Empleados")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ExportarResumenAsistencias([FromQuery] int anio, [FromQuery] int mes)
        {
            var result = await _empleadoService.ExportarResumenAsistenciasAsync(anio, mes);
            return result.Success
                ? File(result.Data!.Contenido, result.Data.TipoContenido, result.Data.NombreArchivo)
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpPost("permisos")]
        [RequierePermiso("Gestionar Empleados")]
        [ProducesResponseType(typeof(DTOs.Empleado.PermisoEmpleadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegistrarPermiso([FromBody] DTOs.Empleado.PermisoEmpleadoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _empleadoService.RegistrarPermisoAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }
    }
}
