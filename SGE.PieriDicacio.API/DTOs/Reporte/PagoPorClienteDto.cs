namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class PagoPorClienteDto
    {
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int CantidadPagos { get; set; }
        public decimal TotalPagado { get; set; }
    }
}
