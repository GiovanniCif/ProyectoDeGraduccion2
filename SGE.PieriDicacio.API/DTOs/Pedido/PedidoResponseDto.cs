namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class PedidoResponseDto
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int IdEstadoPedido { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
        public List<PedidoDetalleResponseDto> Detalles { get; set; } = new();
    }
}