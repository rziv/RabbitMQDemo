using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.Retry
{
    public class Retry: IRetry
    {
       
        public void RepublishMessage(IModel channel, BasicDeliverEventArgs deliveryArgs, int interval)
        {

            int currentAttempt = GetRetryAttempts(deliveryArgs.BasicProperties.Headers) + 1;          

            //Ack original message
            channel.BasicAck(deliveryArgs.DeliveryTag, false);
            
                Task.Run(async delegate
                {
                    await Task.Delay(interval);
                    //Create retry message
                    var properties = channel.CreateBasicProperties();
                    properties.Headers = SetRetryAttempts(deliveryArgs.BasicProperties.Headers, currentAttempt);

                    //Publish retry message
                    channel.BasicPublish(deliveryArgs.Exchange, deliveryArgs.RoutingKey, properties, deliveryArgs.Body);
                    
                    Console.WriteLine("--- Process ({0} Subscriber) Error retry Attempt: {1}", deliveryArgs.Exchange, currentAttempt);
                });   
        }

        public void RejectMessage(IModel channel, BasicDeliverEventArgs deliveryArgs)
        {            
            // reject the message. If a dead letter queue was defined it will go to there (https://www.rabbitmq.com/dlx.html)
            channel.BasicReject(deliveryArgs.DeliveryTag, false);
        }

        public int GetRetryAttempts(IDictionary<string, object> Headers)
        {
            int RetryAttempts = 0;
            var RetryHeader = Configuration.Instance.RetryHeader;

            if (Headers != null && Headers.ContainsKey(RetryHeader))
            {
                var value = Headers[RetryHeader];
                RetryAttempts = (value != null) ? Convert.ToInt32(value) : 0;
            }

            return RetryAttempts;            
        }

        private IDictionary<string, object> SetRetryAttempts(IDictionary<string, object> headers, int newAttempts)
        {
            IDictionary<string, object> newHeaders = CloneMessageHeaders(headers);
            var retryHeader = Configuration.Instance.RetryHeader;
            if (newHeaders.ContainsKey(retryHeader))
            {
                newHeaders[retryHeader] = newAttempts;
            }
            else
            {
                newHeaders.Add(retryHeader, newAttempts);
            }

            return newHeaders;
        }        

        private IDictionary<string, object> CloneMessageHeaders(IDictionary<string, object> existingHeaders)
        {
            var newHeaders = new Dictionary<string, object>();
            if (existingHeaders != null)
            {
                var enumerator = existingHeaders.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    newHeaders.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }
            return newHeaders;
        }

        public bool ShouldRejectMessage(BasicDeliverEventArgs deliveryArgs, int maxRetryAttempts)
        {
            int nextAttempt = GetRetryAttempts(deliveryArgs.BasicProperties.Headers) + 1;

            return nextAttempt > maxRetryAttempts;
        }
    }
}
