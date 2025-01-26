using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public abstract class RabbitMQService : IDisposable
    {
        protected IConnection _connection;
        protected IModel _channel;
        private readonly string _url;
        protected string QueueName { get; set; }
        protected string ExchangeName { get; set; }
        private bool disposedValue;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(string url, string queueName, ILogger<RabbitMQService> logger)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));
            }

            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentException($"'{nameof(queueName)}' cannot be null or empty.", nameof(queueName));
            }

            this._url = url;

            this.QueueName = queueName;
            this.ExchangeName = $"swkom.exchange";
            this._logger = logger;

            _logger.LogInformation($"RabbitMQService: {queueName} is created.");
            InitFactory();
        }

        private void InitFactory()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_url)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: $"{QueueName}.*",
                arguments: null
            );

            _channel.BasicQos(prefetchSize: 0,
                              prefetchCount: 1,
                              global: false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._connection.Dispose();
                    this._channel.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
