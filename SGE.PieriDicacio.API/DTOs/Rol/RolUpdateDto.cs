using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Rol
{
    public class RolUpdateDto
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50, MinimumLength = 2)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }
    }
}
