namespace SGE.PieriDicacio.API.Entities
{
    public class PermisoEmpleado
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int IdTipoPermiso { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? DocumentoUrl { get; set; }
        public string? AutorizadoPor { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Empleado Empleado { get; set; } = null!;
        public TipoPermiso TipoPermiso { get; set; } = null!;
    }
}