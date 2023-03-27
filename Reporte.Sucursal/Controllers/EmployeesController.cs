using Microsoft.AspNetCore.Mvc;
using Reporte.Sucursal.Services;

namespace Reporte.Sucursal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController:ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            this._employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await _employeeService.GetAsync();
            return Ok(result);
        }

        [HttpGet("{empId}")]
        public async Task<IActionResult> GetEmployee(string empId)
        {
            var result = await _employeeService.GetAsync(empId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}
