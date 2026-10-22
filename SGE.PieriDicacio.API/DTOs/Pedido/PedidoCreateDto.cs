using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class PedidoCreateDto
    {
        [Required(ErrorMessage = "El cliente es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente válido")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El estado del pedido es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estado válido")]
        public int IdEstadoPedido { get; set; }

        public int? IdDepartamentoResponsable { get; set; }

        public int? IdUsuarioResponsable { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
        public DateTime FechaEntrega { get; set; }

        [StringLength(20)]
        public string Prioridad { get; set; } = "MEDIA";

        [Range(0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo")]
        public decimal Descuento { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "El pedido debe tener al menos un detalle")]
        public List<PedidoDetalleCreateDto> Detalles { get; set; } = new();
    }
}
