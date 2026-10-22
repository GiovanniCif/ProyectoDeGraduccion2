namespace SGE.PieriDicacio.API.Entities
{
    public class Permiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;

        public ICollection<RolPermiso> RolesPermisos { get; set; } = new List<RolPermiso>();
    }
}