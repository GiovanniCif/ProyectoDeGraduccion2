namespace SGE.PieriDicacio.API.DTOs.Rol
{
    public class RolResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}