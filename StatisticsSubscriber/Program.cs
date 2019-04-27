using EventBus;
using EventBus.EventModels;
using System;

namespace Statistics.Subscriber
{
    class Program
    {
        private const int maxAttempts = 3;
        private const int interval = 3000;

        static void Main(string[] args)
        {
            string QueueName = Configuration.Instance.ProcessStatisticsQueueName;
            RetryPolicy retryPolicy = new RetryPolicy(maxAttempts, interval);
            Broker broker = new Broker(QueueName);

            void HandleProcessMessage(object sender, MessageEventArgs messageEventArgs)
            {
                try
                {
                    var process = (Process)messageEventArgs.Body.DeSerialize(typeof(Process));
                    if (String.IsNullOrEmpty(process.Data))
                    {
                        throw new Exception("Missing process Data");
                    }
                   
                    Console.WriteLine("--- Process (Statistics Subscriber) - {0} : {1}", process.ServiceName, process.Data);
                    broker.Approve(messageEventArgs);
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



