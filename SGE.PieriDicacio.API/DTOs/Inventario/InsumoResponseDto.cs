namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class InsumoResponseDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public int? IdUnidadMedida { get; set; }
        public string? UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoReferencial { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
    }
}