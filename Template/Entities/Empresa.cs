using System;
using System.Collections.Generic;

namespace Template.Entities
{
    public partial class Empresa
    {
        public Empresa()
        {
            Empleados = new HashSet<Empleado>();
        }

        public string Id { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string Responsable { get; set; } = null!;
        public int Dvh { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; }
    }
}
