using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Bitacora
{
    public class BitacoraCreateDto
    {
        [Required(ErrorMessage = "El módulo es obligatorio")]
        [StringLength(100)]
        public string Modulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La acción es obligatoria")]
        [StringLength(100)]
        public string Accion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La tabla afectada es obligatoria")]
        [StringLength(100)]
        public string TablaAfectada { get; set; } = string.Empty;

        public long IdRegistroAfectado { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        public string? DatosAnteriores { get; set; }

        public string? DatosNuevos { get; set; }

        [StringLength(50)]
        public string? IpEquipo { get; set; }
    }
}
