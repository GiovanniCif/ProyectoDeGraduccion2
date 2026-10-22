using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Pago
{
    public class PagoCreateDto
    {
        [Required(ErrorMessage = "El pedido es obligatorio")]
        [Range(1, int.MaxValue)]
        public int IdPedido { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        [Range(1, int.MaxValue)]
        public int IdMetodoPago { get; set; }

        public DateTime FechaPago { get; set; } = DateTime.UtcNow;

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [StringLength(100)]
        public string? NumeroReferencia { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public string? VoucherUrl { get; set; }
    }
}
