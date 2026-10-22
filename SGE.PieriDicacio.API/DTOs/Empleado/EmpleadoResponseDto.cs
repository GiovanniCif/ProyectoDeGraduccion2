namespace SGE.PieriDicacio.API.DTOs.Empleado
{
    public class EmpleadoResponseDto
    {
        public int Id { get; set; }
        public int IdDepartamento { get; set; }
        public string Departamento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string? Dpi { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? Puesto { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Estado { get; set; }
    }
}