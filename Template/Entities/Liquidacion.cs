using System;
using System.Collections.Generic;

namespace Template.Entities
{
    public partial class Liquidacion
    {
        public Liquidacion()
        {
            LiquiCodLiquidados = new HashSet<LiquiCodLiquidado>();
        }

        public int LiquiId { get; set; }
        public string EmpleadoId { get; set; } = null!;
        public int ReciboNro { get; set; }
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = null!;
        public decimal SueldoBasico { get; set; }
        public string Banco { get; set; } = null!;
        public DateTime FecUltDeposito { get; set; }
        public decimal TotalHaberes { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal TotalNeto { get; set; }
        public DateTime Firmado { get; set; }
        public bool? Visto { get; set; }
        public int Dvh { get; set; }

        public virtual Empleado Empleado { get; set; } = null!;
        public virtual ICollection<LiquiCodLiquidado> LiquiCodLiquidados { get; set; }
    }
}
