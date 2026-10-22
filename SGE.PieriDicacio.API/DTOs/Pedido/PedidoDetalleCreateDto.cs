using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class PedidoDetalleCreateDto
    {
        public int? IdProductoTerminado { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de prenda válido")]
        public int IdTipoPrenda { get; set; }

        [StringLength(500)]
        public string? DescripcionPrenda { get; set; }

        public int? IdTalla { get; set; }

        public int? IdColor { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
