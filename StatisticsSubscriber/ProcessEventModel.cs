using System;
using EventBus;

namespace StatisticsSubscriber
{
    [Serializable]
    internal class ProcessEventModel: IEventModel
    {
        internal string ServiceName;
        public Guid CorrelationId { get; set ; }
        public EventActionType EventActionType { get; set; }
        public DateTime CreationDate { get; set; }
        public string Data { get; set; }
    }
}
