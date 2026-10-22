namespace SGE.PieriDicacio.API.DTOs.Reporte
{
    public class ExportResultDto
    {
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoContenido { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public byte[] Contenido { get; set; } = Array.Empty<byte>();
    }
}
