using System.ComponentModel.DataAnnotations;

namespace SGE.PieriDicacio.API.DTOs.Pedido
{
    public class CambiarEstadoPedidoDto
    {
        [Required(ErrorMessage = "El nuevo estado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un estado válido")]
        public int IdEstadoNuevo { get; set; }

        public int? IdDepartamento { get; set; }

        [StringLength(500)]
        public string? Comentario { get; set; }
    }
}
