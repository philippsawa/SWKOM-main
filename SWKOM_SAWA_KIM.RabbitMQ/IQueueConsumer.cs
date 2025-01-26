using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public interface IQueueConsumer
    {
        event EventHandler<QueueReceivedEventArgs> OnReceived;
        void StartReceive();
        void StopReceive();
    }
}
