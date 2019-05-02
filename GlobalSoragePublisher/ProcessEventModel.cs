using System;
using EventBus;

namespace GlobalStorage
{
    [Serializable]
    class ProcessEventModel: IEventModel
    {       
        public string ServiceName;
        public Guid CorrelationId { get; set ; }
        public EventActionType EventActionType { get; set; }
        public DateTime CreationDate { get; set; }
        public string Data { get; set; }
    }
}
