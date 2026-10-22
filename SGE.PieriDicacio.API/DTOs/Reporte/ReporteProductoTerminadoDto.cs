namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class ReporteProductoTerminadoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string TipoPrenda { get; set; } = string.Empty;
        public string? Talla { get; set; }
        public string? Color { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal PrecioReferencial { get; set; }
        public bool Estado { get; set; }
    }
}
