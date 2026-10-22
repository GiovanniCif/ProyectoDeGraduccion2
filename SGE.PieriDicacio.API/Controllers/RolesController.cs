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
    public class RolesController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DTOs.Rol.RolResponseDto>), 200)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _rolService.ObtenerTodosAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DTOs.Rol.RolResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _rolService.ObtenerPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Gestionar Roles y Permisos")]
        [ProducesResponseType(typeof(DTOs.Rol.RolResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Crear([FromBody] DTOs.Rol.RolCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _rolService.CrearAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPut("{id:int}")]
        [RequierePermiso("Gestionar Roles y Permisos")]
        [ProducesResponseType(typeof(DTOs.Rol.RolResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] DTOs.Rol.RolUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _rolService.ActualizarAsync(id, dto);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPatch("{id:int}/estado")]
        [RequierePermiso("Gestionar Roles y Permisos")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var result = await _rolService.CambiarEstadoAsync(id);
            return result.Success
                ? Ok(new { mensaje = "Estado del rol actualizado correctamente" })
                : NotFound(new { mensaje = result.Error });
        }
    }
}
