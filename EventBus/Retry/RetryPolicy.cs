using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class RetryPolicy: IRetryPolicy
    {
        public int maxAttempts { get; set; }
        public int interval { get; set; }

        public RetryPolicy(int maxAttempts, int interval)
        {
            this.maxAttempts = maxAttempts;
            this.interval = interval;
        }
    }
}
