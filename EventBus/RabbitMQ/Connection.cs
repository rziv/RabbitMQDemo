using RabbitMQ.Client;

namespace EventBus.RabbitMQ
{
    public static class Connection
    {
        public static IConnection GetConnection()
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
