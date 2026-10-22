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
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [RequierePermiso("Ver Pedidos")]
        [ProducesResponseType(typeof(List<DTOs.Pedido.PedidoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _pedidoService.ObtenerTodosAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [RequierePermiso("Ver Pedidos")]
        [ProducesResponseType(typeof(DTOs.Pedido.PedidoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _pedidoService.ObtenerPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Crear Pedidos")]
        [ProducesResponseType(typeof(DTOs.Pedido.PedidoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Crear([FromBody] DTOs.Pedido.PedidoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _pedidoService.CrearAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPatch("{id:int}/estado")]
        [RequierePermiso("Cambiar Estado Pedido")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] DTOs.Pedido.CambiarEstadoPedidoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _pedidoService.CambiarEstadoAsync(id, dto, idUsuario);
            return result.Success
                ? Ok(new { mensaje = "Estado del pedido actualizado correctamente" })
                : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("alertas")]
        [RequierePermiso("Ver Pedidos")]
        [ProducesResponseType(typeof(List<DTOs.Pedido.PedidoAlertaResponseDto>), 200)]
        public async Task<IActionResult> ObtenerAlertas()
        {
            var result = await _pedidoService.ObtenerAlertasAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}/historial")]
        [RequierePermiso("Ver Pedidos")]
        [ProducesResponseType(typeof(List<DTOs.Pedido.PedidoHistorialResponseDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerHistorial(int id)
        {
            var result = await _pedidoService.ObtenerHistorialAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }
    }
}
