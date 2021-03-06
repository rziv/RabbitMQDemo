﻿using EventBus;
using EventBus.Serialization;
using EventBus.RabbitMQ;
using RabbitMQ.Client;
using System;

namespace FormsManager
{
    class Program
    {       
        private const int maxAttempts = 4;
        private const int interval = 4000;

        static void Main(string[] args)
        {
            string ExchangeName = Configuration.Instance.ProcessExchangeName;
            var QueueName = Configuration.Instance.ProcessFormsManagerQueueName;
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
