using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class InsumoCreateDto
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Categoria { get; set; }

        public int? IdUnidadMedida { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StockActual { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StockMinimo { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CostoReferencial { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
