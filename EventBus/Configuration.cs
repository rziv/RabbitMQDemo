using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    // Singletone with simple thread-safety
    public class Configuration
    {

        public string ExchangeName = "DirectRouting_Exchange";
        public string DeadLetterExchangeName = "DirectRouting_DeadLetterExchange";
        public string DeadLetterQueueName = "DeadLetter_Queue";
        public string ProcessFormsManagerQueueName = "ProcessFormsManager_Queue";
        public string ProcessStatisticsQueueName = "ProcessStatistics_Queue";
        public string HostName = "localhost";
        public string UserName = "guest";
        public string Password = "guest";
        public string RetryHeader = "RETRY-COUNT";

        private static Configuration instance = null;
        private static readonly object padlock = new object();

        private Configuration()
        {
           
        }

        public static Configuration Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Configuration();
                    }
                    return instance;
                }
            }
        }
    }
}

