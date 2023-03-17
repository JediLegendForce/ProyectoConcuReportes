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
        _channel.QueueDeclare("validacion_queue", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Received += async (model, content) =>
        {
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var _registers = JsonConvert.DeserializeObject<InfoBlocksDataTransferObject>(json);
            var cars_res = await this._httpClient.GetStringAsync(baseUrl + "/cars");
            var cars = JsonConvert.DeserializeObject<List<CarDataTransferObject>>(cars_res);
            var employees_res = await this._httpClient.GetStringAsync(baseUrl + "/employees");
            var employees = JsonConvert.DeserializeObject<List<EmployeeDataTransferObject>>(employees_res);
            var sucursales_res = await this._httpClient.GetStringAsync(baseUrl + "/sucursales");
            var sucursales = JsonConvert.DeserializeObject<List<SucursalDataTransferObject>>(sucursales_res);

            var reg_index = 0
            var found = 0;
            foreach (var list in _registers.Information)
            {
                foreach (var item in list)
                {
                    foreach (var employee in employees)
                    {
                        if (employee.username == item.username)
                        {
                            // employee found
                        }
                        // Default Not Found
                    }
                    foreach (var car in cars)
                    {
                        if (car.Id == item.car_id)
                        {
                            // car_id found
                        }  
                        // Default Not Found
                    }
                    if (item.vin.Length != 17)
                    {
                        // VIN Not Valid
                    }
                    if (item.buyer_FName == null || item.buyer_FName.Equals("")) {
                        // Buyer First Name is Null or Empty
                    }
                    if (item.buyer_LName == null || item.buyer_LName.Equals(""))
                    {
                        // Buyer Last Name is Null or Empty
                    }
                }
            }
        };
        _channel.BasicConsume("validacion_queue", true, _consumer);
        return Task.CompletedTask;
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