using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [RequierePermiso("Gestionar Usuarios")]
        [ProducesResponseType(typeof(List<DTOs.Usuario.UsuarioResponseDto>), 200)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _usuarioService.ObtenerTodosAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [RequierePermiso("Gestionar Usuarios")]
        [ProducesResponseType(typeof(DTOs.Usuario.UsuarioResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _usuarioService.ObtenerPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Gestionar Usuarios")]
        [ProducesResponseType(typeof(DTOs.Usuario.UsuarioResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Crear([FromBody] DTOs.Usuario.UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _usuarioService.CrearAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPut("{id:int}")]
        [RequierePermiso("Gestionar Usuarios")]
        [ProducesResponseType(typeof(DTOs.Usuario.UsuarioResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] DTOs.Usuario.UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _usuarioService.ActualizarAsync(id, dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPatch("{id:int}/estado")]
        [RequierePermiso("Gestionar Usuarios")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var result = await _usuarioService.CambiarEstadoAsync(id);
            return result.Success
                ? Ok(new { mensaje = "Estado del usuario actualizado correctamente" })
                : NotFound(new { mensaje = result.Error });
        }
    }
}
