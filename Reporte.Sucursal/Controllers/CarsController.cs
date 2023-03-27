using Microsoft.AspNetCore.Mvc;
using Reporte.Sucursal.Dtos;
using Reporte.Sucursal.Services;

namespace Reporte.Sucursal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController:ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars() {
            var result = await _carService.GetAsync();

            return Ok(result);
        }

        [HttpGet("{carId}")]
        public async Task<IActionResult> GetCar(string carId)
        {
            var result = await _carService.GetAsync(carId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
