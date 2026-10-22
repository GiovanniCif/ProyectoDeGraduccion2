namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class CuentaPorCobrarDto
    {
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public decimal TotalPedido { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string EstadoFinanciero { get; set; } = string.Empty;
    }
}
