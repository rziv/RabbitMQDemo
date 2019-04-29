using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    interface IMessageEventArgs
    {
        byte[] Body  { get; set; }
    }
}
