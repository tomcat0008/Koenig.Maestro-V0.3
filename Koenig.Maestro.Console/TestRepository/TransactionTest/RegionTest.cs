using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class RegionTest : IMaestroTester
    {
        static string tranCode = "REGION";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    result = Add(testData["NAME"].ToString(), testData["PK"].ToString());
                    break;
                case "Delete":
                    result = Delete((long)testData["ID"]);
                    break;
                case "Get":
                    result = Get((long)testData["ID"]);
                    break;
                case "List":
                    result = List();
                    break;
                case "Update":
                    result = Update(testData["NAME"].ToString(), testData["PK"].ToString(), (long)testData["ID"]);
                    break;
            }
            return result;
        }


        public static ResponseMessage Update(string name, string postalCode, long id)
        {
            MaestroRegion item = new MaestroRegion()
            {
                Id = id,
                Description = "Region description",
                Name = name,
                PostalCode = postalCode
            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        public static ResponseMessage Add(string name, string postalCode)
        {
            MaestroRegion item = new MaestroRegion()
            {
                Description = "Region description",
                Name = name,
                PostalCode = postalCode
            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        public static ResponseMessage Delete(long id)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("ID", id.ToString());

            return new MessageBroker().Execute(request);
        }

        public static ResponseMessage Get(long id)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("ID", id.ToString());
            return new MessageBroker().Execute(request);
        }

        public static ResponseMessage List()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            return new MessageBroker().Execute(request);
        }

    }
}
