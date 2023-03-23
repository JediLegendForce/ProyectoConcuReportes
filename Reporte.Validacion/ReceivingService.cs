using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reporte.Sucursal.Dtos;
using Reporte.Validacion.Dtos;

namespace Reporte.Validacion;

public class ReceivingService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;
    private readonly HttpClient _httpClient;
    private readonly string baseUrl = $"localhost:7100/events";

    public ReceivingService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };
        _httpClient = new HttpClient();
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare("val_queue", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Received += async (model, content) =>
        {
            List<string> errors = new List<string>();
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var _registers = JsonConvert.DeserializeObject<InfoBlocksDataTransferObject>(json);
            var cars_res = await this._httpClient.GetStringAsync(baseUrl + "/cars");
            var cars = JsonConvert.DeserializeObject<List<CarDataTransferObject>>(cars_res);
            var employees_res = await this._httpClient.GetStringAsync(baseUrl + "/employees");
            var employees = JsonConvert.DeserializeObject<List<EmployeeDataTransferObject>>(employees_res);
            var sucursales_res = await this._httpClient.GetStringAsync(baseUrl + "/sucursales");
            var sucursales = JsonConvert.DeserializeObject<List<SucursalDataTransferObject>>(sucursales_res);

            var reg_index = 2;
            var found = 0;
            foreach (var item in _registers.Information)
            {
                foreach (var employee in employees)
                {
                    if (employee.username == item.username)
                    {
                        found = 1;
                    }
                    // Default Not Found
                }
                if (found == 0)
                {
                    errors.Add($"Employee at line {reg_index} not found on database");
                }
                found = 0;
                foreach (var car in cars)
                {
                    if (car.Id == item.car_id)
                    {
                        found = 1;
                    }  
                    // Default Not Found
                }
                if (found == 0)
                {
                    errors.Add($"Car at line {reg_index} not found on database");
                }
                if (item.vin.Length != 17)
                {
                    errors.Add($"VIN at line {reg_index} is not valid");
                }
                if (item.buyer_FName == null || item.buyer_FName.Equals("")) {
                    errors.Add($"Buyer first name at line {reg_index} is empty");
                }
                if (item.buyer_LName == null || item.buyer_LName.Equals(""))
                {
                    errors.Add($"Buyer last name at line {reg_index} is empty");
                }
                
            }
        };
        _channel.BasicConsume("val_queue", true, _consumer);
        return Task.CompletedTask;
    }

    private void SendResponse(TransactionDataTransferObject transaction)
    {
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
                    channel.QueueDeclare("res_queue", false, false, false, null);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(string.Empty, "res_queue", null, body);
                }
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Validacion en linea {DateTimeOffset.Now}");
            await Task.Delay(1000, stoppingToken);
        }
    }
}