using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SWKOM_SAWA_KIM.ElasticSearch;
using SWKOM_SAWA_KIM.Minio;
using SWKOM_SAWA_KIM.RabbitMQ;

namespace SWKOM_SAWA_KIM.OCR
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IOcrClient, OcrClient>();

                    services.Configure<QueueOptions>(context.Configuration.GetSection("QueueOptions"));
                    services.Configure<OcrOptions>(context.Configuration.GetSection("OcrOptions"));
                    services.AddSingleton<IQueueConsumer, QueueConsumer>();
                    services.AddSingleton<IQueueProducer, QueueProducer>();

                    services.Configure<MinioOptions>(context.Configuration.GetSection("MinioOptions"));
                    services.AddSingleton<IMinioService, MinioService>();

                    services.AddSingleton<ISearchIndex, SearchIndex>();

                    services.AddHostedService<OcrWorker>();
                })
                .Build();

            Console.WriteLine("OCR service is running...");
            await builder.RunAsync();
        }
    }
}
