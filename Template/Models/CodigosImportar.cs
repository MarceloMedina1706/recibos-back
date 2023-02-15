namespace Template.Models
{
    public class CodigosImportar
    {
        public int Codigo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Importe { get; set; }
        public string CodDescripcion { get; set; } = null!;
        public string CodTipo { get; set; } = null!;
    }
}
