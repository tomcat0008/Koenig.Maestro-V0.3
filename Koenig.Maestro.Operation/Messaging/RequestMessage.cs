using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Messaging
{
    [Serializable]
    public class RequestMessage
    {
        public MessageHeader MessageHeader { get; set; }
        public List<ITransactionEntity> TransactionEntityList { get; set; }
        public string MessageTag { get; set; }
        public Dictionary<string, string> MessageDataExtension { get; set; }
        public long ObjectId
        {
            get
            {
                long result = -1;
                if (MessageDataExtension.ContainsKey(MessageDataExtensionKeys.ID))
                    long.TryParse(MessageDataExtension[MessageDataExtensionKeys.ID], out result);
                return result;
            }
        }

        public string RequestType
        {
            get
            {
                string result = null;
                if (MessageDataExtension.ContainsKey(MessageDataExtensionKeys.REQUEST_TYPE))
                    result = MessageDataExtension[MessageDataExtensionKeys.REQUEST_TYPE];
                return result;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MessageHeader:{0} \n", MessageHeader);
            sb.AppendFormat("MessageTag:{0} \n", MessageTag ?? "null");
            if(MessageDataExtension == null)
                sb.AppendFormat("MessageDataExtension: null \n");
            else
            {
                sb.AppendFormat("MessageDataExtension: \n");
                sb.Append("{");
                foreach (KeyValuePair<string, string> kvp in MessageDataExtension)
                    sb.AppendFormat("\t {0}:`{1}` \n", kvp.Key, kvp.Value);
                sb.Append("}");
            }

            if(TransactionEntityList == null)
                sb.AppendFormat("TransactionEntityList: null \n");
            else
            {
                sb.Append("{");
                foreach (ITransactionEntity en in TransactionEntityList)
                    sb.AppendFormat("\t {0} \n", en.ToString());
                sb.Append("}");
            }


            return sb.ToString();            
        }
    }
}
