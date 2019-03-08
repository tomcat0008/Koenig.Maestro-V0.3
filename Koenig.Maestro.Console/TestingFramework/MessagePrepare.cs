using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console
{
    internal class MessagePrepareAgent
    {
        public static RequestMessage GetRequest(string actionType, string tranCode, string requestType, List<ITransactionEntity> list)
        {
            RequestMessage result = new RequestMessage();
            MessageHeader hdr = new MessageHeader();
            ActionType at = ActionType.Undefined;
            bool b = Enum.TryParse<ActionType>(actionType, out at);
            hdr.HostName = Environment.MachineName;
            hdr.TransactionCode = tranCode;
            hdr.AgentInfo = "CONSOLE_TESTER";
            hdr.ClientDate = DateTime.Now;
            hdr.UserName = "TEST_USER";
            hdr.ActionType = at;
            result.MessageHeader = hdr;
            result.MessageTag = "A message tag";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add(MessageDataExtensionKeys.REQUEST_TYPE, requestType);
            result.MessageDataExtension = dict;
            result.TransactionEntityList = list;
            return result;

        }


    }
}
