using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using EventBus.RabbitMQ;

namespace Setup
{

    //TODO: All this setup shoold be moved to script and run once as part of the CI-CD
    //Implementation - powershell script that is executed with a bat file.
    class Program
    {
        static void Main(string[] args)
        {           
                var ExchangeName = Configuration.Instance.ProcessExchangeName;
                var ProcessFormsManagerQueueName = Configuration.Instance.ProcessFormsManagerQueueName;
                var ProcessStatisticsQueueName = Configuration.Instance.ProcessStatisticsQueueName;
                IConnection _connection = Connection.GetConnection();
                IModel _model = _connection.CreateModel();
                _model.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);
                _model.ExchangeDeclare(Configuration.Instance.DeadLetterExchangeName, ExchangeType.Fanout, true);
                IDictionary<string, object> arguments = new Dictionary<string, object>();
                arguments.Add("x-dead-letter-exchange", Configuration.Instance.DeadLetterExchangeName);
                _model.QueueDeclare(ProcessStatisticsQueueName, true, false, false, arguments);
                _model.QueueDeclare(ProcessFormsManagerQueueName, true, false, false, null);
                _model.QueueDeclare(Configuration.Instance.DeadLetterQueueName, true, false, false, null);
                _model.QueueBind(ProcessFormsManagerQueueName, ExchangeName, "FormsManager");
                _model.QueueBind(ProcessStatisticsQueueName, ExchangeName, "Statistics");
                _model.QueueBind(Configuration.Instance.DeadLetterQueueName, Configuration.Instance.DeadLetterExchangeName, "DeadLetter");
           
        }
    }
}
