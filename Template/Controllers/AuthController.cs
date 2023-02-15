using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Template.DTOs;
using Template.Exceptions;
using Template.Helpers;
using Template.Services;
using Template.Utils;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;
        public AuthController(JwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("/login")]
        public async Task<ActionResult> Login(UserLoginDTO userLogin)
        {
            try
            {
                var Empleado = _userService.GetEmpleado(userLogin);
                var token = _jwtService.Generate(Empleado.EmpresaId); // Empleado.EmpresaId == Cuil
                int Code  = 1;// CODE 1 indica que todo salio bien
                string codigo = null;
                if (Empleado.PrimerLogin ?? false)
                {
                    Code = 2;// CODE 2 indica que es el primer login
                }
                else if(Empleado.AuthEmail ?? false)
                {
                    Code = 3;// CODE 3 indica que tiene doble autenticacion
                    Guid miGuid = Guid.NewGuid();
                    codigo = Convert.ToBase64String(miGuid.ToByteArray());
                    codigo = codigo.Replace('/', '-');
                    codigo = codigo.Substring(0, 4);
                    MailSender.SendCodigoVerificacion(Empleado.Mail, Empleado.Nombre, codigo);
                }

                if(Code == 3)
                {
                    var Response = new ResponseDTO()
                    {
                        Code = Code,
                        Data = new { token = token[0], expire = token[1], empleadoId = Empleado.Id, Verificacion = codigo }
                    };

                    return Ok(Response);
                }
                else
                {
                    var Response = new ResponseDTO()
                    {
                        Code = Code,
                        Data = new { token = token[0], expire = token[1], empleadoId = Empleado.Id }
                    };

                    return Ok(Response);
                }

            }catch(EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }catch(InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch(Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpGet("/GetDatosUsuario/{cuil}")]
        public async Task<ActionResult> GetDatosUsuario(string cuil)
        {
            try
            {
                var Empleado = _userService.GetDatosUsuario(cuil);


                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Data = Empleado
                };

                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }


        [Authorize]
        [HttpPost("/VerificarEmpleados")]
        public async Task<ActionResult> VerificarEmpleados(string[] cuils)
        {
            try
            {
                var Empleados = _userService.VerificarEmpleados(cuils);

                if (Empleados.Any())
                {
                    var Response = new ResponseDTO()
                    {
                        Code = 2,// CODE 2 indica que hay empledos que no estan cargados en la BBDD
                        Data = Empleados
                    };
                    return Ok(Response);
                }
                else
                {
                    var Response = new ResponseDTO()
                    {
                        Code = 1,// CODE 1 indica que todo salio bien
                    };
                    return Ok(Response);
                }
                
            }
            catch (InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [HttpPost("/RecuperarClave")]
        public async Task<ActionResult> RecuperarClave(UserRecuperarDTO datosUser)
        {
            try
            {
                _userService.EnviarMailRecuperacion(datosUser);


                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Message = "Te hemos enviado un mail con el enlace de recuperación."
                };

                return Ok(Response);
            }
            catch (SendEmailException e)
            {
                
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [HttpGet("/VerificarTokenRecuperacion/{token}")]
        public async Task<ActionResult> VerificarTokenRecuperacion(string token)
        {
            try
            {
                var empleado = _userService.VerificarTokenRecuperacion(token);

                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                   Data = empleado
                };

                return Ok(Response);
            }
            catch (TokenExpiredException e)
            {

                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (TokenException e)
            {

                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException)
            { 
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("/ReestablecerClave")]
        public async Task<ActionResult> ReestablecerClave(ReestablecerClaveDTO Datos)
        {
            try
            {
                _userService.ReestablecerClave(Datos);

                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Message = "La clave se reestableció correctamente."
                };

                return Ok(Response);
            }
            catch (TokenException e)
            {

                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/CambiarClave")]
        public async Task<ActionResult> CambiarClave(ClaveNuevaDTO Datos)
        {
            try
            {
                _userService.CambiarClave(Datos);

                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Message = "La clave se ha cambiado."
                };

                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/CambiarEmail")]
        public async Task<ActionResult> CambiarEmail(EmailNuevoDTO Datos)
        {
            try
            {
                var email = _userService.CambiarEmail(Datos);

                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Message = "El email se ha cambiado.",
                    Data = email
                };

                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/EstablecerAuthEmail")]
        public async Task<ActionResult> EstablecerAuthEmail(AuthEmailDTO Datos)
        {
            try
            {
                _userService.EstablecerAuthEmail(Datos);

                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Message = "El cambio se ha realizado satisfactoriamente."
                };

                return Ok(Response);
            }
            catch (EmpleadoNotFoundException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = e.Message
                };
                return Ok(Response);
            }
            catch (InvalidOperationException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con la base de datos."
                };
                return Ok(Response);
            }
            catch (Exception)
            {
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/RefreshToken/{Token}")]
        public async Task<ActionResult> RefreshToken(string Token)
        {

            try
            {
                var Cuil = _jwtService.GetClaim(Token);
                var NewToken = _jwtService.Generate(Cuil);
                return Ok(new ResponseDTO
                {
                    Code = 1,
                    Data = new
                    {
                        Token = NewToken[0],
                        Expires = NewToken[1]
                    }
                });
            }
            catch
            {
                return Ok(new ResponseDTO
                {
                    Code = 0,
                    Message = "Ha ocurrido un error con el servidor."
                }) ;
            }
            
        }
    }
}
