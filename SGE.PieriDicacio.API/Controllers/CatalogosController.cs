using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Authz;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.Entities;
using System.Text.RegularExpressions;

namespace SGE.PieriDicacio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CatalogosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("tipos-prenda")]
        public async Task<ActionResult<List<TipoPrenda>>> GetTiposPrenda()
        {
            var tipos = await _context.TiposPrenda
                .Where(t => t.Estado)
                .OrderBy(t => t.Nombre)
                .ToListAsync();
            return Ok(tipos);
        }

        [HttpGet("tallas")]
        public async Task<ActionResult<List<Talla>>> GetTallas()
        {
            var tallas = await _context.Tallas
                .Where(t => t.Estado)
                .OrderBy(t => t.Nombre)
                .ToListAsync();
            return Ok(tallas);
        }

        [HttpGet("colores")]
        public async Task<ActionResult<List<Color>>> GetColores()
        {
            var colores = await _context.Colores
                .Where(c => c.Estado)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            return Ok(colores);
        }

        [HttpGet("unidades-medida")]
        public async Task<ActionResult<List<UnidadMedida>>> GetUnidadesMedida()
        {
            var unidades = await _context.UnidadesMedida
                .Where(u => u.Estado)
                .OrderBy(u => u.Nombre)
                .ToListAsync();
            return Ok(unidades);
        }

        [HttpGet("estados-pedido")]
        public async Task<ActionResult<List<DTOs.Pedido.EstadoPedidoResponseDto>>> GetEstadosPedido()
        {
            var estados = await _context.EstadosPedido
                .OrderBy(e => e.Orden)
                .Select(e => new DTOs.Pedido.EstadoPedidoResponseDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    Orden = e.Orden,
                    Estado = e.Estado
                })
                .ToListAsync();
            return Ok(estados);
        }

        [HttpGet("departamentos")]
        public async Task<ActionResult<List<Departamento>>> GetDepartamentos()
        {
            var departamentos = await _context.Departamentos
                .Where(d => d.Estado)
                .OrderBy(d => d.Nombre)
                .ToListAsync();
            return Ok(departamentos);
        }

        private string? ValidarNombre(string? nombre, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre es obligatorio.";

            var nombreLimpio = nombre.Trim();

            if (nombreLimpio.Length < 1)
                return "El nombre debe tener al menos 1 carácter.";

            if (nombreLimpio.Length > maxLength)
                return $"El nombre no puede exceder {maxLength} caracteres.";

            var caracteresInvalidos = new[] { '<', '>', ';', '"', '\\', '/', '\t', '\n', '\r' };
            if (nombreLimpio.Any(c => caracteresInvalidos.Contains(c)))
                return "El nombre contiene caracteres no permitidos.";

            return null;
        }

        private async Task<bool> ExisteDuplicadoAsync(string nombre, string tipoCatalogo)
        {
            var nombreNormalizado = nombre.Trim().ToLower();

            return tipoCatalogo switch
            {
                "tipos-prenda" => await _context.TiposPrenda.AnyAsync(t => t.Nombre.Trim().ToLower() == nombreNormalizado),
                "tallas" => await _context.Tallas.AnyAsync(t => t.Nombre.Trim().ToLower() == nombreNormalizado),
                "colores" => await _context.Colores.AnyAsync(c => c.Nombre.Trim().ToLower() == nombreNormalizado),
                "unidades-medida" => await _context.UnidadesMedida.AnyAsync(u => u.Nombre.Trim().ToLower() == nombreNormalizado),
                _ => false
            };
        }

        private string ObtenerNombreTipo(string tipoCatalogo)
        {
            return tipoCatalogo switch
            {
                "tipos-prenda" => "tipo de prenda",
                "tallas" => "talla",
                "colores" => "color",
                "unidades-medida" => "unidad de medida",
                _ => "elemento"
            };
        }

        [HttpPost("tipos-prenda")]
        [RequierePermiso("Gestionar Catálogos")]
        public async Task<ActionResult<TipoPrenda>> CreateTipoPrenda([FromBody] CreateCatalogoDto dto)
        {
            var errorValidacion = ValidarNombre(dto.Nombre);
            if (errorValidacion != null)
                return BadRequest(new { mensaje = errorValidacion });

            var nombreLimpio = dto.Nombre.Trim();
            var tipoNombre = ObtenerNombreTipo("tipos-prenda");

            var existe = await ExisteDuplicadoAsync(nombreLimpio, "tipos-prenda");
            if (existe)
                return Conflict(new { mensaje = $"Ya existe un {tipoNombre} con el nombre \"{nombreLimpio}\". Por favor ingresa uno diferente." });

            var tipo = new TipoPrenda
            {
                Nombre = nombreLimpio,
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Estado = true
            };

            _context.TiposPrenda.Add(tipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTiposPrenda), new { id = tipo.Id }, tipo);
        }

        [HttpPost("tallas")]
        [RequierePermiso("Gestionar Catálogos")]
        public async Task<ActionResult<Talla>> CreateTalla([FromBody] CreateCatalogoDto dto)
        {
            var errorValidacion = ValidarNombre(dto.Nombre);
            if (errorValidacion != null)
                return BadRequest(new { mensaje = errorValidacion });

            var nombreLimpio = dto.Nombre.Trim();
            var tipoNombre = ObtenerNombreTipo("tallas");

            var existe = await ExisteDuplicadoAsync(nombreLimpio, "tallas");
            if (existe)
                return Conflict(new { mensaje = $"Ya existe una {tipoNombre} con el nombre \"{nombreLimpio}\". Por favor ingresa una diferente." });

            var talla = new Talla
            {
                Nombre = nombreLimpio,
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Estado = true
            };

            _context.Tallas.Add(talla);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTallas), new { id = talla.Id }, talla);
        }

        [HttpPost("colores")]
        [RequierePermiso("Gestionar Catálogos")]
        public async Task<ActionResult<Color>> CreateColor([FromBody] CreateCatalogoDto dto)
        {
            var errorValidacion = ValidarNombre(dto.Nombre);
            if (errorValidacion != null)
                return BadRequest(new { mensaje = errorValidacion });

            var nombreLimpio = dto.Nombre.Trim();
            var tipoNombre = ObtenerNombreTipo("colores");

            var existe = await ExisteDuplicadoAsync(nombreLimpio, "colores");
            if (existe)
                return Conflict(new { mensaje = $"Ya existe un {tipoNombre} con el nombre \"{nombreLimpio}\". Por favor ingresa uno diferente." });

            var color = new Color
            {
                Nombre = nombreLimpio,
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Estado = true
            };

            _context.Colores.Add(color);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetColores), new { id = color.Id }, color);
        }

        [HttpPost("unidades-medida")]
        [RequierePermiso("Gestionar Catálogos")]
        public async Task<ActionResult<UnidadMedida>> CreateUnidadMedida([FromBody] CreateUnidadMedidaDto dto)
        {
            var errorValidacion = ValidarNombre(dto.Nombre);
            if (errorValidacion != null)
                return BadRequest(new { mensaje = errorValidacion });

            var nombreLimpio = dto.Nombre.Trim();
            var tipoNombre = ObtenerNombreTipo("unidades-medida");

            var existe = await ExisteDuplicadoAsync(nombreLimpio, "unidades-medida");
            if (existe)
                return Conflict(new { mensaje = $"Ya existe una {tipoNombre} con el nombre \"{nombreLimpio}\". Por favor ingresa una diferente." });

            var abreviaturaLimpia = string.IsNullOrWhiteSpace(dto.Abreviatura)
                ? nombreLimpio.Length <= 3 ? nombreLimpio.ToUpper() : nombreLimpio.Substring(0, Math.Min(3, nombreLimpio.Length)).ToUpper()
                : dto.Abreviatura.Trim();

            if (abreviaturaLimpia.Length > 20)
                abreviaturaLimpia = abreviaturaLimpia.Substring(0, 20);

            var unidad = new UnidadMedida
            {
                Nombre = nombreLimpio,
                Abreviatura = abreviaturaLimpia,
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Estado = true
            };

            _context.UnidadesMedida.Add(unidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUnidadesMedida), new { id = unidad.Id }, unidad);
        }
    }

    public class CreateCatalogoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class CreateUnidadMedidaDto : CreateCatalogoDto
    {
        public string? Abreviatura { get; set; }
    }
}
