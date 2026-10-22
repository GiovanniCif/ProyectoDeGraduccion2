using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.DTOs.Inventario;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InventarioService(
            ApplicationDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<ProductoTerminadoResponseDto>>> ObtenerProductosTerminadosAsync()
        {
            var productos = await _context.ProductosTerminados
                .Include(p => p.TipoPrenda)
                .Include(p => p.Talla)
                .Include(p => p.Color)
                .Include(p => p.UnidadMedida)
                .OrderBy(p => p.Nombre)
                .Select(p => new ProductoTerminadoResponseDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    IdTipoPrenda = p.IdTipoPrenda,
                    TipoPrenda = p.TipoPrenda != null ? p.TipoPrenda.Nombre : null,
                    Modelo = p.Modelo,
                    IdTalla = p.IdTalla,
                    Talla = p.Talla != null ? p.Talla.Nombre : null,
                    IdColor = p.IdColor,
                    Color = p.Color != null ? p.Color.Nombre : null,
                    IdUnidadMedida = p.IdUnidadMedida,
                    UnidadMedida = p.UnidadMedida != null ? p.UnidadMedida.Nombre : null,
                    StockActual = p.StockActual,
                    StockMinimo = p.StockMinimo,
                    CostoReferencial = p.CostoReferencial,
                    PrecioReferencial = p.PrecioReferencial,
                    Observaciones = p.Observaciones,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<ProductoTerminadoResponseDto>>.Ok(productos);
        }

        public async Task<Result<ProductoTerminadoResponseDto>> CrearProductoTerminadoAsync(ProductoTerminadoCreateDto dto)
        {
            var codigoExiste = await _context.ProductosTerminados.AnyAsync(p => p.Codigo == dto.Codigo);
            if (codigoExiste)
                return Result<ProductoTerminadoResponseDto>.Fail("Ya existe un producto con ese código");

            var producto = new ProductoTerminado
            {
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                IdTipoPrenda = dto.IdTipoPrenda,
                Modelo = dto.Modelo?.Trim(),
                IdTalla = dto.IdTalla,
                IdColor = dto.IdColor,
                IdUnidadMedida = dto.IdUnidadMedida,
                StockActual = dto.StockActual,
                StockMinimo = dto.StockMinimo,
                CostoReferencial = dto.CostoReferencial,
                PrecioReferencial = dto.PrecioReferencial,
                Observaciones = dto.Observaciones?.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.ProductosTerminados.Add(producto);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync("Productos", "Crear", "producto_terminado", producto.Id, $"Se creó el producto terminado {producto.Codigo} - {producto.Nombre}");

            return await ObtenerProductoPorId(producto.Id);
        }

        public async Task<Result<ProductoTerminadoResponseDto>> ObtenerProductoPorId(int id)
        {
            var producto = await _context.ProductosTerminados
                .Include(p => p.TipoPrenda)
                .Include(p => p.Talla)
                .Include(p => p.Color)
                .Include(p => p.UnidadMedida)
                .Where(p => p.Id == id)
                .Select(p => new ProductoTerminadoResponseDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    IdTipoPrenda = p.IdTipoPrenda,
                    TipoPrenda = p.TipoPrenda != null ? p.TipoPrenda.Nombre : null,
                    Modelo = p.Modelo,
                    IdTalla = p.IdTalla,
                    Talla = p.Talla != null ? p.Talla.Nombre : null,
                    IdColor = p.IdColor,
                    Color = p.Color != null ? p.Color.Nombre : null,
                    IdUnidadMedida = p.IdUnidadMedida,
                    UnidadMedida = p.UnidadMedida != null ? p.UnidadMedida.Nombre : null,
                    StockActual = p.StockActual,
                    StockMinimo = p.StockMinimo,
                    CostoReferencial = p.CostoReferencial,
                    PrecioReferencial = p.PrecioReferencial,
                    Observaciones = p.Observaciones,
                    Estado = p.Estado
                })
                .FirstOrDefaultAsync();

            return producto != null
                ? Result<ProductoTerminadoResponseDto>.Ok(producto)
                : Result<ProductoTerminadoResponseDto>.Fail("Producto no encontrado");
        }

        public async Task<Result<MovimientoProductoTerminadoResponseDto>> RegistrarMovimientoProductoTerminadoAsync(MovimientoProductoTerminadoCreateDto dto, int idUsuario)
        {
            var producto = await _context.ProductosTerminados.FirstOrDefaultAsync(p => p.Id == dto.IdProductoTerminado && p.Estado);
            if (producto == null)
                return Result<MovimientoProductoTerminadoResponseDto>.Fail("Producto no encontrado o desactivado");

            var tipoMovimiento = await _context.TiposMovimientoInventario.FirstOrDefaultAsync(t => t.Id == dto.IdTipoMovimiento && t.Estado);
            if (tipoMovimiento == null)
                return Result<MovimientoProductoTerminadoResponseDto>.Fail("Tipo de movimiento no válido");

            var stockAnterior = producto.StockActual;
            var stockResultante = stockAnterior;

            if (tipoMovimiento.TipoEfecto == "SUMA")
                stockResultante += dto.Cantidad;
            else if (tipoMovimiento.TipoEfecto == "RESTA")
                stockResultante -= dto.Cantidad;

            if (stockResultante < 0)
                return Result<MovimientoProductoTerminadoResponseDto>.Fail("Stock insuficiente para esta operación");

            var movimiento = new MovimientoProductoTerminado
            {
                IdProductoTerminado = dto.IdProductoTerminado,
                IdTipoMovimiento = dto.IdTipoMovimiento,
                IdPedido = dto.IdPedido,
                Cantidad = dto.Cantidad,
                StockAnterior = stockAnterior,
                StockResultante = stockResultante,
                Motivo = dto.Motivo?.Trim() ?? string.Empty,
                Referencia = dto.Referencia?.Trim(),
                IdUsuario = idUsuario,
                FechaMovimiento = DateTime.UtcNow,
                Observaciones = dto.Observaciones?.Trim()
            };

            producto.StockActual = stockResultante;
            producto.FechaModificacion = DateTime.UtcNow;

            _context.MovimientosProductoTerminado.Add(movimiento);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync("Productos", "Movimiento", "movimiento_producto_terminado", movimiento.Id, $"Movimiento de stock ({tipoMovimiento.Nombre}) de {producto.Nombre}: {movimiento.StockAnterior} → {movimiento.StockResultante}", idUsuario);

            return await ObtenerMovimientoProductoTerminado(movimiento.Id);
        }

        private async Task<Result<MovimientoProductoTerminadoResponseDto>> ObtenerMovimientoProductoTerminado(int id)
        {
            var movimiento = await _context.MovimientosProductoTerminado
                .Include(m => m.ProductoTerminado)
                .Include(m => m.TipoMovimiento)
                .Where(m => m.Id == id)
                .Select(m => new MovimientoProductoTerminadoResponseDto
                {
                    Id = m.Id,
                    IdProductoTerminado = m.IdProductoTerminado,
                    ProductoTerminado = m.ProductoTerminado.Nombre,
                    IdTipoMovimiento = m.IdTipoMovimiento,
                    TipoMovimiento = m.TipoMovimiento.Nombre,
                    Cantidad = m.Cantidad,
                    StockAnterior = m.StockAnterior,
                    StockResultante = m.StockResultante,
                    Motivo = m.Motivo,
                    Referencia = m.Referencia,
                    IdUsuario = m.IdUsuario,
                    FechaMovimiento = m.FechaMovimiento,
                    Observaciones = m.Observaciones
                })
                .FirstOrDefaultAsync();

            return movimiento != null
                ? Result<MovimientoProductoTerminadoResponseDto>.Ok(movimiento)
                : Result<MovimientoProductoTerminadoResponseDto>.Fail("Movimiento no encontrado");
        }

        public async Task<Result<List<InsumoResponseDto>>> ObtenerInsumosAsync()
        {
            var insumos = await _context.Insumos
                .Include(i => i.UnidadMedida)
                .OrderBy(i => i.Nombre)
                .Select(i => new InsumoResponseDto
                {
                    Id = i.Id,
                    Codigo = i.Codigo,
                    Nombre = i.Nombre,
                    Categoria = i.Categoria,
                    IdUnidadMedida = i.IdUnidadMedida,
                    UnidadMedida = i.UnidadMedida != null ? i.UnidadMedida.Nombre : null,
                    StockActual = i.StockActual,
                    StockMinimo = i.StockMinimo,
                    CostoReferencial = i.CostoReferencial,
                    Observaciones = i.Observaciones,
                    Estado = i.Estado
                })
                .ToListAsync();

            return Result<List<InsumoResponseDto>>.Ok(insumos);
        }

        public async Task<Result<InsumoResponseDto>> CrearInsumoAsync(InsumoCreateDto dto)
        {
            var codigoExiste = await _context.Insumos.AnyAsync(i => i.Codigo == dto.Codigo);
            if (codigoExiste)
                return Result<InsumoResponseDto>.Fail("Ya existe un insumo con ese código");

            var insumo = new Insumo
            {
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Categoria = dto.Categoria?.Trim(),
                IdUnidadMedida = dto.IdUnidadMedida,
                StockActual = dto.StockActual,
                StockMinimo = dto.StockMinimo,
                CostoReferencial = dto.CostoReferencial,
                Observaciones = dto.Observaciones?.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Insumos.Add(insumo);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync("Insumos", "Crear", "insumo", insumo.Id, $"Se creó el insumo {insumo.Codigo} - {insumo.Nombre}");

            return await ObtenerInsumoPorId(insumo.Id);
        }

        public async Task<Result<InsumoResponseDto>> ObtenerInsumoPorId(int id)
        {
            var insumo = await _context.Insumos
                .Include(i => i.UnidadMedida)
                .Where(i => i.Id == id)
                .Select(i => new InsumoResponseDto
                {
                    Id = i.Id,
                    Codigo = i.Codigo,
                    Nombre = i.Nombre,
                    Categoria = i.Categoria,
                    IdUnidadMedida = i.IdUnidadMedida,
                    UnidadMedida = i.UnidadMedida != null ? i.UnidadMedida.Nombre : null,
                    StockActual = i.StockActual,
                    StockMinimo = i.StockMinimo,
                    CostoReferencial = i.CostoReferencial,
                    Observaciones = i.Observaciones,
                    Estado = i.Estado
                })
                .FirstOrDefaultAsync();

            return insumo != null
                ? Result<InsumoResponseDto>.Ok(insumo)
                : Result<InsumoResponseDto>.Fail("Insumo no encontrado");
        }

        public async Task<Result<MovimientoInsumoResponseDto>> RegistrarMovimientoInsumoAsync(MovimientoInsumoCreateDto dto, int idUsuario)
        {
            var insumo = await _context.Insumos.FirstOrDefaultAsync(i => i.Id == dto.IdInsumo && i.Estado);
            if (insumo == null)
                return Result<MovimientoInsumoResponseDto>.Fail("Insumo no encontrado o desactivado");

            var tipoMovimiento = await _context.TiposMovimientoInventario.FirstOrDefaultAsync(t => t.Id == dto.IdTipoMovimiento && t.Estado);
            if (tipoMovimiento == null)
                return Result<MovimientoInsumoResponseDto>.Fail("Tipo de movimiento no válido");

            var stockAnterior = insumo.StockActual;
            var stockResultante = stockAnterior;

            if (tipoMovimiento.TipoEfecto == "SUMA")
                stockResultante += dto.Cantidad;
            else if (tipoMovimiento.TipoEfecto == "RESTA")
                stockResultante -= dto.Cantidad;

            if (stockResultante < 0)
                return Result<MovimientoInsumoResponseDto>.Fail("Stock insuficiente para esta operación");

            var movimiento = new MovimientoInsumo
            {
                IdInsumo = dto.IdInsumo,
                IdTipoMovimiento = dto.IdTipoMovimiento,
                Cantidad = dto.Cantidad,
                StockAnterior = stockAnterior,
                StockResultante = stockResultante,
                Motivo = dto.Motivo?.Trim() ?? string.Empty,
                Referencia = dto.Referencia?.Trim(),
                IdUsuario = idUsuario,
                FechaMovimiento = DateTime.UtcNow,
                Observaciones = dto.Observaciones?.Trim()
            };

            insumo.StockActual = stockResultante;
            insumo.FechaModificacion = DateTime.UtcNow;

            _context.MovimientosInsumo.Add(movimiento);
            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync("Insumos", "Movimiento", "movimiento_insumo", movimiento.Id, $"Movimiento de stock ({tipoMovimiento.Nombre}) de {insumo.Nombre}: {movimiento.StockAnterior} → {movimiento.StockResultante}", idUsuario);

            var mov = await _context.MovimientosInsumo
                .Include(m => m.Insumo)
                .Include(m => m.TipoMovimiento)
                .Where(m => m.Id == movimiento.Id)
                .Select(m => new MovimientoInsumoResponseDto
                {
                    Id = m.Id,
                    IdInsumo = m.IdInsumo,
                    Insumo = m.Insumo.Nombre,
                    IdTipoMovimiento = m.IdTipoMovimiento,
                    TipoMovimiento = m.TipoMovimiento.Nombre,
                    Cantidad = m.Cantidad,
                    StockAnterior = m.StockAnterior,
                    StockResultante = m.StockResultante,
                    Motivo = m.Motivo,
                    Referencia = m.Referencia,
                    IdUsuario = m.IdUsuario,
                    FechaMovimiento = m.FechaMovimiento,
                    Observaciones = m.Observaciones
                })
                .FirstOrDefaultAsync();

            return mov != null
                ? Result<MovimientoInsumoResponseDto>.Ok(mov)
                : Result<MovimientoInsumoResponseDto>.Fail("No se pudo registrar el movimiento");
        }

        public async Task<Result<KardexProductoTerminadoDto>> ObtenerKardexProductoTerminadoAsync(int id, DateTime? desde, DateTime? hasta)
        {
            var producto = await _context.ProductosTerminados
                .Include(p => p.UnidadMedida)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return Result<KardexProductoTerminadoDto>.Fail("Producto no encontrado");

            var fechaDesde = (desde ?? DateTime.MinValue).Date;
            var fechaHasta = (hasta ?? DateTime.MaxValue).Date.AddDays(1);

            var movimientos = await _context.MovimientosProductoTerminado
                .Include(m => m.TipoMovimiento)
                .Where(m => m.IdProductoTerminado == id)
                .Where(m => m.FechaMovimiento >= fechaDesde && m.FechaMovimiento < fechaHasta)
                .OrderBy(m => m.FechaMovimiento)
                .ThenBy(m => m.Id)
                .Select(m => new
                {
                    m.Id,
                    m.FechaMovimiento,
                    m.StockAnterior,
                    m.StockResultante,
                    m.Cantidad,
                    m.Motivo,
                    m.Referencia,
                    Tipo = m.TipoMovimiento.Nombre,
                    Efecto = m.TipoMovimiento.TipoEfecto
                })
                .ToListAsync();

            var kardex = new KardexProductoTerminadoDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                UnidadMedida = producto.UnidadMedida?.Nombre,
                StockActual = producto.StockActual,
                StockMinimo = producto.StockMinimo
            };

            kardex.Movimientos = movimientos.Select(m => new KardexMovimientoDto
            {
                IdMovimiento = m.Id,
                Fecha = m.FechaMovimiento,
                TipoMovimiento = m.Tipo,
                TipoEfecto = m.Efecto,
                Entradas = m.Efecto == "SUMA" ? m.Cantidad : 0,
                Salidas = m.Efecto == "RESTA" ? m.Cantidad : 0,
                Saldo = m.StockResultante,
                Motivo = m.Motivo,
                Referencia = m.Referencia
            }).ToList();

            return Result<KardexProductoTerminadoDto>.Ok(kardex);
        }

        public async Task<Result<KardexInsumoDto>> ObtenerKardexInsumoAsync(int id, DateTime? desde, DateTime? hasta)
        {
            var insumo = await _context.Insumos
                .Include(i => i.UnidadMedida)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (insumo == null)
                return Result<KardexInsumoDto>.Fail("Insumo no encontrado");

            var fechaDesde = (desde ?? DateTime.MinValue).Date;
            var fechaHasta = (hasta ?? DateTime.MaxValue).Date.AddDays(1);

            var movimientos = await _context.MovimientosInsumo
                .Include(m => m.TipoMovimiento)
                .Where(m => m.IdInsumo == id)
                .Where(m => m.FechaMovimiento >= fechaDesde && m.FechaMovimiento < fechaHasta)
                .OrderBy(m => m.FechaMovimiento)
                .ThenBy(m => m.Id)
                .Select(m => new
                {
                    m.Id,
                    m.FechaMovimiento,
                    m.StockResultante,
                    m.Cantidad,
                    m.Motivo,
                    m.Referencia,
                    Tipo = m.TipoMovimiento.Nombre,
                    Efecto = m.TipoMovimiento.TipoEfecto
                })
                .ToListAsync();

            var kardex = new KardexInsumoDto
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                Categoria = insumo.Categoria,
                UnidadMedida = insumo.UnidadMedida?.Nombre,
                StockActual = insumo.StockActual,
                StockMinimo = insumo.StockMinimo
            };

            kardex.Movimientos = movimientos.Select(m => new KardexMovimientoDto
            {
                IdMovimiento = m.Id,
                Fecha = m.FechaMovimiento,
                TipoMovimiento = m.Tipo,
                TipoEfecto = m.Efecto,
                Entradas = m.Efecto == "SUMA" ? m.Cantidad : 0,
                Salidas = m.Efecto == "RESTA" ? m.Cantidad : 0,
                Saldo = m.StockResultante,
                Motivo = m.Motivo,
                Referencia = m.Referencia
            }).ToList();

            return Result<KardexInsumoDto>.Ok(kardex);
        }

        public async Task<Result<List<AlertaStockResponseDto>>> ObtenerAlertasStockAsync()
        {
            var alertas = new List<AlertaStockResponseDto>();

            var productos = await _context.ProductosTerminados
                .Include(p => p.UnidadMedida)
                .Where(p => p.Estado && p.StockActual < p.StockMinimo)
                .ToListAsync();

            foreach (var p in productos)
            {
                alertas.Add(new AlertaStockResponseDto
                {
                    Id = p.Id,
                    Tipo = "Producto",
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    StockActual = p.StockActual,
                    StockMinimo = p.StockMinimo,
                    Faltante = p.StockMinimo - p.StockActual,
                    UnidadMedida = p.UnidadMedida?.Nombre
                });
            }

            var insumos = await _context.Insumos
                .Include(i => i.UnidadMedida)
                .Where(i => i.Estado && i.StockActual < i.StockMinimo)
                .ToListAsync();

            foreach (var i in insumos)
            {
                alertas.Add(new AlertaStockResponseDto
                {
                    Id = i.Id,
                    Tipo = "Insumo",
                    Codigo = i.Codigo,
                    Nombre = i.Nombre,
                    StockActual = i.StockActual,
                    StockMinimo = i.StockMinimo,
                    Faltante = i.StockMinimo - i.StockActual,
                    UnidadMedida = i.UnidadMedida?.Nombre
                });
            }

            return Result<List<AlertaStockResponseDto>>.Ok(
                alertas.OrderBy(a => a.Tipo).ThenBy(a => a.Nombre).ToList());
        }

        public async Task<Result<bool>> DescontarStockPorEntregaAsync(int idPedido, int idUsuario)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == idPedido);

            if (pedido == null)
                return Result<bool>.Fail("Pedido no encontrado");

            var tipoSalida = await _context.TiposMovimientoInventario
                .FirstOrDefaultAsync(t => t.Nombre == "Salida" && t.TipoEfecto == "RESTA" && t.Estado)
                ?? await _context.TiposMovimientoInventario
                    .FirstOrDefaultAsync(t => t.TipoEfecto == "RESTA" && t.Estado);

            if (tipoSalida == null)
                return Result<bool>.Fail("No existe un tipo de movimiento de salida configurado");

            var detallesConProducto = pedido.Detalles
                .Where(d => d.IdProductoTerminado.HasValue)
                .ToList();

            if (detallesConProducto.Count == 0)
                return Result<bool>.Ok(true);

            foreach (var detalle in detallesConProducto)
            {
                var producto = await _context.ProductosTerminados.FindAsync(detalle.IdProductoTerminado!.Value);
                if (producto == null)
                    return Result<bool>.Fail("Uno de los productos del pedido ya no existe");

                if (producto.StockActual < detalle.Cantidad)
                    return Result<bool>.Fail($"Stock insuficiente de \"{producto.Nombre}\" ({producto.StockActual} disponible, {detalle.Cantidad} requerido)");

                var stockAnterior = producto.StockActual;
                var stockResultante = stockAnterior - detalle.Cantidad;

                producto.StockActual = stockResultante;
                producto.FechaModificacion = DateTime.UtcNow;

                _context.MovimientosProductoTerminado.Add(new MovimientoProductoTerminado
                {
                    IdProductoTerminado = producto.Id,
                    IdTipoMovimiento = tipoSalida.Id,
                    IdPedido = pedido.Id,
                    Cantidad = detalle.Cantidad,
                    StockAnterior = stockAnterior,
                    StockResultante = stockResultante,
                    Motivo = $"Entrega de pedido {pedido.NumeroPedido}",
                    Referencia = pedido.NumeroPedido,
                    IdUsuario = idUsuario,
                    FechaMovimiento = DateTime.UtcNow,
                    Observaciones = $"Salida automática por entrega del pedido {pedido.NumeroPedido}"
                });
            }

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync("Productos", "Entrega pedido", "producto_terminado", pedido.Id, $"Stock descontado automáticamente por entrega del pedido {pedido.NumeroPedido}", idUsuario);

            return Result<bool>.Ok(true);
        }

        private async Task RegistrarBitacoraAsync(string modulo, string accion, string tabla, long idRegistro, string descripcion, int? idUsuarioExplicito = null)
        {
            try
            {
                var idUsuario = idUsuarioExplicito ?? _httpContextAccessor.HttpContext?.GetUserIdFromHttpContext() ?? 0;
                if (idUsuario == 0)
                    return;

                await _bitacoraService.RegistrarAsync(new BitacoraCreateDto
                {
                    Modulo = modulo,
                    Accion = accion,
                    TablaAfectada = tabla,
                    IdRegistroAfectado = idRegistro,
                    Descripcion = descripcion,
                    IpEquipo = AuditHelper.ObtenerIp(_httpContextAccessor.HttpContext)
                }, idUsuario);
            }
            catch
            {
                // La bitácora nunca debe impedir la operación principal
            }
        }
    }
}
