using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class UnitTest : IMaestroTester
    {
        static string tranCode = "UNIT";
        static string action = string.Empty;

        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    result = Add(testData["NAME"].ToString(),
                        (testData["QB_UNIT"]==null ? null:
                        testData["QB_UNIT"].ToString()), (long)testData["UNIT_TYPE_ID"]);
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
                    result = Update(testData["NAME"].ToString(),
                        (testData["QB_UNIT"] == null ? null :
                        testData["QB_UNIT"].ToString()), (long)testData["UNIT_TYPE_ID"],
                        (long)testData["ID"]);
                    break;
            }
            return result;
        }

        static ResponseMessage Update(string name, string qbUnit, long unitTypeId, long id)
        {
            MaestroUnit item = new MaestroUnit()
            {
                Id = id,
                Name = name,
                QuickBooksUnit = qbUnit,
                UnitType = new MaestroUnitType() { Id = unitTypeId },
            };

            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        static ResponseMessage Add(string name, string qbUnit, long unitTypeId)
        {
            MaestroUnit item = new MaestroUnit()
            {
                Name = name,
                QuickBooksUnit = qbUnit,
                UnitType = new MaestroUnitType() { Id = unitTypeId},
            };

            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        static ResponseMessage Delete(long id)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("ID", id.ToString());

            return new MessageBroker().Execute(request);
        }

        static ResponseMessage Get(long id)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("ID", id.ToString());
            return new MessageBroker().Execute(request);
        }

        static ResponseMessage List()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            return new MessageBroker().Execute(request);
        }

    }
}
