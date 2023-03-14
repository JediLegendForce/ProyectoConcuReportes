using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal.Services
{
    public class SucursalesService:ISucursalesService
    {
        private readonly HttpClient _httpClient;

        private readonly List<SucursalDataTransferObject> _sucursales;

        public SucursalesService(List<SucursalDataTransferObject> sucursales)
        {
            _sucursales = sucursales;
        }

        public async Task<List<SucursalDataTransferObject>> GetAsync() {
            return _sucursales;
        }

        public async Task<SucursalDataTransferObject> GetAsync(int Id) {
            foreach (var s in _sucursales) {
                if (s.Id == Id) {
                    return s;
                }
            }

            return null;
        }
    }
}
