using EventBus;
using EventBus.EventModels;
using EventBus.Retry;
using EventBus.API;
using RabbitMQ.Client;
using System;

namespace Statistics.Subscriber
{
    class Program
    {
      
        private static IConnection _connection;
        private const int maxAttempts = 3;
        private const int interval = 3000;

        static void Main(string[] args)
        {
            var QueueName = Configuration.Instance.ProcessStatisticsQueueName;
            RetryPolicy retryPolicy = new RetryPolicy(maxAttempts, interval);
            Retry retry = new Retry();
           
            using (_connection = Connection.GetConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(QueueName, false, consumer);

                    while (true)
                    {
                        var eventMessage = consumer.Queue.Dequeue();
                        try
                        {
                            var process = (Process)eventMessage.Body.DeSerialize(typeof(Process));
                            if (String.IsNullOrEmpty(process.Data))
                            {
                                throw new Exception("Missing process Data");
                            }
                            var routingKey = eventMessage.RoutingKey;
                            Console.WriteLine("--- Process (Statistics Subscriber) - Routing Key <{0}> : {1} : {2}", routingKey, process.ServiceName, process.Data);
                            channel.BasicAck(eventMessage.DeliveryTag, false);
                        }
                        catch (Exception)
                        {
                            if (retry.ShouldRejectMessage(eventMessage, retryPolicy.maxAttempts)) {
                                retry.RejectMessage(channel, eventMessage);
                            }
                            else
                            {
                                retry.RepublishMessage(channel, eventMessage, retryPolicy.interval);
                            }

                        }
                    }
                }
            }
        }       
    }
}

