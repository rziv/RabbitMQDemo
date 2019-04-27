using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus
{
    interface IRetry
    {
        void Retry(MessageEventArgs messageEventArgs, RetryPolicy policy);
        void RepublishMessage(IModel channel, BasicDeliverEventArgs deliveryArgs, int interval);        
    }
}
