using EventBus;
using EventBus.RabbitMQ;
using EventBus.Serialization;
using System;

namespace StatisticsSubscriber
{
    class Program
    {
        private const int maxAttempts = 3;
        private const int interval = 3000;

        static void Main(string[] args)
        {
            string ExchangeName = Configuration.Instance.ProcessExchangeName;
            string QueueName = Configuration.Instance.ProcessStatisticsQueueName;
            RetryPolicy retryPolicy = new RetryPolicy(maxAttempts, interval);
            IEventBus broker = new Broker(ExchangeName, QueueName);

            void HandleProcessMessage(object sender, MessageEventArgs messageEventArgs)
            {
                try
                {
                    var process = (ProcessEventModel)messageEventArgs.Body.DeSerialize(typeof(ProcessEventModel));
                    if (String.IsNullOrEmpty(process.Data))
                    {
                        throw new Exception("Missing process Data");
                    }
                   
                    Console.WriteLine("--- Process (Statistics Subscriber) - {0} : {1}", process.ServiceName, process.Data);
                    broker.ApproveMessage(messageEventArgs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    broker.Retry(messageEventArgs, retryPolicy);
                }
            }

            broker.Subscribe(HandleProcessMessage);
            Console.ReadLine();
        }

    }
}



