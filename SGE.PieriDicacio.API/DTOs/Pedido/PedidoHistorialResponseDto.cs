namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class PedidoHistorialResponseDto
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public int? IdEstadoAnterior { get; set; }
        public string? EstadoAnterior { get; set; }
        public int IdEstadoNuevo { get; set; }
        public string EstadoNuevo { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public int? IdDepartamento { get; set; }
        public string? Departamento { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCambio { get; set; }
    }
}
