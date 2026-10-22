namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class AlertaStockResponseDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal Faltante { get; set; }
        public string? UnidadMedida { get; set; }
    }
}
