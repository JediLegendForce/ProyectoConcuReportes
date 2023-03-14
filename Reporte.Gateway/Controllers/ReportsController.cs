using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Reporte.Gateway.Dtos;
using System.Text;

namespace Reporte.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var transaction = new TransactionDTO
            {
                Id = Guid.NewGuid(),
                errors = new List<string>()
            };

            /*
            var transaction = new TransactionDTO
            {
                Id = Guid.NewGuid(),
                errors = new List<string>
                {
                    "ERROR",
                    "ERROR 2",
                    "ERROR 3"
                }
            };*/

            try
            {
                var json = JsonConvert.SerializeObject(transaction);
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare("rec-queue", false, false, false, null);
                        var body = Encoding.UTF8.GetBytes(json);
                        channel.BasicPublish(string.Empty, "rec-queue", null, body);

                    }
                }


                return Ok(transaction.Id);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}
