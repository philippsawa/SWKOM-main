using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public class QueueOptions
    {
        public const string Queue = "Queue";

        public string ConnectionString { get; set; } = string.Empty;
        public string OcrQueueName { get; set; } = string.Empty;   
        public string ResultQueueName { get; set; } = string.Empty;
    }
}
