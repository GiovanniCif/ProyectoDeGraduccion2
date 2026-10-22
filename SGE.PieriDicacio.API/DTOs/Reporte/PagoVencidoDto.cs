namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class PagoVencidoDto
    {
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string EstadoPedido { get; set; } = string.Empty;
        public decimal TotalPedido { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int DiasAtraso { get; set; }
    }
}
