namespace SGE.PieriDicacio.API.Entities
{
    public class PedidoDetalle
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int? IdProductoTerminado { get; set; }
        public int IdTipoPrenda { get; set; }
        public string? DescripcionPrenda { get; set; }
        public int? IdTalla { get; set; }
        public int? IdColor { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;

        public Pedido Pedido { get; set; } = null!;
        public ProductoTerminado? ProductoTerminado { get; set; }
        public TipoPrenda TipoPrenda { get; set; } = null!;
        public Talla? Talla { get; set; }
        public Color? Color { get; set; }
    }
}
