using RabbitMQ.Client.Events;


namespace EventBus
{
    public class MessageEventArgs: BasicDeliverEventArgs, IMessageEventArgs
    {
        //public new byte[] Body { get; set; }

        public new byte[] Body
        {
            get { return base.Body; }
            set { base.Body = value; }
        }
    }
}



