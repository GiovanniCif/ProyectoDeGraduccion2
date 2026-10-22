namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class PedidoDetalleResponseDto
    {
        public int Id { get; set; }
        public int? IdProductoTerminado { get; set; }
        public string? ProductoTerminado { get; set; }
        public int IdTipoPrenda { get; set; }
        public string TipoPrenda { get; set; } = string.Empty;
        public string? DescripcionPrenda { get; set; }
        public int? IdTalla { get; set; }
        public string? Talla { get; set; }
        public int? IdColor { get; set; }
        public string? Color { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? Observaciones { get; set; }
    }
}