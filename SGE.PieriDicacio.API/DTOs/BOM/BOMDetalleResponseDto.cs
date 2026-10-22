namespace SGE.PieriDicacio.API.DTOs.BOM
{
    public class BOMDetalleResponseDto
    {
        public int Id { get; set; }
        public int IdProductoTerminado { get; set; }
        public string? ProductoTerminado { get; set; }
        public string? ProductoTerminadoCodigo { get; set; }
        public int IdInsumo { get; set; }
        public string? Insumo { get; set; }
        public string? InsumoCodigo { get; set; }
        public string? InsumoCategoria { get; set; }
        public decimal CantidadRequerida { get; set; }
        public int? IdUnidadMedida { get; set; }
        public string? UnidadMedida { get; set; }
        public string? UnidadMedidaAbreviatura { get; set; }
        public decimal PorcentajeDesperdicio { get; set; }
        public decimal CantidadConDesperdicio => CantidadRequerida * (1 + PorcentajeDesperdicio / 100);
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
