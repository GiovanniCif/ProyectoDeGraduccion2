using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.BOM
{
    public class BOMDetalleCreateDto
    {
        [Required(ErrorMessage = "El producto terminado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto válido")]
        public int IdProductoTerminado { get; set; }

        [Required(ErrorMessage = "El insumo es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un insumo válido")]
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadRequerida { get; set; }

        public int? IdUnidadMedida { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de desperdicio debe estar entre 0 y 100")]
        public decimal PorcentajeDesperdicio { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
