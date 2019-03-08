using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class ProductTest : IMaestroTester
    {
        static string tranCode = "PRODUCT";

        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    result = Add((int)testData["MINIMUM_ORDER"], (decimal)testData["PRICE"],
                        (long)testData["UNIT_TYPE_ID"], testData["NAME"].ToString(),
                        (long)testData["GROUP_ID"]);
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
                    result = Update((int)testData["MINIMUM_ORDER"],
                        testData["NAME"].ToString(), (decimal)testData["PRICE"],(long)testData["UNIT_TYPE_ID"]
                        , (long)testData["ID"], (long)testData["GROUP_ID"]);
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

        public static ResponseMessage Update(int minimumOrder, string name, decimal price, long unitTypeId, long id, long groupId)
        {
            MaestroProduct item = new MaestroProduct()
            {
                Id = id,
                Name = name,
                Description = "A description",
                MinimumOrderQuantity = minimumOrder,
                Price = price,
                QuickBooksProductId = string.Empty,
                GroupId = groupId,
                UnitType = new MaestroUnitType() { Id = unitTypeId }
            };

            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        public static ResponseMessage Add(int minimumOrder, decimal price, long unitTypeId, string name, long groupId)
        {
            MaestroProduct item = new MaestroProduct()
            {
                Name = name,
                Description = "A description",
                MinimumOrderQuantity = minimumOrder,
                Price = price,
                QuickBooksProductId = string.Empty,
                GroupId = groupId,
                UnitType = new MaestroUnitType() { Id = unitTypeId}
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
