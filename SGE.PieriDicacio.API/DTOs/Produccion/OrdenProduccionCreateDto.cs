using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Produccion
{
    public class OrdenProduccionCreateDto
    {
        [Required(ErrorMessage = "El producto terminado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto válido")]
        public int IdProductoTerminado { get; set; }

        public int? IdPedido { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadProgramada { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
