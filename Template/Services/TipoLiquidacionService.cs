using Template.DTOs;
using Template.Entities;
using Template.Exceptions;

namespace Template.Services
{
    public class TipoLiquidacionService : ITipoLiquidacionService
    {
        public List<TipoLiquidacionDTO> GetTipoLiquidacions()
        {
           using(var context = new LiquidacionContext())
            {
                var tLiquidacion = context.TipoLiquidacions.Select(t => new TipoLiquidacionDTO
                {
                    Id = t.Id,
                    Descripcion = t.Descripcion,
                }).ToList();

                if (tLiquidacion != null) return tLiquidacion;
                else throw new EntityNotFoundException("No se han encontrado tipos de liquidaciones.");
            }
        }

        public string GetDescripcionByLiquiId(int liquiId)
        {
            using (var context = new LiquidacionContext())
            {
                var tipoId = Int16.Parse(liquiId.ToString().Substring(6));
                var LiquiDescripcion = context.TipoLiquidacions.Where(tl => tl.Id == tipoId).Select(tl => tl.Descripcion).FirstOrDefault();

                if (LiquiDescripcion != null) return LiquiDescripcion;
                else throw new EntityNotFoundException("No se han encontrado tipos de liquidaciones.");
            }
        }

        public void AgregarTipoLiquidacion(TipoLiquidacionDTO TipoLiqui)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var OTipoLiquidacion = new TipoLiquidacion()
                {
                    Descripcion = TipoLiqui.Descripcion
                };

                dbContext.Add(OTipoLiquidacion);
                dbContext.SaveChanges();
            }
        }

        public void EditarTipoLiquidacion(TipoLiquidacionDTO TipoLiqui)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var OTipoLiquidacion = dbContext.TipoLiquidacions.Where(t => t.Id == TipoLiqui.Id).FirstOrDefault();
                if (OTipoLiquidacion != null)
                {
                    OTipoLiquidacion.Descripcion = TipoLiqui.Descripcion;
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new EntityNotFoundException("No se ha encontrado el tipo de liquidación.");
                }
                
            }
        }

        public void EliminarTipoLiquidacion(int TipoLiquidacionId)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var OTipoLiquidacion = dbContext.TipoLiquidacions.Where(t => t.Id == TipoLiquidacionId).FirstOrDefault();
                if (OTipoLiquidacion != null)
                {
                    dbContext.TipoLiquidacions.Remove(OTipoLiquidacion);
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new EntityNotFoundException("No se ha encontrado el tipo de liquidación.");
                }
                
            }
        }
    }
}
