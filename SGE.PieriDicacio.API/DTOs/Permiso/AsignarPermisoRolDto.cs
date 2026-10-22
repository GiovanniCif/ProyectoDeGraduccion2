using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Permiso
{
    public class AsignarPermisoRolDto
    {
        [Required(ErrorMessage = "El permiso es obligatorio")]
        public int IdPermiso { get; set; }
    }
}
