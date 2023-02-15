namespace Template.DTOs
{
    public class DatosLiquiDTO
    {
        public int LiquiId { get; set; }
        public string Periodo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int? CantidadEmpleados { get; set; }
        public decimal SumaTotalHaberes { get; set; }
        public decimal SumaTotalDeducciones { get; set; }
        public decimal SumaTotalNetos { get; set; }
    }
}
