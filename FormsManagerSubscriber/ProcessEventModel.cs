using System;
using EventBus;

namespace FormsManager
{
    [Serializable]
    internal class ProcessEventModel: IEventModel
    {       
        public string ServiceName { get; set; }
        public Guid CorrelationId { get; set ; }
        public EventActionType EventActionType { get; set; }
        public DateTime CreationDate { get; set; }
        public string Data { get; set; }
    }
}
