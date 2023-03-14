using Microsoft.AspNetCore.Mvc;
using Reporte.Sucursal.Services;

namespace Reporte.Sucursal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SucursalesController:ControllerBase
    {
        private readonly ISucursalesService _sucursalesService;

        public SucursalesController(ISucursalesService sucursalesService)
        {
            this._sucursalesService = sucursalesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSucursales()
        {
            var result = await _sucursalesService.GetAsync();
            return Ok(result);
        }

        [HttpGet("{sucursalId}")]
        public async Task<IActionResult> GetEmployee(int sucursalId)
        {
            var result = await _sucursalesService.GetAsync(sucursalId);
            return Ok(result);
        }
    }
}
