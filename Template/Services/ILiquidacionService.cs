using Template.DTOs;

namespace Template.Services
{
    public interface ILiquidacionService
    {
        public List<LiquidacionItemDTO> GetLiquidacionItems(string Cuil);
        public LiquidacionDTO GetLiquidacion(string Cuil, int LiquiId);
        public LiquidacionDTO GetLiquidaciones(string Cuil, int LiquiId);
        public bool setFirmar(string Cuil, int LiquiId);
        public void ImportarLiquidaciones(ImportarDTO datosImportar);
        public void ImportarLiquidacionesForce(ImportarDTO datosImportar);
        public List<DatosLiquiDTO> ObtenerLiquidaciones(string User);
        public List<DatosLiquiEmpleadoDTO> GetEmpleadosLiquidacion(UserLiquiDTO UserLiqui);
        public DatosLiquiEmpleadoDTO GetLiquidacionEspecifica(UserLiquiEmpleadoDTO Datos);
        public void EliminarLiquidacion(int LiquiId);
    }
}
