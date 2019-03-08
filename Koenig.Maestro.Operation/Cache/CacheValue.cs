using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Cache
{
    public class CacheValue<TValue>
    {
        public TValue Value
        {
            get;
            set;
        }

        public DateTime ExpiryTime
        {
            get;
            set;
        }
        
        public int LoadingThreadId
        {
            get;
            set;
        }

    }
}
