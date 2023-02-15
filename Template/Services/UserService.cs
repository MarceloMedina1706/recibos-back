using Template.DTOs;
using Template.Entities;
using Template.Exceptions;
using Template.Utils;

namespace Template.Services
{
    public class UserService : IUserService
    {
        public Empleado GetEmpleado(UserLoginDTO userLogin)
        {
           using(var context = new LiquidacionContext())
            {
                var Empleado = context.Empleados.FirstOrDefault(e => e.Mail == userLogin.Mail && e.Clave == userLogin.Clave);
                if (Empleado == null) throw new EmpleadoNotFoundException("Credenciales erróneas.");
                return Empleado;
            }
        }

        public UsuarioDTO GetDatosUsuario(string EmpleadoId)
        {
            using (var context = new LiquidacionContext())
            { 
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
                UsuarioDTO user = (from emp in context.Empleados
                                   join empresa in context.Empresas
                                   on emp.EmpresaId equals empresa.Id
                                   where emp.Id == EmpleadoId && emp.Activo
                                   select new UsuarioDTO
                                   {
                                       Nombre = emp.Nombre,
                                       Apellido = emp.Apellido,
                                       Cuil = emp.Id,
                                       Mail = emp.Mail,
                                       Role = emp.Role,
                                       Empresa = empresa.RazonSocial,
                                       AuthEmail = emp.AuthEmail ?? false,
                                       PrimerLogin = emp.PrimerLogin ?? false,
                                   }).FirstOrDefault();
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL

                if (user == null) throw new EmpleadoNotFoundException("No se encontraron datos del empleado en la base de datos.");

                var cant = context.Liquidacions.Count(u => u.EmpleadoId == user.Cuil && u.Firmado.Year == 1900);
                user.SinFirmar = cant;
                return user;
            }
        }


        public bool VerificarEmpleado(string EmpleadoId, string Clave)
        {
            using(var context = new LiquidacionContext())
            {
                var anyrow = context.Empleados.Any(e => e.Id == EmpleadoId && e.Clave == Clave && e.Activo);
                return anyrow;
            }
        }

        public List<string> VerificarEmpleados(string[] Empleados)
        {
            using (var context = new LiquidacionContext())
            {
                var NotExist = new List<string>();
                foreach (var EmpleadoId in Empleados)
                {
                    var exist = context.Empleados.Where(e => e.Id == EmpleadoId).Any();
                    if (!exist)
                    {
                        NotExist.Add(EmpleadoId);
                    }
                }

                return NotExist;
            }
        }

        public void EnviarMailRecuperacion(UserRecuperarDTO datosUser)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var empleado = (from emp in dbContext.Empleados
                                where emp.Mail == datosUser.Email && emp.Id == datosUser.Cuil
                                select new
                                {
                                    EmpleadoId = emp.Id,
                                    mail = emp.Mail,
                                    nombre = emp.Nombre + " " + emp.Apellido
                                })
                               .FirstOrDefault();

