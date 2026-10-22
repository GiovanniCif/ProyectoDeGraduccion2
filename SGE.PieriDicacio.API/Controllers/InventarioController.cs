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
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpGet("productos-terminados")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(List<DTOs.Inventario.ProductoTerminadoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerProductosTerminados()
        {
            var result = await _inventarioService.ObtenerProductosTerminadosAsync();
            return Ok(result.Data);
        }

        [HttpGet("productos-terminados/{id:int}")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.ProductoTerminadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerProductoTerminado(int id)
        {
            var result = await _inventarioService.ObtenerProductoPorId(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost("productos-terminados")]
        [RequierePermiso("Gestionar Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.ProductoTerminadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CrearProductoTerminado([FromBody] DTOs.Inventario.ProductoTerminadoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _inventarioService.CrearProductoTerminadoAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPut("productos-terminados/{id:int}")]
        [RequierePermiso("Gestionar Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.ProductoTerminadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ActualizarProductoTerminado(int id, [FromBody] DTOs.Inventario.ProductoTerminadoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _inventarioService.CrearProductoTerminadoAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPost("productos-terminados/movimientos")]
        [RequierePermiso("Gestionar Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.MovimientoProductoTerminadoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegistrarMovimientoProductoTerminado([FromBody] DTOs.Inventario.MovimientoProductoTerminadoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _inventarioService.RegistrarMovimientoProductoTerminadoAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("insumos")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(List<DTOs.Inventario.InsumoResponseDto>), 200)]
        public async Task<IActionResult> ObtenerInsumos()
        {
            var result = await _inventarioService.ObtenerInsumosAsync();
            return Ok(result.Data);
        }

        [HttpGet("alertas-stock")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(List<DTOs.Inventario.AlertaStockResponseDto>), 200)]
        public async Task<IActionResult> ObtenerAlertasStock()
        {
            var result = await _inventarioService.ObtenerAlertasStockAsync();
            return Ok(result.Data);
        }

        [HttpGet("productos-terminados/kardex/{id:int}")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.KardexProductoTerminadoDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerKardexProductoTerminado(int id, [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta)
        {
            var result = await _inventarioService.ObtenerKardexProductoTerminadoAsync(id, desde?.ToDateTime(TimeOnly.MinValue), hasta?.ToDateTime(TimeOnly.MinValue));
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpGet("insumos/kardex/{id:int}")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.KardexInsumoDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerKardexInsumo(int id, [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta)
        {
            var result = await _inventarioService.ObtenerKardexInsumoAsync(id, desde?.ToDateTime(TimeOnly.MinValue), hasta?.ToDateTime(TimeOnly.MinValue));
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpGet("insumos/{id:int}")]
        [RequierePermiso("Ver Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.InsumoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerInsumo(int id)
        {
            var result = await _inventarioService.ObtenerInsumoPorId(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost("insumos")]
        [RequierePermiso("Gestionar Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.InsumoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CrearInsumo([FromBody] DTOs.Inventario.InsumoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _inventarioService.CrearInsumoAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPost("insumos/movimientos")]
        [RequierePermiso("Gestionar Inventario")]
        [ProducesResponseType(typeof(DTOs.Inventario.MovimientoInsumoResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegistrarMovimientoInsumo([FromBody] DTOs.Inventario.MovimientoInsumoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var idUsuario = HttpContext.GetUserIdFromHttpContext();
            if (idUsuario == 0)
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            var result = await _inventarioService.RegistrarMovimientoInsumoAsync(dto, idUsuario);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }
    }
}
