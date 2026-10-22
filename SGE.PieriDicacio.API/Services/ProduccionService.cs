using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.BOM;
using SGE.PieriDicacio.API.DTOs.Produccion;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class ProduccionService : IProduccionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBOMService _bomService;

        public ProduccionService(ApplicationDbContext context, IBOMService bomService)
        {
            _context = context;
            _bomService = bomService;
        }

        public async Task<Result<List<OrdenProduccionResponseDto>>> ObtenerOrdenesProduccionAsync()
        {
            var ordenes = await _context.OrdenesProduccion
                .Include(o => o.ProductoTerminado)
                .Include(o => o.Pedido)
                .Include(o => o.EstadoOrdenProduccion)
                .Include(o => o.UsuarioResponsable)
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => new OrdenProduccionResponseDto
                {
                    Id = o.Id,
                    NumeroOrden = o.NumeroOrden,
                    IdProductoTerminado = o.IdProductoTerminado,
                    ProductoTerminado = o.ProductoTerminado != null ? o.ProductoTerminado.Nombre : null,
                    ProductoTerminadoCodigo = o.ProductoTerminado != null ? o.ProductoTerminado.Codigo : null,
                    IdPedido = o.IdPedido,
                    NumeroPedido = o.Pedido != null ? o.Pedido.NumeroPedido : null,
                    CantidadProgramada = o.CantidadProgramada,
                    CantidadProducida = o.CantidadProducida,
                    IdEstadoOrdenProduccion = o.IdEstadoOrdenProduccion,
                    EstadoOrdenProduccion = o.EstadoOrdenProduccion != null ? o.EstadoOrdenProduccion.Nombre : null,
                    FechaCreacion = o.FechaCreacion,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    IdUsuarioResponsable = o.IdUsuarioResponsable,
                    UsuarioResponsable = o.UsuarioResponsable != null ? o.UsuarioResponsable.Username : null,
                    Observaciones = o.Observaciones,
                    Estado = o.Estado
                })
                .ToListAsync();

            return Result<List<OrdenProduccionResponseDto>>.Ok(ordenes);
        }

        public async Task<Result<OrdenProduccionResponseDto>> ObtenerOrdenPorIdAsync(int id)
        {
            var orden = await _context.OrdenesProduccion
                .Include(o => o.ProductoTerminado)
                .Include(o => o.Pedido)
                .Include(o => o.EstadoOrdenProduccion)
                .Include(o => o.UsuarioResponsable)
                .Where(o => o.Id == id)
                .Select(o => new OrdenProduccionResponseDto
                {
                    Id = o.Id,
                    NumeroOrden = o.NumeroOrden,
                    IdProductoTerminado = o.IdProductoTerminado,
                    ProductoTerminado = o.ProductoTerminado != null ? o.ProductoTerminado.Nombre : null,
                    ProductoTerminadoCodigo = o.ProductoTerminado != null ? o.ProductoTerminado.Codigo : null,
                    IdPedido = o.IdPedido,
                    NumeroPedido = o.Pedido != null ? o.Pedido.NumeroPedido : null,
                    CantidadProgramada = o.CantidadProgramada,
                    CantidadProducida = o.CantidadProducida,
                    IdEstadoOrdenProduccion = o.IdEstadoOrdenProduccion,
                    EstadoOrdenProduccion = o.EstadoOrdenProduccion != null ? o.EstadoOrdenProduccion.Nombre : null,
                    FechaCreacion = o.FechaCreacion,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    IdUsuarioResponsable = o.IdUsuarioResponsable,
                    UsuarioResponsable = o.UsuarioResponsable != null ? o.UsuarioResponsable.Username : null,
                    Observaciones = o.Observaciones,
                    Estado = o.Estado
                })
                .FirstOrDefaultAsync();

            return orden != null
                ? Result<OrdenProduccionResponseDto>.Ok(orden)
                : Result<OrdenProduccionResponseDto>.Fail("Orden de producción no encontrada");
        }

        public async Task<Result<OrdenProduccionResponseDto>> CrearOrdenProduccionAsync(OrdenProduccionCreateDto dto, int idUsuario)
        {
            var producto = await _context.ProductosTerminados
                .FirstOrDefaultAsync(p => p.Id == dto.IdProductoTerminado && p.Estado);

            if (producto == null)
                return Result<OrdenProduccionResponseDto>.Fail("Producto terminado no encontrado o desactivado");

            if (dto.IdPedido.HasValue)
            {
                var pedido = await _context.Pedidos
                    .FirstOrDefaultAsync(p => p.Id == dto.IdPedido.Value && p.Estado);

                if (pedido == null)
                    return Result<OrdenProduccionResponseDto>.Fail("Pedido no encontrado o desactivado");
            }

            var estadoPendiente = await _context.EstadosOrdenProduccion
                .FirstOrDefaultAsync(e => e.Orden == 1 && e.Estado);

            if (estadoPendiente == null)
                return Result<OrdenProduccionResponseDto>.Fail("No se encontró el estado 'Pendiente' para órdenes de producción");

            var numeroOrden = await GenerarNumeroOrdenAsync();

            var orden = new OrdenProduccion
            {
                NumeroOrden = numeroOrden,
                IdProductoTerminado = dto.IdProductoTerminado,
                IdPedido = dto.IdPedido,
                CantidadProgramada = dto.CantidadProgramada,
                CantidadProducida = 0,
                IdEstadoOrdenProduccion = estadoPendiente.Id,
                FechaCreacion = DateTime.UtcNow,
                IdUsuarioResponsable = idUsuario,
                Observaciones = dto.Observaciones?.Trim(),
                Estado = true
            };

            _context.OrdenesProduccion.Add(orden);
            await _context.SaveChangesAsync();

            return await ObtenerOrdenPorIdAsync(orden.Id);
        }

        public async Task<Result<OrdenProduccionResponseDto>> CambiarEstadoOrdenAsync(int id, int idEstadoNuevo, int idUsuario, string? comentario)
        {
            var orden = await _context.OrdenesProduccion
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return Result<OrdenProduccionResponseDto>.Fail("Orden de producción no encontrada");

            var estadoNuevo = await _context.EstadosOrdenProduccion
                .FirstOrDefaultAsync(e => e.Id == idEstadoNuevo && e.Estado);

            if (estadoNuevo == null)
                return Result<OrdenProduccionResponseDto>.Fail("Estado no válido");

            orden.IdEstadoOrdenProduccion = idEstadoNuevo;

            if (estadoNuevo.Nombre == "En Proceso" && !orden.FechaInicio.HasValue)
            {
                orden.FechaInicio = DateTime.UtcNow;
            }

            if ((estadoNuevo.Nombre == "Completada" || estadoNuevo.Nombre == "Cancelada") && !orden.FechaFin.HasValue)
            {
                orden.FechaFin = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return await ObtenerOrdenPorIdAsync(orden.Id);
        }

        public async Task<Result<List<BOMDetalleResponseDto>>> VerificarDisponibilidadParaProduccionAsync(int idOrdenProduccion)
        {
            var orden = await _context.OrdenesProduccion
                .FirstOrDefaultAsync(o => o.Id == idOrdenProduccion && o.Estado);

            if (orden == null)
                return Result<List<BOMDetalleResponseDto>>.Fail("Orden de producción no encontrada o desactivada");

            var cantidadPendiente = orden.CantidadProgramada - orden.CantidadProducida;
            if (cantidadPendiente <= 0)
                return Result<List<BOMDetalleResponseDto>>.Fail("No hay cantidad pendiente de producir en esta orden");

            return await _bomService.VerificarDisponibilidadInsumosAsync(orden.IdProductoTerminado, cantidadPendiente);
        }

        public async Task<Result<OrdenProduccionResponseDto>> RealizarProduccionAsync(RealizarProduccionDto dto, int idUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orden = await _context.OrdenesProduccion
                    .Include(o => o.ProductoTerminado)
                    .FirstOrDefaultAsync(o => o.Id == dto.IdOrdenProduccion && o.Estado);

                if (orden == null)
                {
                    await transaction.RollbackAsync();
                    return Result<OrdenProduccionResponseDto>.Fail("Orden de producción no encontrada o desactivada");
                }

                var cantidadPendiente = orden.CantidadProgramada - orden.CantidadProducida;
                if (dto.CantidadProducir > cantidadPendiente)
                {
                    await transaction.RollbackAsync();
                    return Result<OrdenProduccionResponseDto>.Fail($"La cantidad a producir ({dto.CantidadProducir}) excede la cantidad pendiente ({cantidadPendiente})");
                }

                var bom = await _context.BOMDetalles
                    .Include(b => b.Insumo)
                    .Where(b => b.IdProductoTerminado == orden.IdProductoTerminado && b.Estado)
                    .ToListAsync();

                if (bom.Count == 0)
                {
                    await transaction.RollbackAsync();
                    return Result<OrdenProduccionResponseDto>.Fail("El producto no tiene lista de materiales (BOM) configurada");
                }

                foreach (var item in bom)
                {
                    var cantidadConDesperdicio = item.CantidadRequerida * (1 + item.PorcentajeDesperdicio / 100);
                    var cantidadTotalNecesaria = cantidadConDesperdicio * dto.CantidadProducir;

                    if (item.Insumo != null && item.Insumo.StockActual < cantidadTotalNecesaria)
                    {
                        await transaction.RollbackAsync();
                        return Result<OrdenProduccionResponseDto>.Fail(
                            $"Insuficiencia de insumo: {item.Insumo.Nombre}. " +
                            $"Disponible: {item.Insumo.StockActual:N2}, Necesario: {cantidadTotalNecesaria:N2}");
                    }
                }

                foreach (var item in bom)
                {
                    if (item.Insumo == null) continue;

                    var cantidadConDesperdicio = item.CantidadRequerida * (1 + item.PorcentajeDesperdicio / 100);
                    var cantidadTotalNecesaria = cantidadConDesperdicio * dto.CantidadProducir;

                    var stockAnterior = item.Insumo.StockActual;
                    item.Insumo.StockActual -= cantidadTotalNecesaria;
                    item.Insumo.FechaModificacion = DateTime.UtcNow;

                    var movimientoInsumo = new MovimientoInsumo
                    {
                        IdInsumo = item.IdInsumo,
                        IdTipoMovimiento = 2,
                        Cantidad = cantidadTotalNecesaria,
                        StockAnterior = stockAnterior,
                        StockResultante = item.Insumo.StockActual,
                        Motivo = "Consumo en producción",
                        Referencia = $"Orden: {orden.NumeroOrden}",
                        IdUsuario = idUsuario,
                        FechaMovimiento = DateTime.UtcNow,
                        Observaciones = dto.Observaciones?.Trim()
                    };

                    _context.MovimientosInsumo.Add(movimientoInsumo);
                }

                if (orden.ProductoTerminado != null)
                {
                    var stockAnteriorProducto = orden.ProductoTerminado.StockActual;
                    orden.ProductoTerminado.StockActual += dto.CantidadProducir;
                    orden.ProductoTerminado.FechaModificacion = DateTime.UtcNow;

                    var movimientoProducto = new MovimientoProductoTerminado
                    {
                        IdProductoTerminado = orden.IdProductoTerminado,
                        IdTipoMovimiento = 1,
                        Cantidad = dto.CantidadProducir,
                        StockAnterior = stockAnteriorProducto,
                        StockResultante = orden.ProductoTerminado.StockActual,
                        Motivo = "Entrada por producción",
                        Referencia = $"Orden: {orden.NumeroOrden}",
                        IdUsuario = idUsuario,
                        FechaMovimiento = DateTime.UtcNow,
                        Observaciones = dto.Observaciones?.Trim()
                    };

                    _context.MovimientosProductoTerminado.Add(movimientoProducto);
                }

                orden.CantidadProducida += dto.CantidadProducir;

                if (orden.CantidadProducida >= orden.CantidadProgramada)
                {
                    var estadoCompletada = await _context.EstadosOrdenProduccion
                        .FirstOrDefaultAsync(e => e.Nombre == "Completada" && e.Estado);

                    if (estadoCompletada != null)
                    {
                        orden.IdEstadoOrdenProduccion = estadoCompletada.Id;
                        orden.FechaFin = DateTime.UtcNow;
                    }
                }
                else if (orden.CantidadProducida > 0)
                {
                    var estadoEnProceso = await _context.EstadosOrdenProduccion
                        .FirstOrDefaultAsync(e => e.Nombre == "En Proceso" && e.Estado);

                    if (estadoEnProceso != null && !orden.FechaInicio.HasValue)
                    {
                        orden.IdEstadoOrdenProduccion = estadoEnProceso.Id;
                        orden.FechaInicio = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await ObtenerOrdenPorIdAsync(orden.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<OrdenProduccionResponseDto>.Fail($"Error al realizar producción: {ex.Message}");
            }
        }

        private async Task<string> GenerarNumeroOrdenAsync()
        {
            var fecha = DateTime.Now;
            var prefijo = $"OP-{fecha:yyyyMMdd}";

            var ultimaOrden = await _context.OrdenesProduccion
                .Where(o => o.NumeroOrden.StartsWith(prefijo))
                .OrderByDescending(o => o.NumeroOrden)
                .FirstOrDefaultAsync();

            var consecutivo = 1;
            if (ultimaOrden != null)
            {
                var partes = ultimaOrden.NumeroOrden.Split('-');
                if (partes.Length >= 3 && int.TryParse(partes[2], out var ultimo))
                {
                    consecutivo = ultimo + 1;
                }
            }

            return $"{prefijo}-{consecutivo:D4}";
        }
    }
}
