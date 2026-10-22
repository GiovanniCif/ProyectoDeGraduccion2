namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class ProductoTerminadoResponseDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int? IdTipoPrenda { get; set; }
        public string? TipoPrenda { get; set; }
        public string? Modelo { get; set; }
        public int? IdTalla { get; set; }
        public string? Talla { get; set; }
        public int? IdColor { get; set; }
        public string? Color { get; set; }
        public int? IdUnidadMedida { get; set; }
        public string? UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoReferencial { get; set; }
        public decimal PrecioReferencial { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
    }
}