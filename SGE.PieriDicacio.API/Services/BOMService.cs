using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.BOM;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class BOMService : IBOMService
    {
        private readonly ApplicationDbContext _context;

        public BOMService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<EstadoOrdenProduccionResponseDto>>> ObtenerEstadosOrdenProduccionAsync()
        {
            var estados = await _context.EstadosOrdenProduccion
                .Where(e => e.Estado)
                .OrderBy(e => e.Orden)
                .Select(e => new EstadoOrdenProduccionResponseDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    Orden = e.Orden,
                    Estado = e.Estado
                })
                .ToListAsync();

            return Result<List<EstadoOrdenProduccionResponseDto>>.Ok(estados);
        }

        public async Task<Result<List<BOMDetalleResponseDto>>> ObtenerBOMPorProductoAsync(int idProductoTerminado)
        {
            var producto = await _context.ProductosTerminados
                .FirstOrDefaultAsync(p => p.Id == idProductoTerminado && p.Estado);

            if (producto == null)
                return Result<List<BOMDetalleResponseDto>>.Fail("Producto terminado no encontrado o desactivado");

            var bom = await _context.BOMDetalles
                .Include(b => b.ProductoTerminado)
                .Include(b => b.Insumo)
                .Include(b => b.UnidadMedida)
                .Where(b => b.IdProductoTerminado == idProductoTerminado && b.Estado)
                .OrderBy(b => b.Id)
                .Select(b => new BOMDetalleResponseDto
                {
                    Id = b.Id,
                    IdProductoTerminado = b.IdProductoTerminado,
                    ProductoTerminado = b.ProductoTerminado != null ? b.ProductoTerminado.Nombre : null,
                    ProductoTerminadoCodigo = b.ProductoTerminado != null ? b.ProductoTerminado.Codigo : null,
                    IdInsumo = b.IdInsumo,
                    Insumo = b.Insumo != null ? b.Insumo.Nombre : null,
                    InsumoCodigo = b.Insumo != null ? b.Insumo.Codigo : null,
                    InsumoCategoria = b.Insumo != null ? b.Insumo.Categoria : null,
                    CantidadRequerida = b.CantidadRequerida,
                    IdUnidadMedida = b.IdUnidadMedida,
                    UnidadMedida = b.UnidadMedida != null ? b.UnidadMedida.Nombre : null,
                    UnidadMedidaAbreviatura = b.UnidadMedida != null ? b.UnidadMedida.Abreviatura : null,
                    PorcentajeDesperdicio = b.PorcentajeDesperdicio,
                    Observaciones = b.Observaciones,
                    Estado = b.Estado,
                    FechaCreacion = b.FechaCreacion
                })
                .ToListAsync();

            return Result<List<BOMDetalleResponseDto>>.Ok(bom);
        }

        public async Task<Result<BOMDetalleResponseDto>> ObtenerBOMPorIdAsync(int id)
        {
            var detalle = await _context.BOMDetalles
                .Include(b => b.ProductoTerminado)
                .Include(b => b.Insumo)
                .Include(b => b.UnidadMedida)
                .Where(b => b.Id == id)
                .Select(b => new BOMDetalleResponseDto
                {
                    Id = b.Id,
                    IdProductoTerminado = b.IdProductoTerminado,
                    ProductoTerminado = b.ProductoTerminado != null ? b.ProductoTerminado.Nombre : null,
                    ProductoTerminadoCodigo = b.ProductoTerminado != null ? b.ProductoTerminado.Codigo : null,
                    IdInsumo = b.IdInsumo,
                    Insumo = b.Insumo != null ? b.Insumo.Nombre : null,
                    InsumoCodigo = b.Insumo != null ? b.Insumo.Codigo : null,
                    InsumoCategoria = b.Insumo != null ? b.Insumo.Categoria : null,
                    CantidadRequerida = b.CantidadRequerida,
                    IdUnidadMedida = b.IdUnidadMedida,
                    UnidadMedida = b.UnidadMedida != null ? b.UnidadMedida.Nombre : null,
                    UnidadMedidaAbreviatura = b.UnidadMedida != null ? b.UnidadMedida.Abreviatura : null,
                    PorcentajeDesperdicio = b.PorcentajeDesperdicio,
                    Observaciones = b.Observaciones,
                    Estado = b.Estado,
                    FechaCreacion = b.FechaCreacion
                })
                .FirstOrDefaultAsync();

            return detalle != null
                ? Result<BOMDetalleResponseDto>.Ok(detalle)
                : Result<BOMDetalleResponseDto>.Fail("Detalle de BOM no encontrado");
        }

        public async Task<Result<BOMDetalleResponseDto>> CrearBOMDetalleAsync(BOMDetalleCreateDto dto)
        {
            var producto = await _context.ProductosTerminados
                .FirstOrDefaultAsync(p => p.Id == dto.IdProductoTerminado && p.Estado);

            if (producto == null)
                return Result<BOMDetalleResponseDto>.Fail("Producto terminado no encontrado o desactivado");

            var insumo = await _context.Insumos
                .FirstOrDefaultAsync(i => i.Id == dto.IdInsumo && i.Estado);

            if (insumo == null)
                return Result<BOMDetalleResponseDto>.Fail("Insumo no encontrado o desactivado");

            var existe = await _context.BOMDetalles
                .AnyAsync(b => b.IdProductoTerminado == dto.IdProductoTerminado && 
                               b.IdInsumo == dto.IdInsumo && 
                               b.Estado);

            if (existe)
                return Result<BOMDetalleResponseDto>.Fail("Este insumo ya está en la lista de materiales para este producto");

            var detalle = new BOMDetalle
            {
                IdProductoTerminado = dto.IdProductoTerminado,
                IdInsumo = dto.IdInsumo,
                CantidadRequerida = dto.CantidadRequerida,
                IdUnidadMedida = dto.IdUnidadMedida,
                PorcentajeDesperdicio = dto.PorcentajeDesperdicio,
                Observaciones = dto.Observaciones?.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.BOMDetalles.Add(detalle);
            await _context.SaveChangesAsync();

            return await ObtenerBOMPorIdAsync(detalle.Id);
        }

        public async Task<Result<BOMDetalleResponseDto>> ActualizarBOMDetalleAsync(int id, BOMDetalleUpdateDto dto)
        {
            var detalle = await _context.BOMDetalles
                .FirstOrDefaultAsync(b => b.Id == id);

            if (detalle == null)
                return Result<BOMDetalleResponseDto>.Fail("Detalle de BOM no encontrado");

            if (dto.CantidadRequerida.HasValue)
                detalle.CantidadRequerida = dto.CantidadRequerida.Value;

            if (dto.IdUnidadMedida.HasValue)
                detalle.IdUnidadMedida = dto.IdUnidadMedida.Value;

            if (dto.PorcentajeDesperdicio.HasValue)
                detalle.PorcentajeDesperdicio = dto.PorcentajeDesperdicio.Value;

            if (dto.Observaciones != null)
                detalle.Observaciones = dto.Observaciones?.Trim();

            if (dto.Estado.HasValue)
                detalle.Estado = dto.Estado.Value;

            detalle.FechaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await ObtenerBOMPorIdAsync(detalle.Id);
        }

        public async Task<Result> EliminarBOMDetalleAsync(int id)
        {
            var detalle = await _context.BOMDetalles
                .FirstOrDefaultAsync(b => b.Id == id);

            if (detalle == null)
                return Result.Fail("Detalle de BOM no encontrado");

            detalle.Estado = false;
            detalle.FechaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Result<List<BOMDetalleResponseDto>>> VerificarDisponibilidadInsumosAsync(int idProductoTerminado, decimal cantidad)
        {
            var producto = await _context.ProductosTerminados
                .FirstOrDefaultAsync(p => p.Id == idProductoTerminado && p.Estado);

            if (producto == null)
                return Result<List<BOMDetalleResponseDto>>.Fail("Producto terminado no encontrado o desactivado");

            var bom = await _context.BOMDetalles
                .Include(b => b.Insumo)
                .Include(b => b.UnidadMedida)
                .Where(b => b.IdProductoTerminado == idProductoTerminado && b.Estado)
                .ToListAsync();

            if (bom.Count == 0)
                return Result<List<BOMDetalleResponseDto>>.Ok(new List<BOMDetalleResponseDto>());

            var resultado = new List<BOMDetalleResponseDto>();
            var insuficientes = new List<string>();

            foreach (var item in bom)
            {
                var cantidadConDesperdicio = item.CantidadRequerida * (1 + item.PorcentajeDesperdicio / 100);
                var cantidadTotalNecesaria = cantidadConDesperdicio * cantidad;

                var dto = new BOMDetalleResponseDto
                {
                    Id = item.Id,
                    IdProductoTerminado = item.IdProductoTerminado,
                    IdInsumo = item.IdInsumo,
                    Insumo = item.Insumo != null ? item.Insumo.Nombre : null,
                    InsumoCodigo = item.Insumo != null ? item.Insumo.Codigo : null,
                    CantidadRequerida = item.CantidadRequerida,
                    PorcentajeDesperdicio = item.PorcentajeDesperdicio,
                    UnidadMedida = item.UnidadMedida != null ? item.UnidadMedida.Nombre : null,
                    UnidadMedidaAbreviatura = item.UnidadMedida != null ? item.UnidadMedida.Abreviatura : null,
                    Estado = item.Estado
                };

                if (item.Insumo != null)
                {
                    var stockDisponible = item.Insumo.StockActual;
                    if (stockDisponible < cantidadTotalNecesaria)
                    {
                        var faltante = cantidadTotalNecesaria - stockDisponible;
                        insuficientes.Add($"{item.Insumo.Nombre}: Faltan {faltante:N2} (Disponible: {stockDisponible:N2}, Necesario: {cantidadTotalNecesaria:N2})");
                    }
                }

                resultado.Add(dto);
            }

            if (insuficientes.Count > 0)
            {
                var mensaje = $"Insuficiencia de insumos para producir {cantidad} unidades:\n" + string.Join("\n", insuficientes);
                return Result<List<BOMDetalleResponseDto>>.Fail(mensaje);
            }

            return Result<List<BOMDetalleResponseDto>>.Ok(resultado);
        }
    }
}
