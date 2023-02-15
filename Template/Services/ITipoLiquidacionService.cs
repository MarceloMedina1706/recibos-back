using Template.DTOs;

namespace Template.Services
{
    public interface ITipoLiquidacionService
    {
        public List<TipoLiquidacionDTO> GetTipoLiquidacions();
        public string GetDescripcionByLiquiId(int liquiId);
        public void AgregarTipoLiquidacion(TipoLiquidacionDTO TipoLiqui);
        public void EditarTipoLiquidacion(TipoLiquidacionDTO TipoLiqui);
        public void EliminarTipoLiquidacion(int TipoLiquidacionId);

    }
}
