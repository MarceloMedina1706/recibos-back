namespace Template.DTOs
{
    public class LiquidacionDTO
    {
        public string Empresa { get; set; } = null!;
        public int LiquiNumero { get; set; }
        public string Cuit { get; set; } = null!;
        public string UltimoDeposito { get; set; } = null!;
        public string Banco { get; set; } = null!;
        public string Cuil { get; set; } = null!;
        public string Beneficiario { get; set; } = null!;
        public string Ingreso { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public string RemBasica { get; set; } = null!;
        public string PeriodoLiquidado { get; set; } = null!;
        public List<CodigoLiquidacionDTO> Codigos { get; set; } = null!;
        public string? TotalHaberes { get; set; }
        public string? TotalDeducciones { get; set; }
        public string? TotalNeto { get; set; }
        public string TotalNetoEnPalabras { get; set; } = null!;
        public byte[]? Firma { get; set; }
        public bool Firmado { get; set; }
        public string? FechaFirmado { get; set; }
    }
}
