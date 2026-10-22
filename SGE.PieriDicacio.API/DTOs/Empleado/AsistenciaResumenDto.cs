namespace SGE.PieriDicacio.API.DTOs.Empleado
{
    public class AsistenciaResumenDto
    {
        public int IdEmpleado { get; set; }
        public string Empleado { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string? Puesto { get; set; }
        public int DiasHabiles { get; set; }
        public int DiasTrabajados { get; set; }
        public int Faltas { get; set; }
        public int Tardanzas { get; set; }
        public int MinutosTardanza { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal HorasExtras { get; set; }
    }
}
