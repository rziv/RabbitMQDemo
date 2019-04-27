using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.API
{
    interface IMessage
    {
        void Approve();
        void Reject();
        void Retry();
    }
}
