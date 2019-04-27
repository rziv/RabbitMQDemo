using RabbitMQ.Client;

namespace EventBus
{
    internal static class Connection
    {
        internal static IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = Configuration.Instance.HostName,
                UserName = Configuration.Instance.UserName,
                Password = Configuration.Instance.Password
            };
            return factory.CreateConnection();
        }
    }
}
