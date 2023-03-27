using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reporte.Recolector.Dtos;
using System.Text;
using System.Transactions;


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
            //ExecuteAsync(cancellationToken);
            _consumer.Received += async (model, content) =>
            {
                var body = content.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var transaction = JsonConvert.DeserializeObject<TransactionDTO>(json);
                await SendCSVData(transaction);
            };
            _channel.BasicConsume("rec-queue", true, _consumer);
            return Task.CompletedTask;
        }

        public async Task SendAsync(string msg)
        {

            var body = Encoding.UTF8.GetBytes(msg);
            _channel.BasicPublish(string.Empty, "val-queue", null, body);

        }

        private async Task SendCSVData(TransactionDTO transactionMeta)
        {
            var locked = false;
            var files = Directory.GetFiles(".\\Sales", "*.csv", SearchOption.TopDirectoryOnly);
            List<SaleDTO> data = new List<SaleDTO>();

            Parallel.ForEach(files, file =>
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        string[] line = sr.ReadLine().Split(',');

                        SaleDTO temp = new SaleDTO()
                        {
                            username = line[0],
                            car_id = line[1],
                            price = line[2] + "," + line[3],
                            vin = line[4],
                            buyer_first_name = line[5],
                            buyer_last_name = line[6],
                            buyer_id = line[7],
                            division_id = line[8]
                        };

                        while (locked) { };
                        locked = true;
                        data.Add(temp);
                        locked = false;
                    }



                    //var testDeserialization = JsonConvert.DeserializeObject<List<SaleDTO>>(testSerialization);
                }
            });

            List<SaleDTO> package = new List<SaleDTO>();

            int counter = 0;
            int offset = 0;

            for (int i = 0; i < data.Count; i++)
            {
                package.Add(data[i]);
                if (package.Count == 50 || i == data.Count - 1)
                {
                    ValidacionDTO mensaje = new ValidacionDTO();
                    mensaje.registros = package;
                    mensaje.transaction = transactionMeta;
                    mensaje.offset = offset;
                    var serializedPackage = JsonConvert.SerializeObject(mensaje);

                    await SendAsync(serializedPackage);
                    offset += 50;
                    counter += package.Count;
                    package = new List<SaleDTO>();
                    
                }

            }

            Console.WriteLine(counter.ToString());

            foreach (var file in new DirectoryInfo(".\\Sales").GetFiles())
            {
                file.Delete();
            }

        }
    }
}


