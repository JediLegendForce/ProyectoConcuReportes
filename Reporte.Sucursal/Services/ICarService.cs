using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal.Services
{
    public interface ICarService
    {
        Task<List<CarDataTransferObject>> GetAsync();

        Task<CarDataTransferObject> GetAsync(string id);
    }
}
