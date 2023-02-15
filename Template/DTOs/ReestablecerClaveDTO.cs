namespace Template.DTOs
{
    public class ReestablecerClaveDTO
    {
        public int TokenId { get; set; }
        public string EmpleadoId { get; set; } = null!;
        public string Clave { get; set; } = null!;
    }
}
