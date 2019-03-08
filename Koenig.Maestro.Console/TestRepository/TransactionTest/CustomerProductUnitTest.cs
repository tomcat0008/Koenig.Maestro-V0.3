using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    internal class CustomerProductUnitTest : IMaestroTester
    {
        static string tranCode = "CUSTOMER_PRODUCT_UNIT";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    result = Add((long)testData["CUSTOMER_ID"], (long)testData["PRODUCT_ID"], (long)testData["UNIT_ID"]);
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
                    result = Update((long)testData["CUSTOMER_ID"], (long)testData["PRODUCT_ID"],
                        (long)testData["UNIT_ID"], (long)testData["ID"]);
                    break;
            }
            return result;
        }


        ResponseMessage Update(long customerId, long productId, long unitId, long id)
        {
            CustomerProductUnit item = new CustomerProductUnit()
            {
                Id = id,
                Customer = new MaestroCustomer { Id = customerId},
                Product = new MaestroProduct { Id = productId},
                Unit = new MaestroUnit { Id = unitId}
                
            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

            return new MessageBroker().Execute(request);
        }

        ResponseMessage Add(long customerId, long productId, long unitId)
        {
            CustomerProductUnit item = new CustomerProductUnit()
            {
                Customer = new MaestroCustomer { Id = customerId },
                Product = new MaestroProduct { Id = productId },
                Unit = new MaestroUnit { Id = unitId }

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
