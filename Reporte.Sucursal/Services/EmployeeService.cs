using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal.Services
{
    public class EmployeeService :IEmployeeService
    {
        private readonly HttpClient _httpClient;

        private readonly List<EmployeeDataTransferObject> _employees;
        public EmployeeService(List<EmployeeDataTransferObject> employees) {
            this._employees = employees;
            this._httpClient = new HttpClient();
        }
        public async Task<List<EmployeeDataTransferObject>> GetAsync() {
            return _employees;
        }

        public async Task<EmployeeDataTransferObject> GetAsync(string Id) {
            foreach (var e in _employees) {
                if (e.Id == Id) { 
                    return e;
                }
            }

            return null;
        }
    }
}
