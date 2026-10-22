namespace SGE.PieriDicacio.API.Entities
{
    public class BOMDetalle
    {
        public int Id { get; set; }
        public int IdProductoTerminado { get; set; }
        public int IdInsumo { get; set; }
        public decimal CantidadRequerida { get; set; }
        public int? IdUnidadMedida { get; set; }
        public decimal PorcentajeDesperdicio { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public ProductoTerminado? ProductoTerminado { get; set; }
        public Insumo? Insumo { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }
    }
}
