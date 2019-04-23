using System;

namespace EventBus.EventModels
{
    [Serializable] 
    public class Process
    {
        public Guid ProcessId;
        public EventActionType EventActionType;
        public string ServiceName;
        public DateTime CreationDate;        
        public string Data;
    }
}
