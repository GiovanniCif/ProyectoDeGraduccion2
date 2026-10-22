namespace SGE.PieriDicacio.API.DTOs.Empleado
{
    public class PermisoEmpleadoResponseDto
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public int IdTipoPermiso { get; set; }
        public string TipoPermiso { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string? DocumentoUrl { get; set; }
        public string? AutorizadoPor { get; set; }
        public bool Estado { get; set; }
    }
}