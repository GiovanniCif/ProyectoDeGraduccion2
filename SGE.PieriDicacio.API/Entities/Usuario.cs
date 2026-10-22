namespace SGE.PieriDicacio.API.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public int IdRol { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime? UltimoAcceso { get; set; }
        public int IntentosFallidos { get; set; } = 0;
        public DateTime? BloqueadoHasta { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        public Rol Rol { get; set; } = null!;
    }
}