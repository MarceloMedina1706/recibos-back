namespace Template.DTOs
{
    public class DatosLiquiEmpleadoDTO
    {
        public string EmpleadoId { get; set; } = null!;
        public string TotalHaberes { get; set; } = null!;
        public string TotalDeducciones { get; set; } = null!;
        public string TotalNeto { get; set; } = null!;
        public int? CantCodigos { get; set; }

        public string? Categoria { get; set; }
        public string? Descripcion { get; set; }
        public string? SueldoBasico { get; set; }
        public DateTime? FecUltDep { get; set; }
        public string? Banco { get; set; }
        public List<CodigoLiquidacionDTO>? CodigoLiquidaciones { get; set; }
    }
}
