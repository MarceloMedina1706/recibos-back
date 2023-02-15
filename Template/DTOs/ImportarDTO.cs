namespace Template.DTOs
{
    public class ImportarDTO
    {
        public int LiquiId { get; set; }
        public string? TipoLiquidacion { get; set; }
        public DatosLiquidacionDTO[] Liquis { get; set; } = null!;
    }
}
