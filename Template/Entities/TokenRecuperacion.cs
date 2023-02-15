using System;
using System.Collections.Generic;

namespace Template.Entities
{
    public partial class TokenRecuperacion
    {
        public int Id { get; set; }
        public string? EmpleadoId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expitarion { get; set; }
        public bool Activo { get; set; }

        public virtual Empleado? Empleado { get; set; }
    }
}
