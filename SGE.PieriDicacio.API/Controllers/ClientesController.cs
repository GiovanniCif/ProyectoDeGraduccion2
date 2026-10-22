using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        [RequierePermiso("Ver Clientes")]
        [ProducesResponseType(typeof(List<DTOs.Cliente.ClienteResponseDto>), 200)]
        public async Task<IActionResult> ObtenerTodos()
        {
            var result = await _clienteService.ObtenerTodosAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [RequierePermiso("Ver Clientes")]
        [ProducesResponseType(typeof(DTOs.Cliente.ClienteResponseDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _clienteService.ObtenerPorIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPost]
        [RequierePermiso("Gestionar Clientes")]
        [ProducesResponseType(typeof(DTOs.Cliente.ClienteResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Crear([FromBody] DTOs.Cliente.ClienteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _clienteService.CrearAsync(dto);
            return result.Success ? Ok(result.Data) : BadRequest(new { mensaje = result.Error });
        }

        [HttpPut("{id:int}")]
        [RequierePermiso("Gestionar Clientes")]
        [ProducesResponseType(typeof(DTOs.Cliente.ClienteResponseDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] DTOs.Cliente.ClienteUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _clienteService.ActualizarAsync(id, dto);
            return result.Success ? Ok(result.Data) : NotFound(new { mensaje = result.Error });
        }

        [HttpPatch("{id:int}/estado")]
        [RequierePermiso("Gestionar Clientes")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var result = await _clienteService.CambiarEstadoAsync(id);
            return result.Success
                ? Ok(new { mensaje = "Estado del cliente actualizado correctamente" })
                : NotFound(new { mensaje = result.Error });
        }
    }
}
