using System;
using System.Collections.Generic;

namespace Template.Entities
{
    public partial class TipoLiquidacion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
    }
}
