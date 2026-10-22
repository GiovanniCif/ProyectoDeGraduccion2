namespace SGE.PieriDicacio.API.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public int IdEstadoPedido { get; set; }
        public int? IdDepartamentoResponsable { get; set; }
        public int? IdUsuarioResponsable { get; set; }
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Prioridad { get; set; } = "MEDIA";
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public Cliente Cliente { get; set; } = null!;
        public EstadoPedido EstadoPedido { get; set; } = null!;
        public Departamento? DepartamentoResponsable { get; set; }
        public Usuario? UsuarioResponsable { get; set; }

        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
        public ICollection<PedidoHistorialEstado> HistorialEstados { get; set; } = new List<PedidoHistorialEstado>();
    }
}