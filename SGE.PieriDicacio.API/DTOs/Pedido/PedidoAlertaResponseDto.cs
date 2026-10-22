namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class PedidoAlertaResponseDto
    {
        public int Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int IdEstadoPedido { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int DiasAtraso { get; set; }
        public string TipoAlerta { get; set; } = string.Empty;
    }
}
