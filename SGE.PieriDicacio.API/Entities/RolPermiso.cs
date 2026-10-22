namespace SGE.PieriDicacio.API.Entities
{
    public class RolPermiso
    {
        public int Id { get; set; }
        public int IdRol { get; set; }
        public int IdPermiso { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public Rol Rol { get; set; } = null!;
        public Permiso Permiso { get; set; } = null!;
    }
}