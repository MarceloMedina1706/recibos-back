namespace Template.DTOs
{
    public class UsuarioDTO
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Cuil { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Empresa { get; set; } = null!;
        public bool AuthEmail { get; set; }
        public bool PrimerLogin { get; set; }
        public int SinFirmar { get; set; } = 0;
    }
}
