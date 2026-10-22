using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequierePermiso("Ver Bitácora")]
    public class BitacoraController : ControllerBase
    {
        private readonly IBitacoraService _bitacoraService;

        public BitacoraController(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DTOs.Bitacora.BitacoraResponseDto>), 200)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _bitacoraService.ObtenerTodosAsync();
            return Ok(result.Data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DTOs.Bitacora.BitacoraResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Registrar([FromBody] DTOs.Bitacora.BitacoraCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _bitacoraService.RegistrarAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }
    }
}
