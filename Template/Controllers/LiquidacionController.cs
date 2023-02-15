
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.DTOs;
using Template.Exceptions;
using Template.Helpers;
using Template.Services;

namespace Template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LiquidacionController : ControllerBase
    {

        private readonly ILiquidacionService _liquidacionService;
        private readonly IUserService _userService;

        public LiquidacionController(ILiquidacionService liquidacionService, IUserService userService)
        {
            _liquidacionService = liquidacionService;
            _userService = userService;
        }

        
        [HttpGet("/GetItemsLiquidaciones/{cuil}")]
        public async Task<ActionResult> GetItemsLiquidaciones(string cuil)
        {
            try
            {
                var Items = _liquidacionService.GetLiquidacionItems(cuil);


                var Response = new ResponseDTO()
                {
                    Code = 1,// CODE 1 indica que todo salio bien
                    Data = Items
                };

                return Ok(Response);
            }
            catch (LiquidacionNotFoundException e)
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

        
        [HttpGet("/GetLiquidacion/{cuil}/{liquiId}")]
        public async Task<ActionResult> GetLiquidacion(string cuil, int liquiId)
        {
            try
            {
                var liquidacion = _liquidacionService.GetLiquidacion(cuil, liquiId);
                
                int code = 1;// CODE 1 indica que el recibo esta firmado
                if (!liquidacion.Firmado) { code = 2; }// CODE 2 indica que el recibo esta visto pero no firmado

                var Response = new ResponseDTO()
                {
                    Code = code,
                    Data = liquidacion
                };

                return Ok(Response);
            }
            catch (RecibosPreviosException e)
            {
                var Response = new ResponseDTO()
                {
                    Code = 3,// CODE 3 indica que tiene recibos previos
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

        
        [HttpPost("/GetLiquidaciones")]
        public async Task<ActionResult> GetLiquidaciones(GetRecibosDTO getRecibos)
        {
            try
            {
                List<LiquidacionDTO> liquidaciones = new List<LiquidacionDTO>();
                var cant = getRecibos.LiquiId.Count();
                for(int i=0; i<cant; i++)
                {
                    var liquidacion = _liquidacionService.GetLiquidaciones(getRecibos.EmpleadoId, getRecibos.LiquiId[i]);
                    liquidaciones.Add(liquidacion);
                }


                var Response = new ResponseDTO()
                {
                    Code = 1,
                    Data = liquidaciones
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
            catch (Exception r)
            {
                
                var Response = new ResponseDTO()
                {
                    Code = 0,// CODE 0 indica error
                    Message = "Ha ocurrido un error con el servidor."
                };
                return Ok(Response);
            }
        }

        
        [HttpGet("/FirmarRecibo/{cuil}/{clave}/{liquiId}")]
        public async Task<ActionResult> FirmarRecibo(string cuil, string clave, int liquiId)
        {
            try
            {
                var res = _userService.VerificarEmpleado(cuil, clave);

                if (!res)
                {
                    var Response = new ResponseDTO()
                    {
                        Code = 0,// CODE 0 indica error
                        Message = "Clave erronea."
                    };
                    return Ok(Response);
                }


                var firmado = _liquidacionService.setFirmar(cuil, liquiId);
                if (firmado)
                {
                    var Response = new ResponseDTO()
                    {
                        Code = 1, //CODE 1 indica que el recibo se firmo
                        Message = "El documentos se ha firmado correctamente."
                    };
                    return Ok(Response);
                }
                else
                {
                    var Response = new ResponseDTO()
                    {
                        Code = 0,// CODE 0 indica error
                        Message = "Ha ocurrido un error con el servidor."

                    };
                    return Ok(Response);
                }
                
                
            }
            catch (InvalidOperationException e)
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

        
        [HttpPost("/ImportarLiquidaciones/{Force?}")]
        public async Task<ActionResult> ImportarLiquidaciones(ImportarDTO importar, bool? Force = false)
        {
            try
            {
                if (Force ?? false)
                {
                    _liquidacionService.ImportarLiquidacionesForce(importar);
                }
                else
                {
                    _liquidacionService.ImportarLiquidaciones(importar);
                }

                return Ok(new ResponseDTO { Code = 1, Message = "Los registros se cargaron satisfactoriamente." });//Indica que todo salió bien
            }
            catch (LiquidacionAlreadyLoadedException e)
            {

                if (importar.LiquiId.ToString().EndsWith("03"))
                {
                    return Ok(new ResponseDTO
                    {
                        Code = 3,//Indica que ya cargo esa liquidacion, pero pueda cargarlo otra vez con otro tipo de liquidacion ( es complementaria)
                        Message = e.Message
                    });
                }
                else
                {
                    return Ok(new ResponseDTO
                    {
                        Code = 2,//Indica que ya cargo esa liquidacion (no es complementaria)
                        Message = e.Message
                    });
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

        
        [HttpGet("/ObtenerLiquidaciones/{User}")]
        public async Task<ActionResult> ObtenerLiquidaciones(string User)
        {
            try
            {
                var Liquis = _liquidacionService.ObtenerLiquidaciones(User);

                return Ok(new ResponseDTO { Code = 1, Data = Liquis });//Indica que todo salió bien
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

        
        [HttpPost("/GetEmpleadosLiquidacion")]
        public async Task<ActionResult> GetEmpleadosLiquidacion(UserLiquiDTO UserLiqui)
        {
            try
            {
                var empleados = _liquidacionService.GetEmpleadosLiquidacion(UserLiqui);

                return Ok(new ResponseDTO { Code = 1, Data = empleados });//Indica que todo salió bien
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

        
        [HttpPost("/GetLiquidacionEspecifica")]
        public async Task<ActionResult> GetLiquidacionEspecifica(UserLiquiEmpleadoDTO Datos)
        {
            try
            {
                var liquidacion = _liquidacionService.GetLiquidacionEspecifica(Datos);

                return Ok(new ResponseDTO { Code = 1, Data = liquidacion });//Indica que todo salió bien
            }
            catch (LiquidacionNotFoundException e)
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

        
        [HttpGet("/EliminarLiquidacion/{LiquiId}")]
        public async Task<ActionResult> EliminarLiquidacion(int LiquiId)
        {
            try
            {
                _liquidacionService.EliminarLiquidacion(LiquiId);

                return Ok(new ResponseDTO { Code = 1, Message = "La operación se completó satisfactoriamente." });//Indica que todo salió bien
            }catch (InvalidOperationException)
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



    }
}
