using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reporte.Recolector.Dtos;
using System.Text;


namespace Reporte.Recolector
{
    public class RecolectorBackgroundService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        

        public RecolectorBackgroundService()
        {

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("rec-queue", false, false, false, null);
            _consumer = new EventingBasicConsumer(_channel);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Recolector en proceso de receptor {DateTimeOffset.Now}");
                await Task.Delay(3000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Received += async (model, content) =>
            {
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var transaction = JsonConvert.DeserializeObject<TransactionDTO>(json);
                Console.WriteLine(transaction.Id);
                foreach (var item in transaction.errors)
                {
                    Console.WriteLine(item);
                }

                
            };
            _channel.BasicConsume("rec-queue", true, _consumer);
            return Task.CompletedTask;
        }
    }

}


