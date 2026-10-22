namespace SGE.PieriDicacio.API.Entities
{
    public class OrdenProduccion
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public int IdProductoTerminado { get; set; }
        public int? IdPedido { get; set; }
        public decimal CantidadProgramada { get; set; }
        public decimal CantidadProducida { get; set; }
        public int IdEstadoOrdenProduccion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdUsuarioResponsable { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;

        public ProductoTerminado? ProductoTerminado { get; set; }
        public Pedido? Pedido { get; set; }
        public EstadoOrdenProduccion? EstadoOrdenProduccion { get; set; }
        public Usuario? UsuarioResponsable { get; set; }
    }
}
