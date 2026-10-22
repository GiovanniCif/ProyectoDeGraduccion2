using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Empleado
{
    public class PermisoEmpleadoCreateDto
    {
        [Required(ErrorMessage = "El empleado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un empleado válido")]
        public int IdEmpleado { get; set; }

        [Required(ErrorMessage = "El tipo de permiso es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un tipo de permiso válido")]
        public int IdTipoPermiso { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500, MinimumLength = 5)]
        public string Motivo { get; set; } = string.Empty;

        public string? DocumentoUrl { get; set; }

        [StringLength(100)]
        public string? AutorizadoPor { get; set; }
    }
}
