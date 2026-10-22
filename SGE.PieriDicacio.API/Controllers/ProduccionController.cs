using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.DTOs.BOM;
using SGE.PieriDicacio.API.DTOs.Produccion;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProduccionController : ControllerBase
    {
        private readonly IProduccionService _produccionService;
        private readonly IBOMService _bomService;

        public ProduccionController(IProduccionService produccionService, IBOMService bomService)
        {
            _produccionService = produccionService;
            _bomService = bomService;
        }

        [HttpGet]
        [RequierePermiso("Ver Producción")]
        [ProducesResponseType(typeof(List<DTOs.Produccion.OrdenProduccionResponseDto>), 200)]
        public async Task<IActionResult> ObtenerOrdenesProduccion()
        {
            var result = await _produccionService.ObtenerOrdenesProduccionAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [RequierePermiso("Ver Producción")]
        [ProducesResponseType(typeof(DTOs.Produccion.OrdenProduccionResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerOrdenPorId(int id)
        {
            var result = await _produccionService.ObtenerOrdenPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Gestionar Producción")]
        [ProducesResponseType(typeof(DTOs.Produccion.OrdenProduccionResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CrearOrdenProduccion([FromBody] OrdenProduccionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _produccionService.CrearOrdenProduccionAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPatch("{id:int}/estado")]
        [RequierePermiso("Gestionar Producción")]
        [ProducesResponseType(typeof(DTOs.Produccion.OrdenProduccionResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CambiarEstadoOrden(int id, [FromBody] CambiarEstadoOrdenProduccionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _produccionService.CambiarEstadoOrdenAsync(id, dto.IdEstadoNuevo, idUsuario, dto.Comentario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("{idOrden:int}/verificar-disponibilidad")]
        [RequierePermiso("Ver Producción")]
        [ProducesResponseType(typeof(List<BOMDetalleResponseDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> VerificarDisponibilidadParaProduccion(int idOrden)
        {
            var result = await _produccionService.VerificarDisponibilidadParaProduccionAsync(idOrden);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPost("realizar")]
        [RequierePermiso("Gestionar Producción")]
        [ProducesResponseType(typeof(DTOs.Produccion.OrdenProduccionResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RealizarProduccion([FromBody] RealizarProduccionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _produccionService.RealizarProduccionAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }
    }
}
