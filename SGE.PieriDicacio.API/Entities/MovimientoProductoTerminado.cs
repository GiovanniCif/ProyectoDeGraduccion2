namespace SGE.PieriDicacio.API.Entities
{
    public class MovimientoProductoTerminado
    {
        public int Id { get; set; }
        public int IdProductoTerminado { get; set; }
        public int IdTipoMovimiento { get; set; }
        public int? IdPedido { get; set; }
        public decimal Cantidad { get; set; }
        public decimal StockAnterior { get; set; }
        public decimal StockResultante { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaMovimiento { get; set; } = DateTime.Now;
        public string? Observaciones { get; set; }

        public ProductoTerminado ProductoTerminado { get; set; } = null!;
        public TipoMovimientoInventario TipoMovimiento { get; set; } = null!;
        public Pedido? Pedido { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}