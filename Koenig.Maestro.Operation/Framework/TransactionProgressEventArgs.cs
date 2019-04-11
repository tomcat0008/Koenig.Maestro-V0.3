using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework
{
    public class TransactionProgressEventArgs : EventArgs
    {
        public TransactionProgressEventArgs(long totalCount, long current, string message)
        {
            TotalCount = totalCount;
            Current = current;
            Message = message;
        }

        public long TotalCount { get; set; }
        public long Current { get; set; }
        public string Message { get; set; }


    }
}
