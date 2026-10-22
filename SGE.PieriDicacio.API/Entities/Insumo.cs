namespace SGE.PieriDicacio.API.Entities
{
    public class Insumo
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public int? IdUnidadMedida { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoReferencial { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public UnidadMedida? UnidadMedida { get; set; }
    }
}
