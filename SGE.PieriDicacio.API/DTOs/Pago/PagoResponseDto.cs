namespace SGE.PieriDicacio.API.DTOs.Pago
{
    public class PagoResponseDto
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int IdMetodoPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? NumeroReferencia { get; set; }
        public string? Observaciones { get; set; }
        public string? VoucherUrl { get; set; }
        public int IdUsuario { get; set; }
        public bool Estado { get; set; }
    }
}
