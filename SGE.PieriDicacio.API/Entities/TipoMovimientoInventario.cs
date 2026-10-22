namespace SGE.PieriDicacio.API.Entities
{
    public class TipoMovimientoInventario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool AfectaStock { get; set; } = true;
        public string TipoEfecto { get; set; } = "NEUTRO";
        public bool Estado { get; set; } = true;
    }
}
