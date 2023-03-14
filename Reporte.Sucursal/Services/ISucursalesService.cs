using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal.Services
{
    public interface ISucursalesService
    {
        Task<List<SucursalDataTransferObject>> GetAsync();
        Task<SucursalDataTransferObject> GetAsync(int Id);
    }
}
