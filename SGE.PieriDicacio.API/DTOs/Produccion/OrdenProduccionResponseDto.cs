namespace SGE.PieriDicacio.API.DTOs.Produccion
{
    public class OrdenProduccionResponseDto
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public int IdProductoTerminado { get; set; }
        public string? ProductoTerminado { get; set; }
        public string? ProductoTerminadoCodigo { get; set; }
        public int? IdPedido { get; set; }
        public string? NumeroPedido { get; set; }
        public decimal CantidadProgramada { get; set; }
        public decimal CantidadProducida { get; set; }
        public decimal CantidadPendiente => CantidadProgramada - CantidadProducida;
        public int IdEstadoOrdenProduccion { get; set; }
        public string? EstadoOrdenProduccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdUsuarioResponsable { get; set; }
        public string? UsuarioResponsable { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; }
    }
}
