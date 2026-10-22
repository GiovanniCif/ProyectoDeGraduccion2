using Microsoft.EntityFrameworkCore;
using SGE.PieriDicacio.API.Data;
using SGE.PieriDicacio.API.DTOs.Bitacora;
using SGE.PieriDicacio.API.DTOs.Pago;
using SGE.PieriDicacio.API.Entities;
using SGE.PieriDicacio.API.Helpers;
using SGE.PieriDicacio.API.Interfaces;

namespace SGE.PieriDicacio.API.Services
{
    public class PagoService : IPagoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagoService(
            ApplicationDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<PagoResponseDto>> RegistrarPagoAsync(PagoCreateDto dto, int idUsuario)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == dto.IdPedido && p.Estado);

            if (pedido == null)
                return Result<PagoResponseDto>.Fail("Pedido no encontrado o desactivado");

            var metodoPagoExiste = await _context.MetodosPago
                .AnyAsync(m => m.Id == dto.IdMetodoPago && m.Estado);

            if (!metodoPagoExiste)
                return Result<PagoResponseDto>.Fail("Método de pago no válido");

            var resumen = await _context.PedidosResumenPago
                .FirstOrDefaultAsync(r => r.IdPedido == dto.IdPedido);

            if (resumen == null)
            {
                resumen = new PedidoResumenPago
                {
                    IdPedido = pedido.Id,
                    TotalPedido = pedido.Total,
                    TotalPagado = 0,
                    SaldoPendiente = pedido.Total,
                    IdEstadoFinanciero = 1,
                    FechaActualizacion = DateTime.UtcNow
                };

                _context.PedidosResumenPago.Add(resumen);
                await _context.SaveChangesAsync();
            }

            if (dto.Monto > resumen.SaldoPendiente)
                return Result<PagoResponseDto>.Fail($"El monto excede el saldo pendiente de {resumen.SaldoPendiente:C}");

            var pago = new Pago
            {
                IdPedido = pedido.Id,
                IdCliente = pedido.IdCliente,
                IdMetodoPago = dto.IdMetodoPago,
                FechaPago = dto.FechaPago,
                Monto = dto.Monto,
                NumeroReferencia = dto.NumeroReferencia?.Trim(),
                Observaciones = dto.Observaciones?.Trim(),
                VoucherUrl = dto.VoucherUrl?.Trim(),
                IdUsuario = idUsuario,
                Estado = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            resumen.TotalPagado += dto.Monto;
            resumen.SaldoPendiente = resumen.TotalPedido - resumen.TotalPagado;
            resumen.FechaActualizacion = DateTime.UtcNow;

            if (resumen.TotalPagado <= 0)
                resumen.IdEstadoFinanciero = 1;
            else if (resumen.SaldoPendiente > 0)
                resumen.IdEstadoFinanciero = 2;
            else
                resumen.IdEstadoFinanciero = 3;

            await _context.SaveChangesAsync();

            await RegistrarBitacoraAsync(pago.Id, "Registrar pago", $"Se registró un pago de {pago.Monto:C} para el pedido {pedido.NumeroPedido}", idUsuario);

            var metodoPago = await _context.MetodosPago.FindAsync(dto.IdMetodoPago);

            return Result<PagoResponseDto>.Ok(new PagoResponseDto
            {
                Id = pago.Id,
                IdPedido = pago.IdPedido,
                NumeroPedido = pedido.NumeroPedido,
                IdCliente = pago.IdCliente,
                Cliente = pedido.Cliente.NombreRazonSocial,
                IdMetodoPago = pago.IdMetodoPago,
                MetodoPago = metodoPago?.Nombre ?? string.Empty,
                FechaPago = pago.FechaPago,
                Monto = pago.Monto,
                NumeroReferencia = pago.NumeroReferencia,
                Observaciones = pago.Observaciones,
                VoucherUrl = pago.VoucherUrl,
                IdUsuario = pago.IdUsuario,
                Estado = pago.Estado
            });
        }

        private async Task RegistrarBitacoraAsync(int idRegistro, string accion, string descripcion, int idUsuarioExplicito)
        {
            try
            {
                var idUsuario = idUsuarioExplicito != 0 ? idUsuarioExplicito : (_httpContextAccessor.HttpContext?.GetUserIdFromHttpContext() ?? 0);
                if (idUsuario == 0)
                    return;

                await _bitacoraService.RegistrarAsync(new BitacoraCreateDto
                {
                    Modulo = "Pagos",
                    Accion = accion,
                    TablaAfectada = "pago",
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

        public async Task<Result<List<PagoResponseDto>>> ObtenerPagosPorPedidoAsync(int idPedido)
        {
            var pagos = await _context.Pagos
                .Include(p => p.Pedido)
                .Include(p => p.Cliente)
                .Include(p => p.MetodoPago)
                .Where(p => p.IdPedido == idPedido && p.Estado)
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new PagoResponseDto
                {
                    Id = p.Id,
                    IdPedido = p.IdPedido,
                    NumeroPedido = p.Pedido.NumeroPedido,
                    IdCliente = p.IdCliente,
                    Cliente = p.Cliente.NombreRazonSocial,
                    IdMetodoPago = p.IdMetodoPago,
                    MetodoPago = p.MetodoPago.Nombre,
                    FechaPago = p.FechaPago,
                    Monto = p.Monto,
                    NumeroReferencia = p.NumeroReferencia,
                    Observaciones = p.Observaciones,
                    VoucherUrl = p.VoucherUrl,
                    IdUsuario = p.IdUsuario,
                    Estado = p.Estado
                })
                .ToListAsync();

            return Result<List<PagoResponseDto>>.Ok(pagos);
        }

        public async Task<Result<PedidoResumenPagoResponseDto>> ObtenerResumenPagoAsync(int idPedido)
        {
            var resumen = await _context.PedidosResumenPago
                .Include(r => r.Pedido)
                .Include(r => r.EstadoFinanciero)
                .FirstOrDefaultAsync(r => r.IdPedido == idPedido);

            if (resumen == null)
                return Result<PedidoResumenPagoResponseDto>.Fail("No existe resumen de pagos para este pedido");

            return Result<PedidoResumenPagoResponseDto>.Ok(new PedidoResumenPagoResponseDto
            {
                IdPedido = resumen.IdPedido,
                NumeroPedido = resumen.Pedido.NumeroPedido,
                TotalPedido = resumen.TotalPedido,
                TotalPagado = resumen.TotalPagado,
                SaldoPendiente = resumen.SaldoPendiente,
                IdEstadoFinanciero = resumen.IdEstadoFinanciero,
                EstadoFinanciero = resumen.EstadoFinanciero.Nombre
            });
        }
    }
}
