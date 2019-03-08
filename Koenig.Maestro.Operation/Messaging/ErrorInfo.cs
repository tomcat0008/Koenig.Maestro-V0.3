using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Messaging
{
    [Serializable]
    public class ErrorInfo
    {
        public string UserFriendlyMessage { get; set; }
        public string StackTrace { get; set; }
        public string TransactionCode { get; set; }
        public string ActionType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UserFriendlyMessage:{0} {1}", UserFriendlyMessage, Environment.NewLine);
            sb.AppendFormat("StackTrace:{0} {1}", StackTrace, Environment.NewLine);
            sb.AppendFormat("TransactionCode:{0} {1}", TransactionCode, Environment.NewLine);
            sb.AppendFormat("ActionType:{0} {1}", ActionType, Environment.NewLine);
            return sb.ToString();
        }

    }
}
