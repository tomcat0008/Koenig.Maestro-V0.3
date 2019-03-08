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

        private static readonly  NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        QuickBooksInvoiceManager qim;
        OrderRequestType requestType;
        OrderManager orderMan;
        public Order(TransactionContext context) : base("ORDER", context)
        {
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
            DateTime endDate = DateTime.Now.AddDays(1);
            DateTime beginDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            long customerId = -1;
            string status = string.Empty;
            string dateField = OrderRequestType.ListByOrderDate.ToString();
            if (extendedData.ContainsKey(MessageDataExtensionKeys.BEGIN_DATE))
                DateTime.TryParse(extendedData[MessageDataExtensionKeys.BEGIN_DATE], out beginDate);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.END_DATE))
                DateTime.TryParse(extendedData[MessageDataExtensionKeys.END_DATE], out endDate);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.CUSTOMER_ID))
                long.TryParse(extendedData[MessageDataExtensionKeys.CUSTOMER_ID], out customerId);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.STATUS))
                status = extendedData[MessageDataExtensionKeys.STATUS];
            if (extendedData.ContainsKey(MessageDataExtensionKeys.REQUEST_TYPE))
                dateField = extendedData[MessageDataExtensionKeys.REQUEST_TYPE];


            List<OrderMaster> result = orderMan.List(beginDate, endDate, customerId, status, dateField);
            this.response.TransactionResult = result.Cast<ITransactionEntity>().ToList();

        }

        protected override void New()
        {
            if (requestType == OrderRequestType.InsertNewOrder)
            {
                OrderMaster om = (OrderMaster)request.TransactionEntityList[0];
                om.CreatedUser = Context.UserName;
                orderMan.InsertOrder(om);
                Context.TransactionObject = om;

                ExportQb();
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

            if(logs.Exists(l=>l.IntegrationStatus == QbIntegrationLogStatus.ERROR))
            {
                string logIds = logs.Where(l => l.IntegrationStatus == QbIntegrationLogStatus.ERROR).Select(l => l.Id.ToString()).Aggregate("", (current, next) => current + "," + next);
                warnings.Add(string.Format("Integration to Quickbooks failed. You can trigger manual integration. Integration log id`s: {0}", logIds));
            }
            
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
            }
        }

        public override void Deserialize(JToken token)
        {
            OrderMaster result = new OrderMaster()
            {
                Id = token["Id"].ToObject<long>(),
                OrderDate = token["OrderDate"].ToObject<DateTime>(),
                DeliveryDate = token["DeliveryDate"].ToObject<DateTime>(),
                PaymentType = token["PaymentType"].ToObject<string>(),
                Notes = token["Notes"].ToObject<string>(),
                OrderStatus = token["OrderStatus"].ToObject<string>(),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                RecordStatus = "A",
                CreatedUser = Context.UserName,
                UpdatedUser = Context.UserName,
                OrderItems = new List<OrderItem>()
            };

            result.Customer = CustomerCache.Instance[token["Customer"].ToObject<long>()];
            List<JToken> orderItemTokens = token["OrderItems"].Children().ToList();
            foreach(JToken itemToken in orderItemTokens)
            {
                Koenig.Maestro.Entity.QuickBooksProductMapDef map = QuickBooksProductMapCache.Instance[itemToken["QbProductMap"].ToObject<long>()];
                MaestroProduct product = ProductCache.Instance[map.Product.Id];
                MaestroUnit unit = UnitCache.Instance[itemToken["Unit"].ToObject<long>()];
                OrderItem orderItem = new OrderItem()
                {
                    OrderId = result.Id,
                    Product = product,
                    QbProductMap = map,
                    Quantity = itemToken["Quantity"].ToObject<int>(),
                    Unit = unit,
                    Price = itemToken["Price"].ToObject<decimal>(),
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
