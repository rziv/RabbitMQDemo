using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.Retry
{
    interface IRetry
    {
        void RepublishMessage(IModel channel, BasicDeliverEventArgs deliveryArgs, int interval);
        void RejectMessage(IModel channel, BasicDeliverEventArgs deliveryArgs);
        bool ShouldRejectMessage(BasicDeliverEventArgs deliveryArgs, int maxRetryAttempts);

    }
}
