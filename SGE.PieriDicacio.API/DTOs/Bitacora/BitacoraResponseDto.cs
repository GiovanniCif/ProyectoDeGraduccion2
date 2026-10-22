namespace SGE.PieriDicacio.API.DTOs.Bitacora
{
    public class BitacoraResponseDto
    {
        public long Id { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string TablaAfectada { get; set; } = string.Empty;
        public long IdRegistroAfectado { get; set; }
        public string? Descripcion { get; set; }
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public string? IpEquipo { get; set; }
        public DateTime FechaEvento { get; set; }
    }
}