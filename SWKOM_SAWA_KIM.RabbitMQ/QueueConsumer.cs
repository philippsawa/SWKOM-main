using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public class QueueConsumer : RabbitMQService, IQueueConsumer
    {
        private EventingBasicConsumer _consumer;
        private readonly ILogger<QueueConsumer> _logger;

        public QueueConsumer(IOptions<QueueOptions> options, ILogger<QueueConsumer> logger) : base(options.Value.ConnectionString, options.Value.OcrQueueName, logger)
        {
            _consumer = new EventingBasicConsumer(_channel);
            _logger = logger;

            _logger.LogInformation($"QueueConsumer: {QueueName} is created.");
        }

        public event EventHandler<QueueReceivedEventArgs> OnReceived;

        public void StartReceive()
        {
            _logger.LogInformation($"QueueConsumer: Start receiving from {QueueName} queue.");

            _consumer.Received += (ch, ea) =>
            {
                try
                {
                    // received message  
                    var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                    var id = ea.BasicProperties.CorrelationId;

                    // handle the received message  
                    HandleMessage(content, id);
                    _channel.BasicAck(ea.DeliveryTag, false);

                    _logger.LogInformation($"QueueConsumer: Processed message from {QueueName} queue.");
                }
                catch (Exception ex)
                {
                    // Message processing failed, retry a certain number of times with delays
                    int retries = 3;
                    int delayMilliseconds = 1000;

                    if (ea.BasicProperties.Headers == null)
                        ea.BasicProperties.Headers = new Dictionary<string, object>();

                    if (!ea.BasicProperties.Headers.ContainsKey("retry-count"))
                        ea.BasicProperties.Headers.Add("retry-count", 0);

                    int retryCount = (int)ea.BasicProperties.Headers["retry-count"];

                    if (retryCount < retries)
                    {
                        // Increase retry count and delay message requeue
                        ea.BasicProperties.Headers["retry-count"] = retryCount + 1;
                        ea.BasicProperties.Expiration = (delayMilliseconds * (retryCount + 1)).ToString();

                        _channel.BasicReject(ea.DeliveryTag, requeue: true);
                    }
                    else
                    {
                        // Message reached maximum retries, send it to the Dead-Letter Exchange
                        _channel.BasicReject(ea.DeliveryTag, requeue: false);
                    }
                }
            };

            _consumer.Shutdown += OnConsumerShutdown;
            _consumer.Registered += OnConsumerRegistered;
            _consumer.Unregistered += OnConsumerUnregistered;
            _consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: _consumer);
        }

        public void StopReceive()
        {
            foreach (var tag in _consumer.ConsumerTags)
            {
                _channel.BasicCancel(tag);
            }
            _channel.Close();
            _channel.Dispose();
            _channel.Close();
            _channel.Dispose();
        }

        private void HandleMessage(string content, string id)
        {
            if (this.OnReceived != null)
            {
                this.OnReceived(this, new QueueReceivedEventArgs(content, id));
            }
        }


        private void OnConsumerConsumerCancelled(object? sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object? sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object? sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutdown(object? sender, ShutdownEventArgs e)
        {
        }
    }
}
