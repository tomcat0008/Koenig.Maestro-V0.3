using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class TranDefTest : IMaestroTester
    {
        static string tranCode = "TRAN_DEFINITION";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    result = Add(testData["CODE_BASE"].ToString(),
                        (int)testData["MENU_ID"],
                        testData["GROUP"].ToString(), testData["DESCRIPTION"].ToString(),
                        testData["TCODE"].ToString());
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
                    result = Update(testData["CODE_BASE"].ToString(), 
                        (int)testData["MENU_ID"], testData["TCODE"].ToString(), (long)testData["ID"]);
                    break;
            }
            return result;
        }

        public static ResponseMessage Update(string codeBase, int menuId, string tcode, long id)
        {
            TransactionDefinition item = new TransactionDefinition()
            {
                Id = id,
                CodeBase = "Koenig.Maestro.Operation.TransactionRepository." + codeBase,
                GroupCode = "A group",
                IsCancellable = false,
                IsQbRelated = false,
                MenuId = menuId,
                TranCode = tcode,
                TranDescription = "A description"
            };

            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        public static ResponseMessage Add(string codeBase, int menuId, string group, string desc, string tcode)
        {
            TransactionDefinition item = new TransactionDefinition()
            {
                CodeBase = "Koenig.Maestro.Operation.TransactionRepository." + codeBase,
                GroupCode = group,
                IsCancellable = false,
                IsQbRelated = false,
                MenuId = menuId,
                TranCode = tcode,
                TranDescription = desc
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
