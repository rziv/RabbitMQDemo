using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Retry
{
    public interface IRetryPolicy
    {
        int maxAttempts { get; set; }
        int interval { get; set; }
    }
}
