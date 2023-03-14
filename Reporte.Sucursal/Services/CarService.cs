
using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal.Services
{
    public class CarService:ICarService
    {
        private readonly HttpClient _httpClient;

        private readonly List<CarDataTransferObject> _cars;

        public CarService(List<CarDataTransferObject> cars) {
            this._cars = cars;
            _httpClient = new HttpClient();
        }
        public async Task<List<CarDataTransferObject>> GetAsync() {
            return _cars;
        }

        public async Task<CarDataTransferObject> GetAsync(string Id)
        {
            foreach (var car in _cars) {
                if (car.Id == Id) {
                    return car;
                }
            }

            return null;
        }
    }
}
