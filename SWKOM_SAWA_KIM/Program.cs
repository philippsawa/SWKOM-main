using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SWKOM_SAWA_KIM.BLL;
using SWKOM_SAWA_KIM.BLL.Mappings;
using SWKOM_SAWA_KIM.BLL.Services;
using SWKOM_SAWA_KIM.BLL.Validators;
using SWKOM_SAWA_KIM.DAL;
using SWKOM_SAWA_KIM.DAL.Data;
using SWKOM_SAWA_KIM.DAL.Repositories;
using SWKOM_SAWA_KIM.ElasticSearch;
using SWKOM_SAWA_KIM.Minio;
using SWKOM_SAWA_KIM.RabbitMQ;
using SWKOM_SAWA_KIM.Workers;
using System.Diagnostics;

namespace SWKOM_SAWA_KIM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // from docker compose file
            builder.Configuration.AddEnvironmentVariables();

            var connectionString = builder.Configuration.GetConnectionString("dbConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");
            }

            builder.Logging.AddDebug();
            builder.Logging.AddConsole();

            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddDataAccessLayer(builder.Configuration);
            builder.Services.AddBusinessLogicLayer(builder.Configuration);
            
            builder.Services.Configure<QueueOptions>(builder.Configuration.GetSection("QueueOptions"));
            builder.Services.AddSingleton<IQueueProducer, QueueProducer>();
            builder.Services.AddSingleton<IQueueConsumer, ResultQueueConsumer>();

            builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("MinioOptions"));
            builder.Services.AddSingleton<IMinioService, MinioService>();

            builder.Services.AddHostedService<ResultQueueWorker>();

            builder.Services.AddSingleton<ISearchIndex, SearchIndex>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost")  
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                //dbContext.Database.Migrate(); 
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
