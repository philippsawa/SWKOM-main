using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.Minio
{
    public class MinioService : IMinioService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;
        private readonly ILogger<MinioService> _logger;

        public MinioService(IOptions<MinioOptions> options, ILogger<MinioService> logger)
        {
            _logger = logger;
            var minioOptions = options.Value;

            _minioClient = new MinioClient()
                .WithEndpoint(minioOptions.Endpoint)
                .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
                .WithSSL(false)
                .Build();

            _logger.LogInformation($"MinioService: Minio client is created.");

            _bucketName = minioOptions.BucketName;
        }

        public async Task EnsureBucketExistsAsync()
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));

            if (!found)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
            }
        }

        public async Task UploadDocumentAsync(string id, Stream fileStream)
        {
            try
            {
                await EnsureBucketExistsAsync();

                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(id)
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length));

                _logger.LogInformation($"MinioService: Document {id} is uploaded.");
            }
            catch (Exception ex)
            {
                // log exception tbd
                _logger.LogError($"MinioService: Document {id} upload failed.");
                throw;
            }
        }

        public async Task<Stream> DownloadDocumentAsync(string id)
        {
            try
            {
                var memoryStream = new MemoryStream();

                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(id)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyToAsync(memoryStream);
                    }));

                memoryStream.Position = 0;
                _logger.LogInformation($"MinioService: Document {id} is downloaded.");
                return memoryStream;
            }
            catch (Exception ex)
            {
                // log exception tbd
                _logger.LogError($"MinioService: Document {id} download failed.");
                throw;
            }
        }
    }
}
