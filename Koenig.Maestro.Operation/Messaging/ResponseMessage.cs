using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Messaging
{
    [Serializable]
    public class ResponseMessage
    {
        public ErrorInfo ErrorInfo { get; set; }
        public string ResultMessage { get; set; }
        //public List<ITransactionEntity> ResultObjects { get; set; }
        public object TransactionResult { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionCode { get; set; }
        public long LogID { get; set; }
        public double TransactionDuration { get; set; }
        public string ActionType { get; set; }
        public List<string> Warnings { get; set; }
        public List<Dictionary<string, object>> GridDisplayMembers { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("ResponseMessage:" + Environment.NewLine);
            sb.AppendFormat("ResultMessage:{0} \n", ResultMessage ?? "null");
            sb.AppendLine(string.Format("TransactionCode: {0}", TransactionCode));
            sb.AppendLine(string.Format("Log Id: {0}", LogID));
            sb.AppendLine(string.Format("ActionType: {0}", ActionType));
            sb.AppendLine(string.Format("TransactionDuration: {0}", TransactionDuration));
            sb.AppendFormat("TransactionResult:{0} \n", TransactionResult ?? "null");
            sb.AppendFormat("TransactionStatus:{0} \n", TransactionStatus ?? "null");
            if (ErrorInfo == null)
                sb.Append("ErrorInfo: null \n");
            else
            {
                sb.Append("ErrorInfo: \n");
                sb.Append("{ \n");
                sb.AppendFormat("\t {0}", ErrorInfo);
                sb.Append("} \n");
            }

            if (TransactionResult is ICollection<ITransactionEntity>)
            {
                sb.AppendLine("Result list:");
                ICollection<ITransactionEntity> list = (ICollection<ITransactionEntity>)TransactionResult;
                foreach (ITransactionEntity te in list)
                    sb.AppendLine(te.ToString());
            }

            if (Warnings != null)
            {
                sb.AppendLine("****** WARNINGS *******");
                foreach (string w in Warnings)
                    sb.AppendLine(string.Format("Warning:{0}",w));
            }
            return sb.ToString();
        }
    }
}
