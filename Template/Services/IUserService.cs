using Template.DTOs;
using Template.Entities;

namespace Template.Services
{
    public interface IUserService
    {
        public Empleado GetEmpleado(UserLoginDTO userLogin);
        public UsuarioDTO GetDatosUsuario(string EmpleadoId);
        public bool VerificarEmpleado(string EmpleadoId, string Clave);
        public List<string> VerificarEmpleados(string[] Empleados);
        public void EnviarMailRecuperacion(UserRecuperarDTO datosUser);
        public UserRecuperadoDTO VerificarTokenRecuperacion(string token);
        public void ReestablecerClave(ReestablecerClaveDTO datos);
        public void CambiarClave(ClaveNuevaDTO datos);
        public string CambiarEmail(EmailNuevoDTO datos);
        public void EstablecerAuthEmail(AuthEmailDTO Datos);
    }
}
