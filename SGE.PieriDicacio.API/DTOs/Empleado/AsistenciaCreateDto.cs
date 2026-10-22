using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Empleado
{
    public class AsistenciaCreateDto
    {
        [Required(ErrorMessage = "El empleado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un empleado válido")]
        public int IdEmpleado { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public TimeSpan? HoraEntrada { get; set; }

        public TimeSpan? HoraSalida { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
