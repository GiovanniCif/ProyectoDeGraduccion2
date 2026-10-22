namespace SGE.PieriDicacio.API.DTOs.Pago
{
    public class PedidoResumenPagoResponseDto
    {
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public decimal TotalPedido { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int IdEstadoFinanciero { get; set; }
        public string EstadoFinanciero { get; set; } = string.Empty;
    }
}
