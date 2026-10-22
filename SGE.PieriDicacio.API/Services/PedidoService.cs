using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.DTOs.Pedido;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInventarioService _inventarioService;

        public PedidoService(
            ApplicationDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor,
            IInventarioService inventarioService)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
            _inventarioService = inventarioService;
        }

        public async Task<Result<List<PedidoResponseDto>>> ObtenerTodosAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.ProductoTerminado)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.TipoPrenda)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Talla)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Color)
                .OrderByDescending(p => p.FechaPedido)
                .Select(p => new PedidoResponseDto
                {
                    Id = p.Id,
                    NumeroPedido = p.NumeroPedido,
                    IdCliente = p.IdCliente,
                    Cliente = p.Cliente.NombreRazonSocial,
                    IdEstadoPedido = p.IdEstadoPedido,
                    EstadoPedido = p.EstadoPedido.Nombre,
                    FechaPedido = p.FechaPedido,
                    FechaEntrega = p.FechaEntrega,
                    Prioridad = p.Prioridad,
                    Subtotal = p.Subtotal,
                    Descuento = p.Descuento,
                    Total = p.Total,
                    Observaciones = p.Observaciones,
                    Estado = p.Estado,
                    Detalles = p.Detalles.Select(d => new PedidoDetalleResponseDto
                    {
                        Id = d.Id,
                        IdProductoTerminado = d.IdProductoTerminado,
                        ProductoTerminado = d.ProductoTerminado != null ? d.ProductoTerminado.Nombre : null,
                        IdTipoPrenda = d.IdTipoPrenda,
                        TipoPrenda = d.TipoPrenda.Nombre,
                        DescripcionPrenda = d.DescripcionPrenda,
                        IdTalla = d.IdTalla,
                        Talla = d.Talla != null ? d.Talla.Nombre : null,
                        IdColor = d.IdColor,
                        Color = d.Color != null ? d.Color.Nombre : null,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal,
                        Observaciones = d.Observaciones
                    }).ToList()
                })
                .ToListAsync();

            return Result<List<PedidoResponseDto>>.Ok(pedidos);
        }

        public async Task<Result<PedidoResponseDto>> ObtenerPorIdAsync(int id)
        {
            var p = await _context.Pedidos
                .Include(x => x.Cliente)
                .Include(x => x.EstadoPedido)
                .Include(x => x.Detalles).ThenInclude(d => d.ProductoTerminado)
                .Include(x => x.Detalles).ThenInclude(d => d.TipoPrenda)
                .Include(x => x.Detalles).ThenInclude(d => d.Talla)
                .Include(x => x.Detalles).ThenInclude(d => d.Color)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return Result<PedidoResponseDto>.Fail("Pedido no encontrado");

            var response = new PedidoResponseDto
            {
                Id = p.Id,
                NumeroPedido = p.NumeroPedido,
                IdCliente = p.IdCliente,
                Cliente = p.Cliente.NombreRazonSocial,
                IdEstadoPedido = p.IdEstadoPedido,
                EstadoPedido = p.EstadoPedido.Nombre,
                FechaPedido = p.FechaPedido,
                FechaEntrega = p.FechaEntrega,
                Prioridad = p.Prioridad,
                Subtotal = p.Subtotal,
                Descuento = p.Descuento,
                Total = p.Total,
                Observaciones = p.Observaciones,
                Estado = p.Estado,
                Detalles = p.Detalles.Select(d => new PedidoDetalleResponseDto
                {
                    Id = d.Id,
                    IdProductoTerminado = d.IdProductoTerminado,
                    ProductoTerminado = d.ProductoTerminado != null ? d.ProductoTerminado.Nombre : null,
                    IdTipoPrenda = d.IdTipoPrenda,
                    TipoPrenda = d.TipoPrenda.Nombre,
                    DescripcionPrenda = d.DescripcionPrenda,
                    IdTalla = d.IdTalla,
                    Talla = d.Talla != null ? d.Talla.Nombre : null,
                    IdColor = d.IdColor,
                    Color = d.Color != null ? d.Color.Nombre : null,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    Observaciones = d.Observaciones
                }).ToList()
            };

            return Result<PedidoResponseDto>.Ok(response);
        }

        public async Task<Result<PedidoResponseDto>> CrearAsync(PedidoCreateDto dto)
        {
            if (!dto.Detalles.Any())
                return Result<PedidoResponseDto>.Fail("El pedido debe tener al menos un detalle");

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.IdCliente && c.Estado);
            var estadoExiste = await _context.EstadosPedido.AnyAsync(e => e.Id == dto.IdEstadoPedido && e.Estado);

            if (!clienteExiste)
                return Result<PedidoResponseDto>.Fail("El cliente seleccionado no existe o está desactivado");
            if (!estadoExiste)
                return Result<PedidoResponseDto>.Fail("El estado del pedido no es válido");

            if (dto.Descuento < 0)
                return Result<PedidoResponseDto>.Fail("El descuento no puede ser negativo");

            decimal subtotal = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            decimal total = subtotal - dto.Descuento;

            if (total < 0)
                return Result<PedidoResponseDto>.Fail("El total del pedido no puede ser negativo");

            var pedido = new Pedido
            {
                NumeroPedido = await GenerarNumeroPedidoAsync(),
                IdCliente = dto.IdCliente,
                IdEstadoPedido = dto.IdEstadoPedido,
                IdDepartamentoResponsable = dto.IdDepartamentoResponsable,
                IdUsuarioResponsable = dto.IdUsuarioResponsable,
                FechaPedido = dto.FechaPedido,
                FechaEntrega = dto.FechaEntrega,
                Prioridad = dto.Prioridad.Trim().ToUpper(),
                Subtotal = subtotal,
                Descuento = dto.Descuento,
                Total = total,
                Observaciones = dto.Observaciones?.Trim(),
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            foreach (var d in dto.Detalles)
            {
                int idTipoPrenda = d.IdTipoPrenda;
                int? idTalla = d.IdTalla;
                int? idColor = d.IdColor;
                string? descripcionPrenda = d.DescripcionPrenda?.Trim();
                int? idProductoTerminado = d.IdProductoTerminado;

                if (idProductoTerminado.HasValue)
                {
                    var producto = await _context.ProductosTerminados
                        .Include(pt => pt.TipoPrenda)
                        .Include(pt => pt.Talla)
                        .Include(pt => pt.Color)
                        .FirstOrDefaultAsync(pt => pt.Id == idProductoTerminado.Value && pt.Estado);
                    
                    if (producto != null)
                    {
                        idTipoPrenda = producto.IdTipoPrenda ?? idTipoPrenda;
                        idTalla = producto.IdTalla ?? idTalla;
                        idColor = producto.IdColor ?? idColor;
                        if (string.IsNullOrEmpty(descripcionPrenda))
                        {
                            descripcionPrenda = producto.Nombre;
                        }
                    }
                }

                var detalle = new PedidoDetalle
                {
                    IdPedido = pedido.Id,
                    IdProductoTerminado = idProductoTerminado,
                    IdTipoPrenda = idTipoPrenda,
                    DescripcionPrenda = descripcionPrenda,
                    IdTalla = idTalla,
                    IdColor = idColor,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Cantidad * d.PrecioUnitario,
                    Observaciones = d.Observaciones?.Trim(),
                    Estado = true
                };

                _context.PedidosDetalle.Add(detalle);
            }

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(pedido.Id, "Crear", $"Se creó el pedido {pedido.NumeroPedido} por un total de {pedido.Total:C}");

            return await ObtenerPorIdAsync(pedido.Id);
        }

        public async Task<Result<bool>> CambiarEstadoAsync(int idPedido, CambiarEstadoPedidoDto dto, int idUsuario)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == idPedido);
            if (pedido == null)
                return Result<bool>.Fail("Pedido no encontrado");

            var estadoNuevoExiste = await _context.EstadosPedido.AnyAsync(e => e.Id == dto.IdEstadoNuevo && e.Estado);
            if (!estadoNuevoExiste)
                return Result<bool>.Fail("El nuevo estado no es válido");

            var estadoActual = await _context.EstadosPedido.FindAsync(pedido.IdEstadoPedido);
            var estadoNuevo = await _context.EstadosPedido.FindAsync(dto.IdEstadoNuevo);

            if (estadoActual == null || estadoNuevo == null)
                return Result<bool>.Fail("No se pudo cargar la información de los estados");

            if (estadoActual.Id == estadoNuevo.Id)
                return Result<bool>.Fail($"El pedido ya se encuentra en estado \"{estadoNuevo.Nombre}\"");

            var validacion = await ValidarTransicionAsync(pedido, estadoActual, estadoNuevo);
            if (!validacion.Success)
                return validacion;

            var esEntrega = estadoNuevo.Nombre == "Entregado";

            if (esEntrega)
            {
                var validacionStock = await ValidarStockParaEntregaAsync(pedido);
                if (!validacionStock.Success)
                    return validacionStock;
            }

            var historial = new PedidoHistorialEstado
            {
                IdPedido = pedido.Id,
                IdEstadoAnterior = pedido.IdEstadoPedido,
                IdEstadoNuevo = dto.IdEstadoNuevo,
                IdUsuario = idUsuario,
                IdDepartamento = dto.IdDepartamento,
                Comentario = dto.Comentario?.Trim(),
                FechaCambio = DateTime.UtcNow
            };

            pedido.IdEstadoPedido = dto.IdEstadoNuevo;
            pedido.FechaModificacion = DateTime.UtcNow;

            _context.PedidosHistorialEstado.Add(historial);

            using (var transaccion = await _context.Database.BeginTransactionAsync())
            {
                await _context.SaveChangesAsync();

                if (esEntrega)
                {
                    var resultadoStock = await _inventarioService.DescontarStockPorEntregaAsync(idPedido, idUsuario);
                    if (!resultadoStock.Success)
                    {
                        await transaccion.RollbackAsync();
                        return Result<bool>.Fail(resultadoStock.Error ?? "No se pudo descontar el stock");
                    }
                }

                await transaccion.CommitAsync();
            }

            var estadoAnterior = await _context.EstadosPedido.FindAsync(historial.IdEstadoAnterior);
            await RegistrarBitacoraAsync(idPedido, "Cambiar estado", $"El pedido {pedido.NumeroPedido} cambió de \"{estadoAnterior?.Nombre}\" a \"{estadoNuevo.Nombre}\"", idUsuario);

            return Result<bool>.Ok(true);
        }

        private async Task<Result<bool>> ValidarStockParaEntregaAsync(Pedido pedido)
        {
            var detallesConProducto = pedido.Detalles.Where(d => d.IdProductoTerminado.HasValue).ToList();
            if (detallesConProducto.Count == 0)
                return Result<bool>.Ok(true);

            foreach (var detalle in detallesConProducto)
            {
                var producto = await _context.ProductosTerminados.FindAsync(detalle.IdProductoTerminado!.Value);
                if (producto == null)
                    return Result<bool>.Fail("Uno de los productos del pedido ya no existe");

                if (producto.StockActual < detalle.Cantidad)
                    return Result<bool>.Fail($"No se puede entregar el pedido: stock insuficiente de \"{producto.Nombre}\" ({producto.StockActual} disponible, {detalle.Cantidad} requerido).");
            }

            return Result<bool>.Ok(true);
        }

        private async Task<Result<bool>> ValidarTransicionAsync(Pedido pedido, Entities.EstadoPedido estadoActual, Entities.EstadoPedido estadoNuevo)
        {
            const string susPendido = "Suspendido";
            const string cancelado = "Cancelado";
            const string entregado = "Entregado";

            if (estadoActual.Nombre == entregado || estadoActual.Nombre == cancelado)
                return Result<bool>.Fail($"El pedido en estado \"{estadoActual.Nombre}\" es un estado final; no se puede cambiar.");

            if (estadoNuevo.Nombre == cancelado)
                return Result<bool>.Ok(true);

            if (estadoNuevo.Nombre == susPendido)
                return Result<bool>.Ok(true);

            if (estadoActual.Nombre == susPendido)
            {
                var anteriorASuspension = await _context.PedidosHistorialEstado
                    .Where(h => h.IdPedido == pedido.Id)
                    .OrderByDescending(h => h.FechaCambio)
                    .ThenByDescending(h => h.Id)
                    .Select(h => h.IdEstadoAnterior)
                    .FirstOrDefaultAsync();

                if (anteriorASuspension.HasValue && estadoNuevo.Id == anteriorASuspension.Value)
                    return Result<bool>.Ok(true);

                return Result<bool>.Fail("Para reanudar un pedido suspendido debe volver al estado previo a la suspensión.");
            }

            var diferenciaOrden = Math.Abs(estadoNuevo.Orden - estadoActual.Orden);
            if (diferenciaOrden == 1)
                return Result<bool>.Ok(true);

            return Result<bool>.Fail(
                $"El pedido está en estado \"{estadoActual.Nombre}\" y solo puede pasar a un estado consecutivo (anterior o siguiente). No se puede cambiar directamente a \"{estadoNuevo.Nombre}\".");
        }

        public async Task<Result<List<PedidoHistorialResponseDto>>> ObtenerHistorialAsync(int idPedido)
        {
            var pedidoExiste = await _context.Pedidos.AnyAsync(p => p.Id == idPedido);
            if (!pedidoExiste)
                return Result<List<PedidoHistorialResponseDto>>.Fail("Pedido no encontrado");

            var historial = await _context.PedidosHistorialEstado
                .Where(h => h.IdPedido == idPedido)
                .Include(h => h.Pedido)
                .Include(h => h.EstadoAnterior)
                .Include(h => h.EstadoNuevo)
                .Include(h => h.Usuario)
                .Include(h => h.Departamento)
                .OrderByDescending(h => h.FechaCambio)
                .ThenByDescending(h => h.Id)
                .Select(h => new PedidoHistorialResponseDto
                {
                    Id = h.Id,
                    IdPedido = h.IdPedido,
                    NumeroPedido = h.Pedido.NumeroPedido,
                    IdEstadoAnterior = h.IdEstadoAnterior,
                    EstadoAnterior = h.EstadoAnterior != null ? h.EstadoAnterior.Nombre : null,
                    IdEstadoNuevo = h.IdEstadoNuevo,
                    EstadoNuevo = h.EstadoNuevo.Nombre,
                    IdUsuario = h.IdUsuario,
                    Usuario = h.Usuario != null ? $"{h.Usuario.Nombres} {h.Usuario.Apellidos}" : null,
                    IdDepartamento = h.IdDepartamento,
                    Departamento = h.Departamento != null ? h.Departamento.Nombre : null,
                    Comentario = h.Comentario,
                    FechaCambio = h.FechaCambio
                })
                .ToListAsync();

            return Result<List<PedidoHistorialResponseDto>>.Ok(historial);
        }

        public async Task<Result<List<PedidoAlertaResponseDto>>> ObtenerAlertasAsync()
        {
            var hoy = DateTime.UtcNow.Date;
            var estadosTerminales = await _context.EstadosPedido
                .Where(e => e.Nombre == "Entregado" || e.Nombre == "Cancelado")
                .Select(e => e.Id)
                .ToListAsync();

            var pendientes = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.EstadoPedido)
                .Where(p => p.Estado && !estadosTerminales.Contains(p.IdEstadoPedido))
                .ToListAsync();

            var alertas = new List<PedidoAlertaResponseDto>();

            foreach (var p in pendientes)
            {
                var vencido = p.FechaEntrega.Date < hoy;
                var suspendido = p.EstadoPedido.Nombre == "Suspendido";

                if (!vencido && !suspendido)
                    continue;

                int diasAtraso = vencido ? (hoy - p.FechaEntrega.Date).Days : 0;

                alertas.Add(new PedidoAlertaResponseDto
                {
                    Id = p.Id,
                    NumeroPedido = p.NumeroPedido,
                    IdCliente = p.IdCliente,
                    Cliente = p.Cliente.NombreRazonSocial,
                    IdEstadoPedido = p.IdEstadoPedido,
                    EstadoPedido = p.EstadoPedido.Nombre,
                    FechaEntrega = p.FechaEntrega,
                    Prioridad = p.Prioridad,
                    Total = p.Total,
                    DiasAtraso = diasAtraso,
                    TipoAlerta = suspendido ? "Suspendido" : "Vencido"
                });
            }

            return Result<List<PedidoAlertaResponseDto>>.Ok(
                alertas.OrderByDescending(a => a.TipoAlerta == "Vencido")
                       .ThenByDescending(a => a.DiasAtraso)
                       .ToList());
        }

        private async Task RegistrarBitacoraAsync(int idRegistro, string accion, string descripcion, int? idUsuarioExplicito = null)
        {
            try
            {
                var idUsuario = idUsuarioExplicito ?? _httpContextAccessor.HttpContext?.GetUserIdFromHttpContext() ?? 0;
                if (idUsuario == 0)
                    return;

                await _bitacoraService.RegistrarAsync(new BitacoraCreateDto
                {
                    Modulo = "Pedidos",
                    Accion = accion,
                    TablaAfectada = "pedido",
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

        private async Task<string> GenerarNumeroPedidoAsync()
        {
            var fecha = DateTime.UtcNow.ToString("yyyyMMdd");
            var ultimoPedidoHoy = await _context.Pedidos
                .Where(p => p.FechaCreacion.Date == DateTime.UtcNow.Date)
                .OrderByDescending(p => p.Id)
                .Select(p => p.NumeroPedido)
                .FirstOrDefaultAsync();

            int consecutivo = 1;
            if (!string.IsNullOrEmpty(ultimoPedidoHoy))
            {
                var partes = ultimoPedidoHoy.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int num))
                {
                    consecutivo = num + 1;
                }
            }

            return $"PED-{fecha}-{consecutivo.ToString("D4")}";
        }
    }
}
