using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Koenig.Maestro.Operation.Messaging;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Koenig.Maestro.Operation.Utility;
using TransactionManager = Koenig.Maestro.Operation.Framework.ManagerRepository.TransactionManager;
using Newtonsoft.Json.Linq;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class Order : TransactionBase
    {

        QuickBooksInvoiceManager qim;
        OrderRequestType requestType;
        OrderManager orderMan;

        public Order(TransactionContext context) : base("ORDER", context)
        {
            this.IsProgressing = true;
            this.MainEntitySample = new OrderMaster();
            qim = new QuickBooksInvoiceManager(Context);
            orderMan = new OrderManager(Context);
        }

        protected override void DeserializeLog(byte[] logData)
        {
            throw new NotImplementedException();
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();

            OrderMaster om = orderMan.GetOrder(id);
            response.TransactionResult = om;
            
        }

        protected override void Delete()
        {
            long orderId = ValidateEntityIdFromDataExtension();
            //Context.TransactionObject = new Framework.ManagerRepository.OrderManager(Context).GetOrder(id);
            QuickBooksInvoiceLog log = qim.GetInvoiceLog(orderId);
            if (log.IntegrationStatus == QbIntegrationLogStatus.OK || log.IntegrationStatus == QbIntegrationLogStatus.REVOKED)
            {
                Context.TransactionObject = log;
                qim.CancelInvoice(log);
            }

            SpCall spCall = new SpCall("DAT.ORDER_MASTER_DELETE");
            spCall.SetBigInt("@ID", orderId);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(spCall);

            

        }

        protected override void List()
        {
            ExtractTransactionCriteria();

            DateTime endDate = (DateTime)Context.Bag[MessageDataExtensionKeys.END_DATE];
            DateTime beginDate = (DateTime)Context.Bag[MessageDataExtensionKeys.BEGIN_DATE];

            long customerId = (long)Context.Bag[MessageDataExtensionKeys.CUSTOMER_ID];
            string status = Context.Bag[MessageDataExtensionKeys.STATUS].ToString();
            string dateField = Context.Bag[MessageDataExtensionKeys.REQUEST_TYPE].ToString();

            List<OrderMaster> result = orderMan.List(beginDate, endDate, customerId, status, dateField);
            this.response.TransactionResult = result.Cast<ITransactionEntity>().ToList();

        }

        protected override void New()
        {
            if (requestType == OrderRequestType.InsertNewOrder)
            {
                OrderMaster om = (OrderMaster)request.TransactionEntityList[0];
                om.OrderStatus = OrderStatus.CREATED;
                om.CreatedUser = Context.UserName;
                orderMan.InsertOrder(om);


                if (request.MessageDataExtension.ContainsKey(MessageDataExtensionKeys.CREATE_INVOICE))
                    if (request.MessageDataExtension[MessageDataExtensionKeys.CREATE_INVOICE].Equals(bool.TrueString.ToLower()))
                    {
                        ExportQb();
                        
                    }
                    else
                    {
                        QuickBooksInvoiceLog log = qim.CreateInvoiceLog(om, QbIntegrationLogStatus.WAITING);
                        qim.InsertIntegrationLog(log);
                    }
                response.TransactionResult = om;
            }
            else
            {
                long newId = new OrderManager(Context).GetNewOrderId();
                this.response.TransactionResult = newId;
            }
        }

        protected override void Update()
        {
            OrderMaster om = (OrderMaster)request.TransactionEntityList[0];
            om.UpdatedUser = Context.UserName;
            bool cleanItems = extendedData.ContainsKey(MessageDataExtensionKeys.CLEAN_ORDER_ITEMS);
            orderMan.UpdateOrder(om, cleanItems);

            //refresh order with id's
            om = orderMan.GetOrder(om.Id);
            QuickBooksInvoiceLog log = om.InvoiceLog;
            
            Context.TransactionObject = om;

            if (log.IntegrationStatus == QbIntegrationLogStatus.OK || log.IntegrationStatus == QbIntegrationLogStatus.REVOKED)
                qim.UpdateQuickBooksInvoice();

        }


        protected override void ExportQb()
        {
            List<OrderMaster> omList = request.TransactionEntityList.Cast<OrderMaster>().ToList();

            qim.IntegrateOrderToQuickBooks(omList);

            List<QuickBooksInvoiceLog> logs = (List<QuickBooksInvoiceLog>)Context.TransactionObject;
            logs.ForEach(delegate (QuickBooksInvoiceLog log)
            {
                OrderMaster o = omList.Find(ord => ord.Id == log.OrderId);
                o.OrderStatus = log.IntegrationStatus == QbIntegrationLogStatus.ERROR ? OrderStatus.ERROR : OrderStatus.INTEGRATED;
                o.InvoiceLog = log;
            });
                
                

            if (logs.Exists(l=>l.IntegrationStatus == QbIntegrationLogStatus.ERROR))
            {
                List<long> errorOrderIds = logs.Where(l => l.IntegrationStatus == QbIntegrationLogStatus.ERROR).Select(l => l.OrderId).ToList();

                orderMan.UpdateOrderStatus(errorOrderIds, OrderStatus.ERROR);

                string logIds = logs.Where(l => l.IntegrationStatus == QbIntegrationLogStatus.ERROR).Select(l => l.Id.ToString()).Aggregate("", (current, next) => current + "," + next);
                warnings.Add(string.Format("Integration to Quickbooks failed. You can trigger manual integration. Integration log id`s: {0}", logIds));



                string orderIds = logs.Where(l => l.IntegrationStatus == QbIntegrationLogStatus.ERROR).Select(l => l.OrderId.ToString()).Aggregate("", (current, next) => current + "," + next);
                responseMessage = string.Format("Exception occured while creating invoice for Orders: {0}", orderIds);
            }

            if(logs.Exists(l=>l.IntegrationStatus == QbIntegrationLogStatus.OK))
            {
                List<long> doneIds = logs.Where(l => l.IntegrationStatus == QbIntegrationLogStatus.OK).Select(l => l.OrderId).ToList();

                orderMan.UpdateOrderStatus(doneIds, OrderStatus.INTEGRATED);

                if (!string.IsNullOrWhiteSpace(response.ResultMessage))
                    responseMessage += Environment.NewLine;

                string invoiceIds = logs.Where(l => l.IntegrationStatus == QbIntegrationLogStatus.OK).Select(l => l.QuickBooksInvoiceId.ToString()).Aggregate("", (current, next) => current + "," + next);
                responseMessage += string.Format("Following invoice id's have been created: {0}", invoiceIds.Remove(0,1));
            }

            if (logs.All(l => l.IntegrationStatus == QbIntegrationLogStatus.ERROR))
                throw new Exception(responseMessage);

            if (omList.Count == 1)
                response.TransactionResult = omList[0];
            else
                response.TransactionResult = omList;
        }

        protected override void ValidateRequest()
        {
            //dont go base
            //base.ValidateRequest();

            switch (request.MessageHeader.ActionType)
            {
                case ActionType.Get:
                case ActionType.Delete:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.ID))
                        throw new Exception(string.Format("MessageDataExtension does not contain key {0}", MessageDataExtensionKeys.ID));
                    break;
                case ActionType.List:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.BEGIN_DATE)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.END_DATE)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.CUSTOMER_ID)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.PERIOD)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.STATUS))
                        throw new Exception("MessageDataExtension does not contain any of order listing keys");

                    break;
                
                case ActionType.New:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.REQUEST_TYPE))
                        throw new Exception(string.Format("MessageDataExtension does not contain key {0}", MessageDataExtensionKeys.REQUEST_TYPE));

                    requestType = EnumUtils.GetEnum<OrderRequestType>(extendedData[MessageDataExtensionKeys.REQUEST_TYPE]);

                    break;
                case ActionType.Update:
                case ActionType.ExportQb:
                    break;
                case ActionType.Report:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.BEGIN_DATE)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.END_DATE)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.CUSTOMER_ID)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.PERIOD))
                        throw new Exception("MessageDataExtension does not contain any of order reporting keys");
                    break;
            }
        }


        protected override void Report()
        {
            ExtractTransactionCriteria();
            new ReportManager(Context).RunReport();

            response.TransactionResult = Context.TransactionObject;

        }

        public override void Deserialize(JToken token)
        {
            JObject entityObj = JObject.Parse(token.ToString());

            OrderMaster result = new OrderMaster()
            {
                Id = entityObj["Id"].ToObject<long>(),
                OrderDate = entityObj["OrderDate"].ToObject<DateTime>(),
                DeliveryDate = entityObj["DeliveryDate"].ToObject<DateTime>(),
                PaymentType = entityObj["PaymentType"].ToObject<string>(),
                Notes = entityObj["Notes"].ToObject<string>(),
                OrderStatus = entityObj.ContainsKey("OrderStatus") ? token["OrderStatus"].ToObject<string>() : string.Empty,
                ShippingAddressId = entityObj["ShippingAddressId"].ToObject<long>(),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                RecordStatus = "A",
                CreatedUser = Context.UserName,
                UpdatedUser = Context.UserName,
                OrderItems = new List<OrderItem>()
            };

            result.Customer = CustomerCache.Instance[entityObj["CustomerId"].ToObject<long>()];
            UnitManager um = new UnitManager(Context);
            List<JToken> orderItemTokens = entityObj["OrderItems"].Children().ToList();
            foreach(JToken itemToken in orderItemTokens)
            {
                QuickBooksProductMapDef map = QuickBooksProductMapCache.Instance[itemToken["MapId"].ToObject<long>()];
                MaestroProduct product = ProductCache.Instance[map.ProductId];
                long unitId = 0;
                if (map.UnitTypeCanHaveUnits)
                    unitId = itemToken["UnitId"].ToObject<long>();

                MaestroUnit unit = unitId> 0 ? UnitCache.Instance[unitId] : um.GetUnknownItem();
                OrderItem orderItem = new OrderItem()
                {
                    OrderId = result.Id,
                    Product = product,
                    QbProductMap = map,
                    Quantity = itemToken["Quantity"].ToObject<int>(),
                    Unit = unit,
                    Price = map.Price,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    RecordStatus = "A",
                    CreatedUser = Context.UserName,
                    UpdatedUser = Context.UserName
                };
                result.OrderItems.Add(orderItem);
            };

            Context.TransactionObject = result;

            
        }


    }
}
