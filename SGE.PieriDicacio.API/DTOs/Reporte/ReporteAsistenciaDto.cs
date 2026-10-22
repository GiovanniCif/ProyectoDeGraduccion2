namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class ReporteAsistenciaDto
    {
        public int Id { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public int TardanzaMinutos { get; set; }
        public decimal HorasTrabajadas { get; set; }
        public string? Observaciones { get; set; }
    }
}