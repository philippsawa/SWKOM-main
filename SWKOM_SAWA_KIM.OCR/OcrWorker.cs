using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SWKOM_SAWA_KIM.ElasticSearch;
using SWKOM_SAWA_KIM.Minio;
using SWKOM_SAWA_KIM.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.OCR
{
    public class OcrWorker : BackgroundService
    {
        private readonly IQueueConsumer _queueConsumer;
        private readonly IQueueProducer _queueProducer;
        private readonly IOcrClient _ocrClient;
        private readonly IMinioService _minioService;
        private readonly ISearchIndex _searchIndex;
        private readonly ILogger<OcrWorker> _logger;

        public OcrWorker(IQueueConsumer queueConsumer, IQueueProducer queueProducer, IOcrClient ocrClient, IMinioService minioService, ISearchIndex searchIndex, ILogger<OcrWorker> logger)
        {
            _queueConsumer = queueConsumer;
            _queueProducer = queueProducer;
            _ocrClient = ocrClient;
            _minioService = minioService;
            _searchIndex = searchIndex;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueConsumer.OnReceived += async (sender, e) =>
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogInformation("Received message");

                var content = e.Content;
                var id = e.MessageId;

                try
                {
                    await ProcessOcr(content, id);
                }
                catch(Exception ex)
                {
                    // log exception
                }
            };

            Console.WriteLine("BEFORE START RECEIVE");
            _queueConsumer.StartReceive();
            Console.WriteLine("AFTER START RECEIVE");

            return Task.CompletedTask;
        }

        private async Task ProcessOcr(string filepath, string id)
        {
            try
            {
                using var documentStream = await _minioService.DownloadDocumentAsync(id);

                if (documentStream == null)
                {
                    return;
                }

                var result = _ocrClient.OcrPdf(documentStream);

                Console.WriteLine($"BEFORE SEND TO RESULT QUEUE");
                _queueProducer.SendToResultQueue(result, id);
                Console.WriteLine($"AFTER SEND TO RESULT QUEUE: {result}");

                var document = new SearchDocumentEntity
                {
                    Id = id,
                    Content = result,
                    Filename = filepath
                };

                Console.WriteLine($"BEFORE ADD DOCUMENT TO SEARCH INDEX: {filepath}");
                _searchIndex.AddDocumentAsync(document);
                Console.WriteLine($"AFTER ADD DOCUMENT TO SEARCH INDEX: {document.Id}");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
