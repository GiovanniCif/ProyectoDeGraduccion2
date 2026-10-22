using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Empleado
{
    public class EmpleadoCreateDto
    {
        [Required(ErrorMessage = "El departamento es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un departamento válido")]
        public int IdDepartamento { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100, MinimumLength = 2)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100, MinimumLength = 2)]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Dpi { get; set; }

        [Phone(ErrorMessage = "El teléfono no es válido")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string? Correo { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Puesto { get; set; }

        public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    }
}
