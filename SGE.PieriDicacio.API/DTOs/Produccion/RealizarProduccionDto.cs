using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Produccion
{
    public class RealizarProduccionDto
    {
        [Required(ErrorMessage = "La orden de producción es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una orden válida")]
        public int IdOrdenProduccion { get; set; }

        [Required(ErrorMessage = "La cantidad a producir es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadProducir { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
