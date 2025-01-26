using SWKOM_SAWA_KIM.BLL.Services;
using SWKOM_SAWA_KIM.RabbitMQ;

namespace SWKOM_SAWA_KIM.Workers
{
    public class ResultQueueWorker : BackgroundService
    {
        private readonly IQueueConsumer _queueConsumer;
        //private readonly IDocumentService _documentService;
        //private readonly IDocumentLogic _documentLogic;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ResultQueueWorker> _logger;

        //public ResultQueueWorker(IQueueConsumer queueConsumer, IDocumentService documentService, IDocumentLogic documentLogic, IServiceScopeFactory serviceScopeFactory)
        public ResultQueueWorker(IQueueConsumer queueConsumer, IServiceScopeFactory serviceScopeFactory, ILogger<ResultQueueWorker> logger)
        {
            Console.WriteLine("RESULT QUEUE CONSTRUCTOR");

            _queueConsumer = queueConsumer;
            //_documentService = documentService;
            //_documentLogic = documentLogic;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("STARTING TO LISTEN EXECUTE ASYNC");

            _queueConsumer.OnReceived += async (sender, e) =>
            {
                Console.WriteLine("ON RECEIVED EVENT");

                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogInformation("Received message");

                using var scope = _serviceScopeFactory.CreateScope();
                var documentService = scope.ServiceProvider.GetRequiredService<IDocumentService>();

                var ocrResult = e.Content;
                var id = e.MessageId;

                await ProcessResult(ocrResult, id, documentService);
                _logger.LogInformation("Processed message");
            };

            Console.WriteLine("Starting to receive messages...");
            _queueConsumer.StartReceive();
            return Task.CompletedTask;
        }

        private async Task ProcessResult(string ocrResult, string id, IDocumentService documentService)
        {
            try
            {
                Console.WriteLine("Processing result...");
                Console.WriteLine(ocrResult);
                await documentService.UpdateDocumentAsync(id, ocrResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the result");
                // log exception
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _queueConsumer.StopReceive();
            return base.StopAsync(cancellationToken);
        }
    }
}
