using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class CustomerTest : IMaestroTester
    {
        static string tranCode = "CUSTOMER";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch(action)
            {
                case "New":
                    result = Add((long)testData["REGION_ID"], testData["NAME"].ToString(), testData["TITLE"].ToString());
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
                    result = Update(testData["TITLE"].ToString(), testData["PHONE"].ToString(), 
                        (long)testData["REGION_ID"], (long)testData["ID"]);
                    break;
                case "ImportQb":
                    result = Import();
                    break;
                case "ExportQb":
                    result = Export();
                    break;
            }
            return result;
        }

        ResponseMessage Export()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("EXPORT_TYPE", "Full");
            return new MessageBroker().Execute(request);
        }

        ResponseMessage Import()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("IMPORT_TYPE", "Full");
            return new MessageBroker().Execute(request);
        }

        ResponseMessage Update(string title, string phone, long regionId, long id)
        {
            MaestroCustomer item = new MaestroCustomer()
            {
                Id = id,
                Address = "An address",
                DefaultPaymentType = "COD",
                Email = "email@email.com",
                Name = "A customer",
                Region = new MaestroRegion() { Id = regionId },
                Title = title,
                Phone = phone

            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);
        }

        ResponseMessage Add(long regionId, string name, string title)
        {
            MaestroCustomer item = new MaestroCustomer()
            {
                Address = "An address",
                DefaultPaymentType = "COD",
                Email = "email@email.com",
                Name = "A customer",
                Region = new MaestroRegion() { Id = regionId },
                Title = "A Title",
                Phone = "123456789",

            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);

        }

        ResponseMessage Delete(long id)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("ID", id.ToString());

            return new MessageBroker().Execute(request);
        }

        ResponseMessage Get(long id)
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add("ID", id.ToString());
            return new MessageBroker().Execute(request);
        }

        ResponseMessage List()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            return new MessageBroker().Execute(request);
        }
    }
}
