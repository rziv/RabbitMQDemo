using EventBus;

namespace EventBus
{
    public interface IRetry
    {
        void Retry(MessageEventArgs messageEventArgs, RetryPolicy policy);        
    }
}
