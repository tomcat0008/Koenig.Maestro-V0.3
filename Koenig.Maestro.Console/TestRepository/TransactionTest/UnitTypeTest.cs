using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class UnitTypeTest : IMaestroTester
    {
        static string tranCode = "UNIT_TYPE";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    result = Add(testData["NAME"].ToString(),
                        testData["DESCRIPTION"].ToString(), (bool)testData["CAN_HAVE_UNITS"]);
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
                        testData["DESCRIPTION"].ToString(),(bool)testData["CAN_HAVE_UNITS"], 
                        (long)testData["ID"]);
                    break;
            }
            return result;
        }

        static ResponseMessage Update(string name, string description, bool canHaveUnits, long id)
        {
            MaestroUnitType item = new MaestroUnitType()
            {
                Id = id,
                CanHaveUnits = canHaveUnits,
                Name = name,
                Description = description
            };

            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        static ResponseMessage Add(string name, string description, bool canHaveUnits)
        {
            MaestroUnitType item = new MaestroUnitType()
            {
                CanHaveUnits = canHaveUnits,
                Name = name,
                Description = description
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
