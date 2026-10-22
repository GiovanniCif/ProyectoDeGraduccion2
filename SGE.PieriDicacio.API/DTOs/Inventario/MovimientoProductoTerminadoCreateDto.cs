using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Inventario
{
    public class MovimientoProductoTerminadoCreateDto
    {
        [Required(ErrorMessage = "El producto es obligatorio")]
        [Range(1, int.MaxValue)]
        public int IdProductoTerminado { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
        [Range(1, int.MaxValue)]
        public int IdTipoMovimiento { get; set; }

        public int? IdPedido { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500, MinimumLength = 3)]
        public string Motivo { get; set; } = string.Empty;

        public string? Referencia { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
