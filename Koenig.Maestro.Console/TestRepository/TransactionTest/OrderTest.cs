using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console.TestRepository.TransactionTest
{
    

    internal class OrderTest : IMaestroTester
    {
        static string tranCode = "ORDER";
        static string action = string.Empty;
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            action = testData["ACTION_TYPE"].ToString();
            ResponseMessage result = null;
            switch (action)
            {
                case "New":
                    string requestType = testData["REQUEST_TYPE"].ToString();

                    if (requestType == "InsertNewOrder")
                        result = Add((long)testData["ID"]);
                    else
                        result = GetNewOrderId();
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
                    result = Update((long)testData["ID"], (int)testData["QUANTITY"], testData["NOTES"].ToString());
                    break;
            }
            return result;
        }

        static ResponseMessage Update(long orderId, int quantity, string notes)
        {
            OrderMaster item = new OrderMaster()
            {
                CreateDate = DateTime.Now.AddDays(-1),
                CreatedUser = "TEST_USER",
                Id = orderId,
                Customer = new MaestroCustomer() { Id = 88 },
                Notes = "Some notes about order",
                DeliveryDate = DateTime.Now.AddDays(3),
                OrderDate = DateTime.Now,
                OrderStatus = "ON",
                PaymentType = "COD",
                OrderItems = new List<OrderItem>(
                    new OrderItem[]
                    {
                        new OrderItem()
                        {
                            OrderId = orderId,
                            Product = new MaestroProduct(){Id = 2},
                            QbProductMap = new QuickBooksProductMapDef(){Id = 10},
                            Quantity = 12,
                            Unit = new MaestroUnit{Id = 12}
                        },
                        new OrderItem()
                        {
                            OrderId = orderId,
                            Product = new MaestroProduct(){Id = 24},
                            Quantity = 11,
                            QbProductMap = new QuickBooksProductMapDef(){Id = 74},
                            Unit = new MaestroUnit{Id = 12}
                        }
                    }
                    )

            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "UpdateQbOrder", new List<ITransactionEntity>(new ITransactionEntity[] { item }));
            request.MessageDataExtension.Add(MessageDataExtensionKeys.CLEAN_ORDER_ITEMS, "true");
            return new MessageBroker().Execute(request);

        }

        static ResponseMessage Add(long orderId)
        {
            OrderMaster item = new OrderMaster()
            {
                Id = orderId,
                Customer = new MaestroCustomer() { Id = 503},
                Notes = "Some notes about order",
                DeliveryDate = DateTime.Now.AddDays(3),
                OrderDate = DateTime.Now,
                OrderStatus = "ON",
                PaymentType = "COD",
                OrderItems = new List<OrderItem>(
                    new OrderItem[]
                    {
                        new OrderItem()
                        {
                            OrderId = orderId,
                            Product = new MaestroProduct(){Id = 164},
                            Quantity = 2,
                            Unit = new MaestroUnit{Id = 1},
                            QbProductMap = new QuickBooksProductMapDef{ Id = 386 }
                        },
                        new OrderItem()
                        {
                            OrderId = orderId,
                            Product = new MaestroProduct(){Id = 165},
                            Quantity = 1,
                            Unit = new MaestroUnit{Id = 1},
                            QbProductMap = new QuickBooksProductMapDef{ Id = 393 }
                        }
                    }
                    )

            };
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "InsertNewOrder", new List<ITransactionEntity>(new ITransactionEntity[] { item }));

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

        static ResponseMessage GetNewOrderId()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, "RequestNewId", null);
            return new MessageBroker().Execute(request);
        }

        static ResponseMessage List()
        {
            RequestMessage request = MessagePrepareAgent.GetRequest(action, tranCode, null, null);
            request.MessageDataExtension.Add(MessageDataExtensionKeys.CUSTOMER_ID, "5");
            request.MessageDataExtension.Add(MessageDataExtensionKeys.LIST_CODE, OrderRequestType.DashboardSummary.ToString());
            return new MessageBroker().Execute(request);
        }

    }
}
