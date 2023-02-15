using Template.Models;

namespace Template.DTOs
{
    public class DatosLiquidacionDTO
    {
        public string EmpleadoId { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public decimal SueldoBasico { get; set; }
        public string Banco { get; set; } = null!;
        public string FecUltDep { get; set; } = null!;
        public decimal TotHaberes { get; set; }
        public decimal TotDeducciones { get; set; }
        public decimal TotNeto { get; set; }
        public List<CodigosImportar> Codigos { get; set; } = null!;
    }
}
