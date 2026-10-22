namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class EstadoPedidoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Orden { get; set; }
        public bool Estado { get; set; }
    }
}
