using System;
using RabbitMQ.Client;
using EventBus;
using EventBus.EventModels;
using System.Collections.Generic;

namespace GlobalStorage
{
    class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;      


        static void Main()
        {   
            var process1 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName="dr1@moin.gov.il", Data = "{\"firstName\": \"Avi\", \"lastName\":\"Cohen\"}" };
            var process2 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "GeneralRequest@rbc.gov.il", Data = "{\"country\": \"Israel\", \"RequestType\":\"Marriage\"}" };
            var process3 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "dr1@moin.gov.il", Data = "{\"firstName\": \"Eli\", \"lastName\":\"Gabay\"}" };
            var process4 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "GeneralRequest@rbc.gov.il", Data = String.Empty };
            var process5 = new Process { ProcessId = new Guid(), CreationDate = DateTime.Now, EventActionType = EventActionType.Created, ServiceName = "GeneralRequest@rbc.gov.il", Data = "{\"country\": \"England\", \"RequestType\":\"Marriage\"}" };


            CreateConnection();

            SendProcess(process1);
            SendProcess(process2);
            SendProcess(process3);
            SendProcess(process4);
            SendProcess(process5);
        }

        private static void SendProcess(Process process)
        {
            SendMessage(process.Serialize(), "FormsManager");
            SendMessage(process.Serialize(), "Statistics");
            Console.WriteLine(" Process Sent {0}, {1}", process.ServiceName, process.Data); 
        }        

        private static void CreateConnection()
        {
        //TODO: All this setup shoold be moved to script and run once as part of the CI-CD
        //Implementation - powershell script that is executed with a bat file.
         var ExchangeName = Configuration.Instance.ExchangeName;
         var ProcessFormsManagerQueueName = Configuration.Instance.ProcessFormsManagerQueueName;
         var ProcessStatisticsQueueName = Configuration.Instance.ProcessStatisticsQueueName;
        _factory = new ConnectionFactory { HostName = Configuration.Instance.HostName, UserName = Configuration.Instance.UserName, Password = Configuration.Instance.Password };
        _connection = _factory.CreateConnection();
        _model = _connection.CreateModel();
        _model.ExchangeDeclare(ExchangeName, "direct",true);
        _model.ExchangeDeclare(Configuration.Instance.DeadLetterExchangeName, "fanout", true);
        IDictionary<string, object> args = new Dictionary<string, object>();
        args.Add("x-dead-letter-exchange", Configuration.Instance.DeadLetterExchangeName);
        _model.QueueDeclare(ProcessStatisticsQueueName, true, false, false, args);
        _model.QueueDeclare(ProcessFormsManagerQueueName, true, false, false, null);
        _model.QueueDeclare(Configuration.Instance.DeadLetterQueueName, true, false, false, null);
        _model.QueueBind(ProcessFormsManagerQueueName, ExchangeName, "FormsManager");
        _model.QueueBind(ProcessStatisticsQueueName, ExchangeName, "Statistics");
        _model.QueueBind(Configuration.Instance.DeadLetterQueueName, Configuration.Instance.DeadLetterExchangeName, "DeadLetter");
        }

        private static void SendMessage(byte[] message, string routingKey)
        {
            var ExchangeName = Configuration.Instance.ExchangeName;
            _model.BasicPublish(ExchangeName, routingKey, null, message);          
        }
    }
}
