#nullable disable

using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
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
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() {
                HostName = _configuration[Const.CONFIG_RABBITMQ_HOST], 
                Port = int.Parse(_configuration[Const.CONFIG_RABBITMQ_PORT])
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                    exchange: Const.EXCHANGE_NAME,
                    type: ExchangeType.Fanout
                );
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: Const.EXCHANGE_NAME, routingKey: "");

            System.Console.WriteLine("---> Listening on the Message Bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, eventArg) =>
            {
                System.Console.WriteLine("---> Event Received");

                var body = eventArg.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
            System.Console.WriteLine("---> MessageBus Disposed");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)

        {
            System.Console.WriteLine("---> RabbitMQ connection shut down");
        }
    }
}