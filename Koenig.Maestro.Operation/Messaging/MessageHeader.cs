using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.UserManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Messaging
{
    [Serializable]
    public class MessageHeader
    {
        public string TransactionCode { get; set; }
        public string AgentInfo { get; set; }
        public DateTime ClientDate { get; set; }
        public string UserName {get; set;}
        public ActionType ActionType { get; set; }
        public string HostName { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{"+Environment.NewLine);
            sb.AppendFormat("\t TransactionCode:{0} {1}", TransactionCode ?? "null", Environment.NewLine);
            sb.AppendFormat("\t AgentInfo:{0} {1}", AgentInfo ?? "null", Environment.NewLine);
            sb.AppendFormat("\t UserCode:{0} {1}", UserName ?? "null", Environment.NewLine);
            sb.AppendFormat("\t ClientDate:{0} {1}", ClientDate, Environment.NewLine);
            sb.AppendFormat("\t ActionType:{0} {1}", ActionType.ToString() ?? "null", Environment.NewLine);
            sb.AppendFormat("\t HostName:{0} {1}", HostName ?? "null", Environment.NewLine);
            sb.Append("}"+Environment.NewLine);
            return sb.ToString();            

        }

    }
}
