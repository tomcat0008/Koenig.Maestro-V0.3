using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Scheduler.Console
{
    public class ConsoleLogEventArgs : EventArgs
    {
        public readonly string Message;
        public readonly Exception Exception;

        public ConsoleLogEventArgs(string message, Exception  ex)
        {
            this.Message = message;
            this.Exception = ex;
        }


    }
}
