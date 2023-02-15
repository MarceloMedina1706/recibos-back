namespace Template.DTOs
{
    public class CodigoLiquidacionDTO
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = null!;
        public string Cantidad { get; set; } = null!;
        public string Importe { get; set; } = null!;
        public string CodTipo { get; set; } = null!;
        public string? Porcentaje { get; set; }
    }
}
