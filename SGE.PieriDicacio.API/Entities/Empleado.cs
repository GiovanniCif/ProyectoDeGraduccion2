namespace SGE.PieriDicacio.API.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public int IdDepartamento { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string? Dpi { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? Puesto { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public Departamento Departamento { get; set; } = null!;
    }
}