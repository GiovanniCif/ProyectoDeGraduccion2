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
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost]
        [RequierePermiso("Registrar Pagos")]
        [ProducesResponseType(typeof(DTOs.Pago.PagoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegistrarPago([FromBody] DTOs.Pago.PagoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _pagoService.RegistrarPagoAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("pedido/{idPedido:int}")]
        [RequierePermiso("Ver Pagos")]
        [ProducesResponseType(typeof(List<DTOs.Pago.PagoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerPagosPorPedido(int idPedido)
        {
            var result = await _pagoService.ObtenerPagosPorPedidoAsync(idPedido);
            return Ok(result.Data);
        }

        [HttpGet("pedido/{idPedido:int}/resumen")]
        [RequierePermiso("Ver Pagos")]
        [ProducesResponseType(typeof(DTOs.Pago.PedidoResumenPagoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerResumenPago(int idPedido)
        {
            var result = await _pagoService.ObtenerResumenPagoAsync(idPedido);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }
    }
}
