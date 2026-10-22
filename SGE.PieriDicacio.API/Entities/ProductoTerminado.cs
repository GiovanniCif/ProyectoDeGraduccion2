namespace SGE.PieriDicacio.API.Entities
{
    public class ProductoTerminado
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int? IdTipoPrenda { get; set; }
        public string? Modelo { get; set; }
        public int? IdTalla { get; set; }
        public int? IdColor { get; set; }
        public int? IdUnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoReferencial { get; set; }
        public decimal PrecioReferencial { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public TipoPrenda? TipoPrenda { get; set; }
        public Talla? Talla { get; set; }
        public Color? Color { get; set; }
        public UnidadMedida? UnidadMedida { get; set; }
    }
}
