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
    private readonly string baseUrl = $"https://localhost:7100/";

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
        _channel.QueueDeclare("val-queue", false, false, false, null);
        _consumer = new EventingBasicConsumer(_channel);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        //ExecuteAsync(cancellationToken);
        _consumer.Received += async (model, content) =>
        {
            List<string> errors = new List<string>();
            var body = content.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var _registers = JsonConvert.DeserializeObject<InfoBlocksDataTransferObject>(json);
            var cars_res = await this._httpClient.GetStringAsync(baseUrl + "cars");
            var cars = JsonConvert.DeserializeObject<List<CarDataTransferObject>>(cars_res);
            var employees_res = await this._httpClient.GetStringAsync(baseUrl + "employees");
            var employees = JsonConvert.DeserializeObject<List<EmployeeDataTransferObject>>(employees_res);
            var sucursales_res = await this._httpClient.GetStringAsync(baseUrl + "sucursales");
            var sucursales = JsonConvert.DeserializeObject<List<SucursalDataTransferObject>>(sucursales_res);

            var reg_index = _registers.offset + 2;
            var found = 0;
            var employee404 = "";
            var car404 = "";


            if(_registers.transaction != null)
            {
                foreach (var item in _registers.registros)
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
                        employee404 = item.username;
                        errors.Add($"Employee {employee404} at line {reg_index} not found on database");
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
                        car404 = item.car_id;
                        errors.Add($"Car with id {car404} at line {reg_index} not found on database");
                    }
                    if (item.vin.Length != 17)
                    {
                        errors.Add($"VIN {item.vin} at line {reg_index} is not valid");
                    }
                    if (item.buyer_first_name == null || item.buyer_first_name.Equals("")) 
                    {
                        errors.Add($"Buyer first name at line {reg_index} is empty");
                    }
                    if (item.buyer_last_name == null || item.buyer_last_name.Equals(""))
                    {
                        errors.Add($"Buyer last name at line {reg_index} is empty");
                    }
                    if (item.buyer_id == null || item.buyer_id.Equals(""))
                    {
                        errors.Add($"Buyer last name at line {reg_index} is empty");
                    }
                    reg_index++;
                }
                _registers.transaction.errors = errors;

                SendResponse(_registers.transaction);
            }
        };
        _channel.BasicConsume("val-queue", true, _consumer);

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
                    channel.QueueDeclare("res-queue", false, false, false, null);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(string.Empty, "res-queue", null, body);
                }
            }
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
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