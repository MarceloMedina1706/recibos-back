namespace Template.DTOs
{
    public class LiquidacionItemDTO
    {
        //mes - anio - id - firmado - tipo
        public int Liqui_Id { get; set; }
        public string Mes { get; set; } = null!;
        public string Anio { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public bool Firmado { get; set; }
        public bool? Visto { get; set; }
    }
}
