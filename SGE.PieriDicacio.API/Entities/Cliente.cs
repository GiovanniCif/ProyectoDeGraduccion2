namespace SGE.PieriDicacio.API.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string TipoCliente { get; set; } = "INDIVIDUAL";
        public string NombreRazonSocial { get; set; } = string.Empty;
        public string? NombreContacto { get; set; }
        public string? Nit { get; set; }
        public string? TelefonoPrincipal { get; set; }
        public string? TelefonoSecundario { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? Municipio { get; set; }
        public string? Departamento { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }
    }
}