using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus
{
    internal static class Retry
    {          
        internal static int GetRetryAttempts(IDictionary<string, object> Headers)
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

        internal static IDictionary<string, object> SetRetryAttempts(IDictionary<string, object> headers, int newAttempts)
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

        internal static IDictionary<string, object> CloneMessageHeaders(IDictionary<string, object> existingHeaders)
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

        internal static bool ShouldRejectMessage(BasicDeliverEventArgs deliveryArgs, int maxRetryAttempts)
        {
            int nextAttempt = GetRetryAttempts(deliveryArgs.BasicProperties.Headers) + 1;

            return nextAttempt > maxRetryAttempts;
        }
    }
}
