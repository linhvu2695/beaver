#nullable disable

using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataService
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string EXCHANGE_NAME = "trigger";

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() {
                HostName = _configuration["RabbitMQHost"], 
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: EXCHANGE_NAME,
                    type: ExchangeType.Fanout
                );

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                System.Console.WriteLine("---> Connected to Message Bus");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"---> Fail to connect to Message Bus: {ex.Message}");
            }
        }
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                System.Console.WriteLine("---> RabbitMQ is open. Sending messages...");
            }
            else 
            {
                System.Console.WriteLine("---> RabbitMQ is closed. Message is not sent");
            }

            SendMessage(message);
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: EXCHANGE_NAME,
                routingKey: "",
                basicProperties: null,
                body: body
            );
            System.Console.WriteLine($"---> Message: \"{message}\" sent");
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            System.Console.WriteLine("---> MessageBus Disposed");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)

        {
            System.Console.WriteLine("---> RabbitMQ connection shut down");
        }
    }
}