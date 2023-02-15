using Template.DTOs;
using Template.Entities;
using Template.Exceptions;
using Template.Models;
using Template.Utils;

namespace Template.Services
{
    public class LiquidacionService : ILiquidacionService
    {
        private readonly ITipoLiquidacionService _tipoLiquidacionService;

        public LiquidacionService(ITipoLiquidacionService tipoLiquidacionService)
        {
            _tipoLiquidacionService = tipoLiquidacionService;
        }

        public List<LiquidacionItemDTO> GetLiquidacionItems(string EmpleadoId)
        {
            using(var context = new LiquidacionContext())
            {
                var liquidaciones = context.Liquidacions.Where(l => l.EmpleadoId == EmpleadoId)
                    .OrderByDescending(l => l.LiquiId)
                    .Select(s => new LiquidacionItemDTO
                    {
                        Liqui_Id = s.LiquiId,
                        Mes = getMesLiqui(s.LiquiId),
                        Anio = getAnioLiqui(s.LiquiId),
                        Descripcion = s.Descripcion ?? "",
                        Firmado = s.Firmado.Year != 1900,
                        Visto = s.Visto
                    })
                    .ToList();

                if (!liquidaciones.Any()) throw new LiquidacionNotFoundException("No se ha encontrado ninguna liquidación.");
                else return liquidaciones;
            }
        }

