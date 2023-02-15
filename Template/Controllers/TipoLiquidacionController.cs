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
    public class TipoLiquidacionController : ControllerBase
    {
        private readonly ITipoLiquidacionService _tipoLiquidacionService;
        public TipoLiquidacionController(ITipoLiquidacionService tipoLiquidacionService)
        {
            _tipoLiquidacionService = tipoLiquidacionService;
        }


        //[Authorize]
        [HttpGet("/GetTipoLiquidacions")]
        public async Task<ActionResult> GetTipoLiquidacions()
        {
            try
            {
                var tipos = _tipoLiquidacionService.GetTipoLiquidacions();
                var Response = new ResponseDTO()
                {
                    Code = 1,//CODE 1 indica que el recibo se firmo
                    Data = tipos
                };
                return Ok(Response);

            }
            catch (EntityNotFoundException)
            {
                var Response = new ResponseDTO()
                {
                    Code = 1,//CODE 1 indica que el recibo se firmo
                    Data = new List<TipoLiquidacionDTO>()
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


        [HttpPost("AgregarTipoLiquidacion")]
        public async Task<ActionResult> AgregarTipoLiquidacion(TipoLiquidacionDTO TipoLiqui)
        {
            try
            {
                _tipoLiquidacionService.AgregarTipoLiquidacion(TipoLiqui);
                return Ok(new ResponseDTO { Code = 1, Message = "El cambio se ha realizado satisfactoriamente." });

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

        [HttpPost("EditarTipoLiquidacion")]
        public async Task<ActionResult> EditarTipoLiquidacion(TipoLiquidacionDTO TipoLiqui)
        {
            try
            {
                _tipoLiquidacionService.EditarTipoLiquidacion(TipoLiqui);
                return Ok(new ResponseDTO { Code = 1, Message = "El cambio se ha realizado satisfactoriamente." });
            }
            catch (EntityNotFoundException e)
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

        [HttpGet("EliminarTipoLiquidacion/{TipoLiquidacionId}")]
        public async Task<ActionResult> EliminarTipoLiquidacion(int TipoLiquidacionId)
        {
            try
            {
                _tipoLiquidacionService.EliminarTipoLiquidacion(TipoLiquidacionId);
                return Ok(new ResponseDTO { Code = 1, Message = "El cambio se ha realizado satisfactoriamente." });
            }
            catch (EntityNotFoundException e)
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
    }
}
