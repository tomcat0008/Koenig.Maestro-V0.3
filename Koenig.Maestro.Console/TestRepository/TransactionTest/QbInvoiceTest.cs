using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class QbInvoiceTest : IMaestroTester
    {
        static string tranCode = "QUICKBOOKS_INVOICE";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    /*
                    result = Add((int)testData["MINIMUM_ORDER"], (decimal)testData["PRICE"],
                        (long)testData["UNIT_TYPE_ID"], testData["NAME"].ToString(),
                        (long)testData["GROUP_ID"]);
                    */
                    break;
                case "Delete":
                    /*
                    result = Delete((long)testData["ID"]);
                    */
                    break;
                case "Get":
                    /*
                    result = Get((long)testData["ID"]);
                    */
                    break;
                case "List":
                    /*
                    result = List();
                    */
                    break;
                case "Update":
                    /*
                    result = Update((int)testData["MINIMUM_ORDER"],
                        testData["NAME"].ToString(), (decimal)testData["PRICE"], (long)testData["UNIT_TYPE_ID"]
                        , (long)testData["ID"], (long)testData["GROUP_ID"]);
                        */
                    break;
                case "ImportQb":
                    result = Import();
                    break;
            }
            return result;
        }

        ResponseMessage Import()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("IMPORT_TYPE", "Full");
            return new MessageBroker().Execute(request);
        }
    }
}
