using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.BOM
{
    public class BOMDetalleUpdateDto
    {
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal? CantidadRequerida { get; set; }

        public int? IdUnidadMedida { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de desperdicio debe estar entre 0 y 100")]
        public decimal? PorcentajeDesperdicio { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public bool? Estado { get; set; }
    }
}
