using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.DTOs.BOM;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequierePermiso("Gestionar Lista de Materiales")]
    public class BOMController : ControllerBase
    {
        private readonly IBOMService _bomService;

        public BOMController(IBOMService bomService)
        {
            _bomService = bomService;
        }

        [HttpGet("estados-orden-produccion")]
        [ProducesResponseType(typeof(List<EstadoOrdenProduccionResponseDto>), 200)]
        public async Task<IActionResult> ObtenerEstadosOrdenProduccion()
        {
            var result = await _bomService.ObtenerEstadosOrdenProduccionAsync();
            return Ok(result.Data);
        }

        [HttpGet("por-producto/{idProductoTerminado:int}")]
        [ProducesResponseType(typeof(List<BOMDetalleResponseDto>), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerBOMPorProducto(int idProductoTerminado)
        {
            var result = await _bomService.ObtenerBOMPorProductoAsync(idProductoTerminado);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BOMDetalleResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerBOMPorId(int id)
        {
            var result = await _bomService.ObtenerBOMPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Gestionar Lista de Materiales")]
        [ProducesResponseType(typeof(BOMDetalleResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> CrearBOMDetalle([FromBody] BOMDetalleCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _bomService.CrearBOMDetalleAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPut("{id:int}")]
        [RequierePermiso("Gestionar Lista de Materiales")]
        [ProducesResponseType(typeof(BOMDetalleResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> ActualizarBOMDetalle(int id, [FromBody] BOMDetalleUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _bomService.ActualizarBOMDetalleAsync(id, dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpDelete("{id:int}")]
        [RequierePermiso("Gestionar Lista de Materiales")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> EliminarBOMDetalle(int id)
        {
            var result = await _bomService.EliminarBOMDetalleAsync(id);
            return result.Success ? Ok(new { mensaje = "Detalle eliminado correctamente" }) : BadRequest(new { mensaje = result.Error });
        }

        [HttpGet("verificar-disponibilidad/{idProductoTerminado:int}")]
        [ProducesResponseType(typeof(List<BOMDetalleResponseDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> VerificarDisponibilidadInsumos(int idProductoTerminado, [FromQuery] decimal cantidad)
        {
            if (cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0" });

            var result = await _bomService.VerificarDisponibilidadInsumosAsync(idProductoTerminado, cantidad);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }
    }
}
