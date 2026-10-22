using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Cliente
{
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El tipo de cliente es obligatorio")]
        public string TipoCliente { get; set; } = "INDIVIDUAL";

        [Required(ErrorMessage = "El nombre o razón social es obligatorio")]
        [StringLength(200, MinimumLength = 2)]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [StringLength(100)]
        public string? NombreContacto { get; set; }

        [StringLength(20)]
        public string? Nit { get; set; }

        public string? TelefonoPrincipal { get; set; }

        public string? TelefonoSecundario { get; set; }

        public string? Correo { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Municipio { get; set; }

        [StringLength(100)]
        public string? Departamento { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
