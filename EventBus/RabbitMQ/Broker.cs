﻿using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.RabbitMQ
{

    public class Broker: IEventBus, IRabbitMQ
    {
        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer consumer;   

        private EventHandler<MessageEventArgs> MessageEventhandler;
        
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }

        public Broker(string ExchangeName) : this(ExchangeName, String.Empty)
        {           
        }

        public Broker(string ExchangeName, string QueueName)
        {
            this.connection = Connection.GetConnection();
            this.channel = connection.CreateModel();
            this.consumer = new EventingBasicConsumer(channel);
            this.ExchangeName = ExchangeName;
            this.QueueName = QueueName;

            this.consumer.Received += RegisterMessageEvent;
        }
        
        public void Subscribe(EventHandler<MessageEventArgs> subscriber)
        {
            MessageEventhandler += subscriber;
            this.channel.BasicConsume(this.QueueName, false, this.consumer);
        }

        public void ApproveMessage(MessageEventArgs messageEventArgs)
        {
            this.channel.BasicAck(messageEventArgs.DeliveryTag, false);
        }

        

        private void RepublishMessage(MessageEventArgs messageEventArgs, int interval)
        {
            BasicDeliverEventArgs deliveryArgs = (BasicDeliverEventArgs)messageEventArgs;
            int currentAttempt = EventBus.RabbitMQ.Retry.GetRetryAttempts(deliveryArgs.BasicProperties.Headers) + 1;

            //Ack original message
            channel.BasicAck(deliveryArgs.DeliveryTag, false);

            Task.Run(async delegate
            {
                await Task.Delay(interval);
                //Create retry message
                var properties = channel.CreateBasicProperties();
                properties.Headers = EventBus.RabbitMQ.Retry.SetRetryAttempts(deliveryArgs.BasicProperties.Headers, currentAttempt);

                //Publish retry message
                channel.BasicPublish(this.ExchangeName, deliveryArgs.RoutingKey, properties, deliveryArgs.Body);

                Console.WriteLine("--- Process ({0} Subscriber) Error retry Attempt: {1}", deliveryArgs.Exchange, currentAttempt);
            });
        }

        public void RejectMessage(MessageEventArgs messageEventArgs)
        {
            BasicDeliverEventArgs deliveryArgs = (BasicDeliverEventArgs)messageEventArgs;
            // reject the message. If a dead letter queue was defined it will go to there (https://www.rabbitmq.com/dlx.html)
            channel.BasicReject(deliveryArgs.DeliveryTag, false);
        }  
        
        public void Publish(byte[] message, string routingKey)
        {
            string routing = String.IsNullOrEmpty(routingKey) ? this.ExchangeName : routingKey;
            this.channel.BasicPublish(this.ExchangeName, routing, null, message);
        }

        private void OnRaiseMessageEvent(MessageEventArgs e)
        {
            MessageEventhandler?.Invoke(this, e);           
        }

        private void RegisterMessageEvent(object sender, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            MessageEventArgs args = new MessageEventArgs();
            args.Body = basicDeliverEventArgs.Body;
            args.BasicProperties = basicDeliverEventArgs.BasicProperties;
            args.DeliveryTag = basicDeliverEventArgs.DeliveryTag;
            args.ConsumerTag = basicDeliverEventArgs.ConsumerTag;
            args.RoutingKey = basicDeliverEventArgs.RoutingKey;
            OnRaiseMessageEvent(args);
        }      

        

        public void Retry(MessageEventArgs messageEventArgs, RetryPolicy policy)
        {
            if (EventBus.RabbitMQ.Retry.ShouldRejectMessage(messageEventArgs, policy.maxAttempts))
            {
                RejectMessage(messageEventArgs);
            }
            else
            {
                RepublishMessage(messageEventArgs, policy.interval);
            }
        }  
    }           
}


