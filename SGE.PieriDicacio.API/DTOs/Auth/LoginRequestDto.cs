using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El usuario o correo es obligatorio")]
        public string UsernameOrCorreo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
