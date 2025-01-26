using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.Minio
{
    public interface IMinioService
    {
        Task EnsureBucketExistsAsync();
        Task UploadDocumentAsync(string id, Stream fileStream);
        Task<Stream> DownloadDocumentAsync(string id);
    }
}
