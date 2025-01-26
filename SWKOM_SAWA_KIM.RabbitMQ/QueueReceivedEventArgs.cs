using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public class QueueReceivedEventArgs
    {
        public QueueReceivedEventArgs(string content, string id)
        {
            Content = content;
            MessageId = id;
        }

        public string Content { get; }
        public string MessageId { get; }
    }
}
