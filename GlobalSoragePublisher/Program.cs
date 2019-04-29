using System;
using EventBus;
using EventBus.Serialization;
using EventBus.EventModels;
using EventBus.RabbitMQ;
using System.Collections.Generic;

namespace GlobalStorage
{
    class Program
    {
        static void Main()
        {   
            var process1 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName="dr1@moin.gov.il", Data = "{\"firstName\": \"Avi\", \"lastName\":\"Cohen\"}" };
            var process2 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "GeneralRequest@rbc.gov.il", Data = "{\"country\": \"Israel\", \"RequestType\":\"Marriage\"}" };
            var process3 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "dr1@moin.gov.il", Data = "{\"firstName\": \"Eli\", \"lastName\":\"Gabay\"}" };
            var process4 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "GeneralRequest@rbc.gov.il", Data = String.Empty };
            var process5 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "GeneralRequest@rbc.gov.il", Data = "{\"country\": \"England\", \"RequestType\":\"Marriage\"}" };

            string ExchangeName = Configuration.Instance.ProcessExchangeName;
            IEventBus broker = new Broker(ExchangeName);

            SendProcess(process1, broker);
            SendProcess(process2, broker);
            SendProcess(process3, broker);
            SendProcess(process4, broker);
            SendProcess(process5, broker);
        }

        private static void SendProcess(Process process, IEventBus broker)
        {
            SendMessage(process.Serialize(),Configuration.Instance.ProcessFormsManagerRoutingName, broker);
            SendMessage(process.Serialize(), Configuration.Instance.ProcessStatisticsRoutingName, broker);
            Console.WriteLine(" Process Sent {0}, {1}", process.ServiceName, process.Data); 
        }

        private static void SendMessage(byte[] message, string routingKey, IEventBus broker)
        {
            broker.Publish(message, routingKey);                     
        }
    } 
}
