using System;
using System.Collections.Generic;

namespace Template.Entities
{
    public partial class LiquiCodLiquidado
    {
        public int LiquiId { get; set; }
        public string EmpleadoId { get; set; } = null!;
        public int Codigo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Importe { get; set; }
        public string CodDescripcion { get; set; } = null!;
        public string CodTipo { get; set; } = null!;
        public int Dvh { get; set; }

        public virtual Liquidacion Liquidacion { get; set; } = null!;
    }
}
