namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class ReportePagoDto
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? NumeroReferencia { get; set; }
        public bool Estado { get; set; }
    }
}
