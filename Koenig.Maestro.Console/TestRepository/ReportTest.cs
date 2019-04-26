using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console.TestRepository
{
    internal class ReportTest : IMaestroTester
    {
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            ResponseMessage result = new ResponseMessage();


            string action = testData["ACTION_TYPE"].ToString();

            switch (action)
            {
                case "ORDER_SUMMARY":
                    result.TransactionResult = DoOrderSummary(testData);
                    break;

            }

            return result;
        }


        ResponseMessage DoOrderSummary(Dictionary<string, object> testData)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest("Report", "ORDER", "", null);
            foreach (KeyValuePair<string, object> kvp in testData)
                request.MessageDataExtension.Add(kvp.Key, kvp.Value.ToString());

            return new MessageBroker().Execute(request);
        }


    }
}
