using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public class QueueProducer : RabbitMQService, IQueueProducer
    {
        private readonly string _resultQueueName;
        private readonly ILogger<QueueProducer> _logger;

        public QueueProducer(IOptions<QueueOptions> options, ILogger<QueueProducer> logger) : base(options.Value.ConnectionString, options.Value.OcrQueueName, logger)
        {
            _resultQueueName = options.Value.ResultQueueName;
            _logger = logger;

            _channel.QueueDeclare(
                queue: _resultQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation($"QueueProducer: {_resultQueueName} is created.");
        }

        public void SendToTaskQueue(string message, string id)
        {
            IBasicProperties properties = base._channel.CreateBasicProperties();
            properties.CorrelationId = id;

            base._channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: $"{QueueName}.*",
                mandatory: false,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(message)
            );

            _logger.LogInformation($"QueueProducer: Sent to {QueueName} queue.");
        }

        public void SendToResultQueue(string message, string id)
        {
            IBasicProperties properties = base._channel.CreateBasicProperties();
            properties.CorrelationId = id;

            base._channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: $"{_resultQueueName}.processed",
                mandatory: false,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(message)
            );

            _logger.LogInformation($"QueueProducer: Sent to {_resultQueueName} queue.");
        }
    }
}
