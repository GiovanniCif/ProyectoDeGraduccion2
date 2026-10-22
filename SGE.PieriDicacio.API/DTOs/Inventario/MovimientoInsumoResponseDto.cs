namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class MovimientoInsumoResponseDto
    {
        public int Id { get; set; }
        public int IdInsumo { get; set; }
        public string Insumo { get; set; } = string.Empty;
        public int IdTipoMovimiento { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal StockAnterior { get; set; }
        public decimal StockResultante { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? Observaciones { get; set; }
    }
}
