using System;
using System.Collections.Generic;

namespace Template.Entities
{
    public partial class Empleado
    {
        public Empleado()
        {
            Liquidacions = new HashSet<Liquidacion>();
            TokenRecuperacions = new HashSet<TokenRecuperacion>();
        }

        public string Id { get; set; } = null!;
        public string EmpresaId { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public DateTime Ingreso { get; set; }
        public string Role { get; set; } = null!;
        public byte[]? Firma { get; set; }
        public string Mail { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public bool Activo { get; set; }
        public int Dvh { get; set; }
        public bool? AuthEmail { get; set; }
        public bool? PrimerLogin { get; set; }

        public virtual Empresa Empresa { get; set; } = null!;
        public virtual ICollection<Liquidacion> Liquidacions { get; set; }
        public virtual ICollection<TokenRecuperacion> TokenRecuperacions { get; set; }
    }
}
