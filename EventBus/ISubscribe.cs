using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public interface ISubscribe
    {
        void Subscribe(EventHandler<MessageEventArgs> subscriber);
    }
}
