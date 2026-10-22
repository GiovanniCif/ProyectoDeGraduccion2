namespace SGE.PieriDicacio.API.Entities
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public int TardanzaMinutos { get; set; }
        public decimal HorasTrabajadas { get; set; }
        public string? Observaciones { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Empleado Empleado { get; set; } = null!;
        public Usuario UsuarioRegistro { get; set; } = null!;
    }
}