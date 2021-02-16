using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace api31
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection {get;set;}
        private IModel _channel {get;set;}

        private readonly RabbitMQService _rabbitMQService;
        private readonly ILogger<RabbitMQConsumerService> _logger;
        public RabbitMQConsumerService(ILogger<RabbitMQConsumerService> logger, RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
            _logger = logger;
            _logger.LogInformation("create factory");
            _factory = new ConnectionFactory() {HostName="rabbitmq-wjr", Port=5672, UserName="guest", Password="guest", VirtualHost="/"};
            _logger.LogInformation("created factory");
            Thread.Sleep(5000);
            _logger.LogInformation("create connection");
            _connection = _factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _logger.LogInformation("created connection");
            _logger.LogInformation("create channel");
            _channel = _connection.CreateModel();
            _logger.LogInformation("created channel");
            _channel.QueueDeclare(queue: "mq.teste", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("ConnectionShutdown rabbitmq");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
              
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var msgContent = JsonConvert.DeserializeObject<string>(content);

                _logger.LogInformation($"log info msg: {msgContent}, {DateTime.Now}");                            
                _rabbitMQService.PostMessage<string>("mq.teste2", $"{msgContent} - Passou aqui!");
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _channel.BasicConsume("mq.teste", false, consumer);

            return Task.CompletedTask;
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("OnConsumerCancelled");
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("OnConsumerUnregistered");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogInformation("OnConsumerRegistered");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("OnConsumerShutdown");
        }
    }

}