namespace SGE.PieriDicacio.API.Entities
{
    public class PedidoHistorialEstado
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int? IdEstadoAnterior { get; set; }
        public int IdEstadoNuevo { get; set; }
        public int IdUsuario { get; set; }
        public int? IdDepartamento { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.Now;

        public Pedido Pedido { get; set; } = null!;
        public EstadoPedido EstadoNuevo { get; set; } = null!;
        public EstadoPedido? EstadoAnterior { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Departamento? Departamento { get; set; }
    }
}