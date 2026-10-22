namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class KardexMovimientoDto
    {
        public int IdMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public string TipoEfecto { get; set; } = string.Empty;
        public decimal Entradas { get; set; }
        public decimal Salidas { get; set; }
        public decimal Saldo { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? Referencia { get; set; }
    }

    public class KardexProductoTerminadoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public List<KardexMovimientoDto> Movimientos { get; set; } = new();
    }

    public class KardexInsumoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string? UnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public List<KardexMovimientoDto> Movimientos { get; set; } = new();
    }
}
