namespace Template.DTOs
{
    public class ClaveNuevaDTO
    {
        public string Empleado { get; set; } = null!;
        //[Required]
        //[MinLength(6, ErrorMessage = "LA pass tiene que tener mas las 6 length")]
        public string Clave { get; set; } = null!;
        public string ClaveNueva { get; set; } = null!;
        public bool? PrimerLogin { get; set; } = false;
    }
}
