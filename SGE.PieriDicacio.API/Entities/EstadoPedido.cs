namespace SGE.PieriDicacio.API.Entities
{
    public class EstadoPedido
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Orden { get; set; }
        public bool Estado { get; set; } = true;

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}