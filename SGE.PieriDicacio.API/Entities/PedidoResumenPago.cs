namespace SGE.PieriDicacio.API.Entities
{
    public class PedidoResumenPago
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public decimal TotalPedido { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int IdEstadoFinanciero { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        public Pedido Pedido { get; set; } = null!;
        public PedidoEstadoFinanciero EstadoFinanciero { get; set; } = null!;
    }
}