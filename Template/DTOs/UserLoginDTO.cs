using System.ComponentModel.DataAnnotations;

namespace Template.DTOs
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage="Este campo es requerido.")]
        //[EmailAddress(ErrorMessage = "Ingrese un email valido.")]
        public string Mail { get; set; } = null!;

        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Clave { get; set; } = null!;
    }
}
