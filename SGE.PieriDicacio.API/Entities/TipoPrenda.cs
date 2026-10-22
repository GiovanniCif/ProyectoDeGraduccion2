namespace SGE.PieriDicacio.API.Entities
{
    public class TipoPrenda
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
    }
}