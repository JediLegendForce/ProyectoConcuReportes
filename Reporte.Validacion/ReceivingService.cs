using System.Text;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Reporte.Validacion;

public class ReceivingService : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;

    public ReceivingService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        };
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
            var message = JsonConvert.DeserializeObject<String>(json);
            Console.WriteLine(message);
        };
        _channel.BasicConsume("validacion_queue", true, _consumer);
        return Task.CompletedTask;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Mensajeria en linea {DateTimeOffset.Now}");
            await Task.Delay(1000, stoppingToken);
        }
    }
}