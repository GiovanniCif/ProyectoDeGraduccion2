namespace SGE.PieriDicacio.API.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public int IdMetodoPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? NumeroReferencia { get; set; }
        public string? Observaciones { get; set; }
        public string? VoucherUrl { get; set; }
        public int IdUsuario { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Pedido Pedido { get; set; } = null!;
        public Cliente Cliente { get; set; } = null!;
        public MetodoPago MetodoPago { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}
