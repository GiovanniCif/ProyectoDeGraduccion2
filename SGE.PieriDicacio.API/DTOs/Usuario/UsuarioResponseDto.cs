namespace SGE.PieriDicacio.API.DTOs.Usuario
{
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Estado { get; set; }
    }
}