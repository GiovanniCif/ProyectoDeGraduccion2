namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class IngresoPeriodoDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadPagos { get; set; }
        public decimal Monto { get; set; }
    }
}
