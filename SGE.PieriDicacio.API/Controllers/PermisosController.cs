using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.DTOs.Permiso;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequierePermiso("Gestionar Roles y Permisos")]
    public class PermisosController : ControllerBase
    {
        private readonly IPermisoService _permisoService;

        public PermisosController(IPermisoService permisoService)
        {
            _permisoService = permisoService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PermisoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerPermisos()
        {
            var result = await _permisoService.ObtenerPermisosAsync();
            return Ok(result.Data);
        }

        [HttpGet("~/api/Roles/{rolId:int}/permisos")]
        [ProducesResponseType(typeof(List<PermisoResponseDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerPermisosPorRol(int rolId)
        {
            var result = await _permisoService.ObtenerPermisosPorRolAsync(rolId);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost("~/api/Roles/{rolId:int}/permisos")]
        [RequierePermiso("Gestionar Roles y Permisos")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> AsignarPermiso(int rolId, [FromBody] AsignarPermisoRolDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _permisoService.AsignarPermisoAsync(rolId, dto);
            return result.Success
                ? Ok(new { mensaje = "Permiso asignado correctamente" })
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpDelete("~/api/Roles/{rolId:int}/permisos/{idPermiso:int}")]
        [RequierePermiso("Gestionar Roles y Permisos")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> QuitarPermiso(int rolId, int idPermiso)
        {
            var result = await _permisoService.QuitarPermisoAsync(rolId, idPermiso);
            return result.Success
                ? Ok(new { mensaje = "Permiso removido correctamente" })
                : BadRequest(new { mensaje = result.Error });
        }
    }
}
