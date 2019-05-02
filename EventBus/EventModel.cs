using System;

namespace EventBus
{   
    public interface IEventModel
    {
        Guid CorrelationId { get; set; }
        EventActionType EventActionType { get; set; }
        DateTime CreationDate { get; set; }
        string Data { get; set; }
    }
}
