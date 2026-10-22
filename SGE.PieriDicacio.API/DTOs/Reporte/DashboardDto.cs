namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class DashboardDto
    {
        public int TotalClientes { get; set; }
        public int TotalPedidos { get; set; }
        public int PedidosPendientes { get; set; }
        public int PedidosEntregados { get; set; }
        public decimal TotalCobrado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public int TotalProductosTerminados { get; set; }
        public int TotalInsumos { get; set; }
        public int AsistenciasRegistradas { get; set; }
        public int EmpleadosActivos { get; set; }
    }
}