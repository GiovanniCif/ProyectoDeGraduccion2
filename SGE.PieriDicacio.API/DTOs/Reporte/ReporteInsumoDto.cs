namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class ReporteInsumoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoReferencial { get; set; }
        public bool Estado { get; set; }
    }
}