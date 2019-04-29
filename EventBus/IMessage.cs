using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IMessage
    {
        void ApproveMessage(MessageEventArgs messageEventArgs);
        void RejectMessage(MessageEventArgs messageEventArgs);
    }
}