        public LiquidacionDTO GetLiquidacion(string Cuil, int LiquiId)
        {
            using(var context = new LiquidacionContext())
            {
                var recibosPrevios = context.Liquidacions.Any(l => l.LiquiId < LiquiId
                                                                    && l.EmpleadoId == Cuil
                                                                    && l.Firmado.Year == 1900);

                if (recibosPrevios) throw new RecibosPreviosException();

                var datos = (from empleado in context.Empleados
                             join empresa in context.Empresas
                             on empleado.EmpresaId equals empresa.Id
                             join liqui in context.Liquidacions
                             on empleado.Id equals liqui.EmpleadoId
                             where empleado.Id == Cuil && liqui.LiquiId == LiquiId
                             select new
                             {
                                 Cuil = empleado.Id,
                                 Cuit = empresa.Id,
                                 Empresa = empresa.RazonSocial,
                                 Nombre = empleado.Nombre,
                                 Apellido = empleado.Apellido,
                                 Ingreso = empleado.Ingreso,
                                 //Categoria = emp.Categoria,
                                 Firma = empleado.Firma,
                                 Numero = liqui.ReciboNro,
                                 Banco = liqui.Banco,
                                 UltimoDeposito = liqui.FecUltDeposito,
                                 Firmado = liqui.Firmado.Year != 1900,
                                 Categoria = liqui.Categoria,
                                 RemBasica = liqui.SueldoBasico,
                                 TotHaberes = liqui.TotalHaberes,
                                 TotDeducciones = liqui.TotalDeducciones,
                                 TotNeto = liqui.TotalNeto,
                                 Visto = liqui.Visto ?? false,
                                 FechaFirmado = liqui.Firmado,
                             }).FirstOrDefault();

                if(datos != null)
                {
                    var codigos = (from cod in context.LiquiCodLiquidados
                                   where cod.LiquiId == LiquiId && cod.EmpleadoId == Cuil
                                   select new CodigoLiquidacionDTO
                                   {
                                       Codigo = cod.Codigo,
                                       Descripcion = cod.CodDescripcion,
                                       Cantidad = cod.Cantidad.ToString(),
                                       Importe = cod.Importe.ToString(),
                                       CodTipo = cod.CodTipo
                                   }).ToList();

                    string periodoLiquidado = getMesLiqui(LiquiId) + "/" + getAnioLiqui(LiquiId);
                    LiquidacionDTO liquidacion = new LiquidacionDTO()
                    {
                        Empresa = datos.Empresa,
                        LiquiNumero = datos.Numero,
                        Cuit = datos.Cuit,
                        UltimoDeposito = datos.UltimoDeposito.ToString(),
                        Banco = datos.Banco,
                        Cuil = datos.Cuil,
                        Beneficiario = datos.Apellido + " " + datos.Nombre,
                        Ingreso = FormatoFecha(datos.Ingreso.ToString()),
                        Categoria = datos.Categoria,
                        RemBasica = datos.RemBasica.ToString(),
                        PeriodoLiquidado = periodoLiquidado,
                        Codigos = codigos,
                        TotalHaberes = datos.TotHaberes.ToString(),
                        TotalDeducciones = datos.TotDeducciones.ToString(),
                        TotalNeto = datos.TotNeto.ToString(),
                        TotalNetoEnPalabras = NumeroTexto.NumeroALetras(datos.TotNeto),
                        Firmado = datos.Firmado,
                        Firma = datos.Firmado ? datos.Firma : null,
                        FechaFirmado = datos.Firmado ? FormatoFecha(datos.FechaFirmado.ToString()) : null
                    };

                    var result = new ResultGetLiquidacion();
                    if (!datos.Visto) {
                        result.Code = 2; // INDICA QUE HAY QUE ACTUALIZAR A VISTO
                        setVisto(Cuil, LiquiId, context);
                    }
                    else
                    {
                        result.Code = 1;
                    }
                    result.Liquidacion = liquidacion;
                    return liquidacion;

                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public LiquidacionDTO GetLiquidaciones(string Cuil, int LiquiId)
        {
            using (var context = new LiquidacionContext())
            {

                var datos = (from empleado in context.Empleados
                             join empresa in context.Empresas
                             on empleado.EmpresaId equals empresa.Id
                             join liqui in context.Liquidacions
                             on empleado.Id equals liqui.EmpleadoId
                             where empleado.Id == Cuil && liqui.LiquiId == LiquiId
                             select new
                             {
                                 Cuil = empleado.Id,
                                 Cuit = empresa.Id,
                                 Empresa = empresa.RazonSocial,
                                 Nombre = empleado.Nombre,
                                 Apellido = empleado.Apellido,
                                 Ingreso = empleado.Ingreso,
                                 //Categoria = emp.Categoria,
                                 Firma = empleado.Firma,
                                 Numero = liqui.ReciboNro,
                                 Banco = liqui.Banco,
                                 UltimoDeposito = liqui.FecUltDeposito,
                                 Firmado = liqui.Firmado.Year != 1900,
                                 Categoria = liqui.Categoria,
                                 RemBasica = liqui.SueldoBasico,
                                 TotHaberes = liqui.TotalHaberes,
                                 TotDeducciones = liqui.TotalDeducciones,
                                 TotNeto = liqui.TotalNeto,
                                 Visto = liqui.Visto ?? false,
                                 FechaFirmado = liqui.Firmado,
                             }).FirstOrDefault();

                if (datos != null)
                {
                    var codigos = (from cod in context.LiquiCodLiquidados
                                   where cod.LiquiId == LiquiId && cod.EmpleadoId == Cuil
                                   select new CodigoLiquidacionDTO
                                   {
                                       Codigo = cod.Codigo,
                                       Descripcion = cod.CodDescripcion,
                                       Cantidad = cod.Cantidad.ToString(),
                                       Importe = cod.Importe.ToString(),
                                       CodTipo = cod.CodTipo
                                   }).ToList();

                    string periodoLiquidado = getMesLiqui(LiquiId) + "/" + getAnioLiqui(LiquiId);
                    LiquidacionDTO liquidacion = new LiquidacionDTO()
                    {
                        Empresa = datos.Empresa,
                        LiquiNumero = datos.Numero,
                        Cuit = datos.Cuit,
                        UltimoDeposito = datos.UltimoDeposito.ToString(),
                        Banco = datos.Banco,
                        Cuil = datos.Cuil,
                        Beneficiario = datos.Apellido + " " + datos.Nombre,
                        Ingreso = FormatoFecha(datos.Ingreso.ToString()),
                        Categoria = datos.Categoria,
                        RemBasica = datos.RemBasica.ToString(),
                        PeriodoLiquidado = periodoLiquidado,
                        Codigos = codigos,
                        TotalHaberes = datos.TotHaberes.ToString(),
                        TotalDeducciones = datos.TotDeducciones.ToString(),
                        TotalNeto = datos.TotNeto.ToString(),
                        TotalNetoEnPalabras = NumeroTexto.NumeroALetras(datos.TotNeto),
                        Firmado = datos.Firmado,
                        Firma = datos.Firmado ? datos.Firma : null,
                        FechaFirmado = datos.Firmado ? FormatoFecha(datos.FechaFirmado.ToString()) : null
                    };

                    var result = new ResultGetLiquidacion();
                    if (!datos.Visto)
                    {
                        result.Code = 2; // INDICA QUE HAY QUE ACTUALIZAR A VISTO
                        setVisto(Cuil, LiquiId, context);
                    }
                    else
                    {
                        result.Code = 1;
                    }
                    result.Liquidacion = liquidacion;
                    return liquidacion;

                }
                else
                {
                    throw new Exception();
                }
            }
        }
        public bool setFirmar(string Cuil, int LiquiId)
        {
            using (var context = new LiquidacionContext())
            {
                var liquidacion = context.Liquidacions.FirstOrDefault(l => l.EmpleadoId == Cuil && l.LiquiId == LiquiId);
                if (liquidacion != null)
                {
                    liquidacion.Firmado = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
                
        }

        public void ImportarLiquidaciones(ImportarDTO datosImportar)
        {
            var empleados = GetEmpleadosId(datosImportar.Liquis);
            VerificarLiquidacion(datosImportar.LiquiId, empleados);
            LiquidacionContext dbContext = new LiquidacionContext();
            int contLiquis = 0, contCodigos = 0;
            var liquis = datosImportar.Liquis;
            var tipoLiqui = _tipoLiquidacionService.GetDescripcionByLiquiId(datosImportar.LiquiId);
            foreach (var liqui in liquis)
            {
                var liquidacion = new Liquidacion
                {
                    LiquiId = datosImportar.LiquiId,
                    EmpleadoId = liqui.EmpleadoId,
                    Descripcion = tipoLiqui ?? liqui.Descripcion,
                    Categoria = liqui.Categoria,
                    SueldoBasico = liqui.SueldoBasico,
                    Banco = liqui.Banco,
                    FecUltDeposito = GetDateTime(liqui.FecUltDep),
                    TotalHaberes = liqui.TotHaberes,
                    TotalDeducciones = liqui.TotDeducciones,
                    TotalNeto = liqui.TotNeto,
                    Firmado = GetDateTime("1900-01-01 00:00:00"),
                    Visto = false,
                    Dvh = 1234
                };

                contLiquis++;
                dbContext = AddLiquidacionToContext(dbContext, liquidacion, contLiquis, 100, true);

            }

            dbContext.SaveChanges();

            foreach (var liqui in liquis)
            {

                liqui.Codigos.ForEach(codigo =>
                {
                    var cod = new LiquiCodLiquidado
                    {
                        LiquiId = datosImportar.LiquiId,
                        EmpleadoId = liqui.EmpleadoId,
                        Codigo = codigo.Codigo,
                        Cantidad = codigo.Cantidad,
                        Porcentaje = codigo.Porcentaje,
                        Importe = codigo.Importe,
                        CodDescripcion = codigo.CodDescripcion,
                        CodTipo = codigo.CodTipo,
                        Dvh = 1234
                    };
                    contCodigos++;
                    dbContext = AddCodigosToContext(dbContext, cod, contCodigos, liqui.Codigos.Count, true);

                });

            }

            dbContext.SaveChanges();



        }

        public void ImportarLiquidacionesForce(ImportarDTO datosImportar)
        {
            var Liqui = GetLiquiId();
            LiquidacionContext dbContext = new LiquidacionContext();
            int contLiquis = 0, contCodigos = 0;
            var liquis = datosImportar.Liquis;
            foreach (var liqui in liquis)
            {
                var liquidacion = new Liquidacion
                {
                    LiquiId = Liqui,
                    EmpleadoId = liqui.EmpleadoId,
                    Descripcion = datosImportar.TipoLiquidacion ?? liqui.Descripcion,
                    Categoria = liqui.Categoria,
                    SueldoBasico = liqui.SueldoBasico,
                    Banco = liqui.Banco,
                    FecUltDeposito = GetDateTime(liqui.FecUltDep),
                    TotalHaberes = liqui.TotHaberes,
                    TotalDeducciones = liqui.TotDeducciones,
                    TotalNeto = liqui.TotNeto,
                    Firmado = GetDateTime("1900-01-01 00:00:00"),
                    Visto = false,
                    Dvh = 1234
                };

                contLiquis++;
                dbContext = AddLiquidacionToContext(dbContext, liquidacion, contLiquis, 100, true);

            }

            dbContext.SaveChanges();

            foreach (var liqui in liquis)
            {

                liqui.Codigos.ForEach(codigo =>
                {
                    var cod = new LiquiCodLiquidado
                    {
                        LiquiId = Liqui,
                        EmpleadoId = liqui.EmpleadoId,
                        Codigo = codigo.Codigo,
                        Cantidad = codigo.Cantidad,
                        Porcentaje = codigo.Porcentaje,
                        Importe = codigo.Importe,
                        CodDescripcion = codigo.CodDescripcion,
                        CodTipo = codigo.CodTipo,
                        Dvh = 1234
                    };
                    contCodigos++;
                    dbContext = AddCodigosToContext(dbContext, cod, contCodigos, liqui.Codigos.Count, true);

                });

            }

            dbContext.SaveChanges();



        }

        public List<DatosLiquiDTO> ObtenerLiquidaciones(string User)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var Liquis = dbContext.Liquidacions.GroupBy(l => l.LiquiId)
                                .Select(li => new DatosLiquiDTO
                                {
                                    LiquiId = li.First().LiquiId,
                                    Periodo = li.First().LiquiId.ToString().Substring(0, 4) + "/" + li.First().LiquiId.ToString().Substring(4, 2),
                                    Descripcion = li.First().Descripcion ?? "",
                                    CantidadEmpleados = li.Count(),
                                    SumaTotalHaberes = li.Sum(h => h.TotalHaberes),
                                    SumaTotalDeducciones = li.Sum(d => d.TotalDeducciones),
                                    SumaTotalNetos = li.Sum(n => n.TotalNeto)
                                }).OrderByDescending(liq => liq.LiquiId).ToList();

                if (!Liquis.Any()) return new List<DatosLiquiDTO>();

                return Liquis;
            }
        }

        public List<DatosLiquiEmpleadoDTO> GetEmpleadosLiquidacion(UserLiquiDTO UserLiqui)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var Empleados = dbContext.Liquidacions
                                .Where(l => l.LiquiId == UserLiqui.LiquiId)
                                .Select(li => new DatosLiquiEmpleadoDTO
                                {
                                    EmpleadoId = li.EmpleadoId,
                                    TotalHaberes = li.TotalHaberes.ToString(),
                                    TotalDeducciones = li.TotalDeducciones.ToString(),
                                    TotalNeto = li.TotalNeto.ToString(),
                                }).ToList();

                if (!Empleados.Any())
                {
                    throw new EmpleadoNotFoundException("No se encontraron empleados.");
                }

                Empleados.ForEach(emp =>
                {
                    emp.CantCodigos = (from cod in dbContext.LiquiCodLiquidados
                                       where cod.LiquiId == UserLiqui.LiquiId && cod.EmpleadoId == emp.EmpleadoId
                                       select cod).Count();
                });

                return Empleados;
            }

        }

        public DatosLiquiEmpleadoDTO GetLiquidacionEspecifica(UserLiquiEmpleadoDTO Datos)
        {
            using (var dbContext = new LiquidacionContext())
            {
                var Liqui = dbContext.Liquidacions
                                .Where(l => l.LiquiId == Datos.LiquiId && l.EmpleadoId == Datos.EmpleadoId)
                                .Select(li => new DatosLiquiEmpleadoDTO
                                {
                                    EmpleadoId = li.EmpleadoId,
                                    TotalHaberes = li.TotalHaberes.ToString(),
                                    TotalDeducciones = li.TotalDeducciones.ToString(),
                                    TotalNeto = li.TotalNeto.ToString(),
                                    Categoria = li.Categoria,
                                    Descripcion = li.Descripcion,
                                    SueldoBasico = li.SueldoBasico.ToString(),
                                    FecUltDep = li.FecUltDeposito,
                                    Banco = li.Banco
                                }).FirstOrDefault();

                if (Liqui == null) throw new LiquidacionNotFoundException("No se encontraron liquidaciones.");

                Liqui.CodigoLiquidaciones = dbContext.LiquiCodLiquidados
                     .Where(l => l.LiquiId == Datos.LiquiId && l.EmpleadoId == Datos.EmpleadoId)
                     .Select(cod => new CodigoLiquidacionDTO
                     {
                         Codigo = cod.Codigo,
                         Descripcion = cod.CodDescripcion,
                         Cantidad = cod.Cantidad.ToString(),
                         Importe = cod.Importe.ToString(),
                         Porcentaje = cod.Porcentaje.ToString(),
                         CodTipo = cod.CodTipo.ToString(),
                     })
                     .ToList();

                return Liqui;
            }

        }

        public void EliminarLiquidacion(int LiquiId)
        {
            using (var dbcontext = new LiquidacionContext())
            {
                var Codigos = dbcontext.LiquiCodLiquidados.Where(cod => cod.LiquiId == LiquiId).ToList();

                if (Codigos.Any())
                {
                    Codigos.ForEach(codigo => dbcontext.LiquiCodLiquidados.Remove(codigo));

                    var Liquis = dbcontext.Liquidacions.Where(liq => liq.LiquiId == LiquiId).ToList();

                    Liquis.ForEach(Liqui => dbcontext.Liquidacions.Remove(Liqui));

                    dbcontext.SaveChanges();

                }

            }
        }

        private static void setVisto(string Cuil, int LiquiId, LiquidacionContext context)
        {
            var liquidacion = context.Liquidacions.FirstOrDefault(l => l.EmpleadoId == Cuil && l.LiquiId == LiquiId);
            if(liquidacion != null){
                liquidacion.Visto = true;
                context.SaveChanges();
            }
        }


        private static LiquidacionContext AddCodigosToContext(LiquidacionContext context,
                    LiquiCodLiquidado entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<LiquiCodLiquidado>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new LiquidacionContext();
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }


        private static LiquidacionContext AddLiquidacionToContext(LiquidacionContext context,
                    Liquidacion entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<Liquidacion>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new LiquidacionContext();
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        private static void VerificarLiquidacion(int liquiId, List<string> empleados)
        {
            var complementario = liquiId.ToString().EndsWith("03");
            using(var context = new LiquidacionContext())
            {
                empleados.ForEach(e =>
                {
                    if (context.Liquidacions.Any(l => l.LiquiId == liquiId && l.EmpleadoId == e))
                    {
                        if(complementario)
                            throw new LiquidacionAlreadyLoadedException("La liquidación que intenta importar ya ha sido cargada a la base de datos. ¿Desea cargar un nuevo tipo de liquidación?");
                        else
                            throw new LiquidacionAlreadyLoadedException("La liquidación que intenta importar ya ha sido cargada a la base de datos.");
                    }
                });
            }
        }
        private static int GetLiquiId()
        {
            using(var context = new LiquidacionContext())
            {
                var maxLiquiId = context.Liquidacions.Max(l => l.LiquiId);
                return maxLiquiId + 1;
            }
        }
        
        private static string getAnioLiqui(int LiquiId)
        {
            var aux = LiquiId.ToString();
            var anio = aux.Substring(0, 4);
            return anio;
        }
        private static string getMesLiqui(int LiquiId)
        {
            var aux = LiquiId.ToString();
            var mes = aux.Substring(4, 2);
            return mes;
        }
        private string FormatoFecha(string fecha)
        {
            var dia = fecha.Split(" ")[0].Split("/")[0];
            var mes = fecha.Split(" ")[0].Split("/")[1];
            var anio = fecha.Split(" ")[0].Split("/")[2];

            if (dia.Length == 1)
            {
                dia = "0" + dia;
            }

            if (mes.Length == 1)
            {
                mes = "0" + mes;
            }
            return dia + "/" + mes + "/" + anio;
        }
        private static DateTime GetDateTime(string date)
        {

            var anio = Int32.Parse(date.Split(" ")[0].Split("-")[0]);
            var mes = Int32.Parse(date.Split(" ")[0].Split("-")[1]);
            var dia = Int32.Parse(date.Split(" ")[0].Split("-")[2]);

            var hora = Int32.Parse(date.Split(" ")[1].Split(":")[0]);
            var minuto = Int32.Parse(date.Split(" ")[1].Split(":")[1]);
            var segundo = Int32.Parse(date.Split(" ")[1].Split(":")[2]);

            return new DateTime(anio, mes, dia, hora, minuto, segundo);
        }
        private static List<string> GetEmpleadosId(DatosLiquidacionDTO[] datos)
        {
            var empleados = new List<string>();
            
            for(int i=0; i<datos.Length; i++)
            {
                empleados.Add(datos[i].EmpleadoId);
            }

            return empleados;

        }

        
    }
}
