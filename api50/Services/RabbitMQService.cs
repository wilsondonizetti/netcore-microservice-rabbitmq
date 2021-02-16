using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace api50
{
    public class RabbitMQService
    {
        private readonly ILogger<RabbitMQService> _logger;
        public RabbitMQService(ILogger<RabbitMQService> logger)
        {
            _logger = logger;
        }        

        public void PostMessage<T>(T msg) where T:class 
        {
            if(msg == null)
                throw new ArgumentNullException("Value null");

            var factory = new ConnectionFactory() {HostName="rabbitmq-wjr", Port=5672, UserName="guest", Password="guest", VirtualHost="/"};
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                var json = JsonConvert.SerializeObject(msg);
                var body = Encoding.UTF8.GetBytes(json);

                _logger.LogInformation($"post message {DateTime.Now}");
                channel.BasicPublish("", "mq.teste", basicProperties: null, body: body);
                _logger.LogInformation($"message posted {DateTime.Now}");
                _logger.LogTrace("teste trace");
            }
        }
    }
    
}