#nullable disable

using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataService
{
    public class MessageBusClient : IMessageBusClient
    {
        # region Const
        public class Const
        {
            public const string EXCHANGE_NAME = "trigger";
            public const string CONFIG_RABBITMQ_HOST = "RabbitMQHost";
            public const string CONFIG_RABBITMQ_PORT = "RabbitMQPort";
        }
        #endregion

        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() {
                HostName = _configuration[Const.CONFIG_RABBITMQ_HOST], 
                Port = int.Parse(_configuration[Const.CONFIG_RABBITMQ_PORT])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(
                    exchange: Const.EXCHANGE_NAME,
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
        public void PublishPlatform(PlatformPublishedDto platformPublishedDto)
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
                exchange: Const.EXCHANGE_NAME,
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