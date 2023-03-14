using System.Globalization;
using Reporte.Sucursal.Dtos;

namespace Reporte.Sucursal.Services
{
    public interface IEmployeeService
    {
        //IEmployeeService EmployeeService(List<EmployeeDataTransferObject> employees) ;
        Task<List<EmployeeDataTransferObject>> GetAsync();

        Task<EmployeeDataTransferObject> GetAsync(string Id);
    }
}