                if (empleado != null)
                {
                    Guid miGuid = Guid.NewGuid();
                    string token = Convert.ToBase64String(miGuid.ToByteArray());
                    token = token.Replace('/', '-');
                    var Expires = DateTimeOffset.Now.AddMinutes(3);
                    var DateExpires = new DateTime(Expires.Year, Expires.Month, Expires.Day, Expires.Hour, Expires.Minute, Expires.Second);

                    TokenRecuperacion tkn = new TokenRecuperacion()
                    {
                        EmpleadoId = empleado.EmpleadoId,
                        Token = token,
                        Expitarion = DateExpires,
                        Activo = true
                    };

                    dbContext.Add(tkn);
                    var result = dbContext.SaveChanges();

                    if (result == 1)
                    {
                        var sent = MailSender.send(empleado.mail, empleado.nombre, tkn.Token);
                        if (!sent) throw new SendEmailException("Ha ocurrido un error al enviar el mail.");
                        else return;
                    }


                    throw new InvalidOperationException("Ha ocurrido un error con la base de datos." );
                }
                throw new EmpleadoNotFoundException("No se ha encontrado al empleado.");
            }
        }

        public UserRecuperadoDTO VerificarTokenRecuperacion(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new TokenException("Ha ocurrido un error con el token.");

            using (var dbContext = new LiquidacionContext())
            {
                var datos = (from tok in dbContext.TokenRecuperacions
                             join emp in dbContext.Empleados
                             on tok.EmpleadoId equals emp.Id
                             where tok.Token == token
                             select new
                             {
                                 Token = tok,
                                 Emp = emp
                             }).FirstOrDefault();
                if (datos != null)
                {
                    if ((datos.Token.Expitarion < DateTime.Now) || (!datos.Token.Activo))
                    {
                        throw new TokenExpiredException("El link ha expirado");
                    }

                    return new UserRecuperadoDTO
                    {
                        TokenId = datos.Token.Id,
                        EmpleadoId = datos.Emp.Id,
                        Nombre = datos.Emp.Nombre + " " + datos.Emp.Apellido
                    };
                }
                else
                {
                    throw new EmpleadoNotFoundException("No se ha encontrado al empleado.");
                }
            }
        }

        public void ReestablecerClave(ReestablecerClaveDTO datos)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var token = dbContext.TokenRecuperacions.Where(e => e.Id == datos.TokenId).FirstOrDefault();
                if (token != null)
                {
                    token.Activo = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new TokenException("No se encontró el token.");
                }

                var empleado = dbContext.Empleados.Where(e => e.Id == datos.EmpleadoId).FirstOrDefault();
                if (empleado != null)
                {
                    empleado.Clave = datos.Clave;
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new EmpleadoNotFoundException("No se ha encontrado al empleado.");
                }

                dbContext.Dispose();

            }
        }

        public void CambiarClave(ClaveNuevaDTO datos)
        {
            using (var dbContext = new LiquidacionContext())
            {
                if (datos.PrimerLogin ?? false)
                {
                    var Empleado = dbContext.Empleados.FirstOrDefault(e => e.Id == datos.Empleado);
                    if (Empleado == null) throw new EmpleadoNotFoundException("Clave errónea.");
                    Empleado.Clave = datos.ClaveNueva;
                    Empleado.PrimerLogin = false;
                    dbContext.SaveChanges();
                }
                else
                {
                    var Empleado = dbContext.Empleados.FirstOrDefault(e => e.Id == datos.Empleado && e.Clave == datos.Clave);
                    if (Empleado == null) throw new EmpleadoNotFoundException("Clave errónea.");
                    Empleado.Clave = datos.ClaveNueva;
                    dbContext.SaveChanges();
                }
                //    var Empleado = dbContext.Empleados.FirstOrDefault(e => e.Id == datos.Empleado && e.Clave == datos.Clave);
                //if (Empleado == null) throw new EmpleadoNotFoundException("Clave errónea.");

                //Empleado.Clave = datos.ClaveNueva;
                //if (datos.PrimerLogin ?? false)
                //{
                //    Empleado.PrimerLogin = false;
                //}

                //dbContext.SaveChanges();
            }
        }

        public string CambiarEmail(EmailNuevoDTO datos)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var Usuario = dbContext.Empleados.Where(u => u.Id == datos.Empleado && u.Clave == datos.Clave).FirstOrDefault();

                if (Usuario == null)
                {
                    throw new EmpleadoNotFoundException("Clave errónea.");
                }


                Usuario.Mail = datos.EmailNuevo;
                dbContext.SaveChanges();

                return datos.EmailNuevo;
            }
        }

        public void EstablecerAuthEmail(AuthEmailDTO Datos)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var Emp = dbContext.Empleados.Where(e => e.Id == Datos.Cuil).FirstOrDefault();
                if (Emp != null)
                {
                    Emp.AuthEmail = Datos.AuthEmail;
                    dbContext.SaveChanges();

                }
                else
                {
                    throw new EmpleadoNotFoundException("No se ha encontrado al empleado.");
                }
            }
        }
    }
}
