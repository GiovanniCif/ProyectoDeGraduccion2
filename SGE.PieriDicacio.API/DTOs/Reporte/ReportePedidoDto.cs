namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class ReportePedidoDto
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string EstadoPedido { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public bool Estado { get; set; }
    }
}