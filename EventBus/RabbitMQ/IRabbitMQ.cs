using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public interface IRabbitMQ
    {
        string ExchangeName { get; set; }
        string QueueName { get; set; }        
    }
}
