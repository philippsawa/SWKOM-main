using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.RabbitMQ
{
    public interface IQueueProducer
    {
        void SendToTaskQueue(string message, string id);
        void SendToResultQueue(string message, string id);
    }
}
