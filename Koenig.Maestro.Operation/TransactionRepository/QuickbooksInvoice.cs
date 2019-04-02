using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Koenig.Maestro.Operation.QuickBooks;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework.ManagerRepository;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class QuickbooksInvoice : TransactionBase
    {
        QuickBooksInvoiceManager invMan;

        public QuickbooksInvoice(TransactionContext context) : base("QUICKBOOKS_INVOICE", context)
        {
            this.MainEntitySample = new QuickBooksInvoiceLog();
            invMan = new QuickBooksInvoiceManager(Context);
        }
        
        protected override void Delete()
        {
            
        }

        protected override void ExportQb()
        {
            List<QuickBooksInvoiceLog> list = request.TransactionEntityList.Cast<QuickBooksInvoiceLog>().ToList() ;

            List<OrderMaster> orders = new OrderManager(Context).GetOrders(list.Select(l => l.OrderId).ToList());

            orders = orders.Where(om => om.IntegrationStatus == QbIntegrationLogStatus.WAITING || om.IntegrationStatus == QbIntegrationLogStatus.ERROR).ToList();

            long batchId = CreateBatch(orders.Select(o => o.Id).ToList());
            string batchStatus = QbIntegrationLogStatus.OK;
            using (qbAgent = new QuickBooksInvoiceAgent(Context))
            {
                foreach (OrderMaster orderMaster in orders)
                {
                    Context.TransactionObject = orderMaster;
                    orderMaster.InvoiceLog.BatchId = batchId;
                    try
                    {
                        qbAgent.Export();
                        orderMaster.InvoiceLog.IntegrationStatus = QbIntegrationLogStatus.OK;
                        orderMaster.InvoiceLog.ErrorLog = string.Empty;
                        invMan.UpdateInvoiceLog(orderMaster.InvoiceLog);
                    }
                    catch(Exception ex)
                    {
                        string msg = string.Format("Exception occured while integrationg order to Quickbooks. Order id:{0}", orderMaster.Id);
                        logger.Error(ex, msg);
                        warnings.Add(msg);
                        orderMaster.InvoiceLog.ErrorLog = ex.ToString();
                        orderMaster.InvoiceLog.IntegrationStatus = QbIntegrationLogStatus.ERROR;
                        invMan.UpdateInvoiceLog(orderMaster.InvoiceLog);
                        batchStatus = QbIntegrationLogStatus.ERROR;
                    }
                }
            }
            invMan.UpdateBatch(batchId, batchStatus);
        }

        long CreateBatch(List<long> orderIds)
        {
            long batchId = invMan.CreateIntegrationBatch(orderIds);
            return batchId;
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = invMan.GetInvoiceLog(id);
        }

        protected override void ImportQb()
        {
            using (qbAgent = new QuickBooksInvoiceAgent(Context))
            {
                qbAgent.Import();
            }
        }

        protected override void List()
        {
            DateTime endDate = DateTime.Now.AddDays(1);
            DateTime beginDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            long customerId = -1, batchID = 0;
            string status = string.Empty;

            if (extendedData.ContainsKey(MessageDataExtensionKeys.BEGIN_DATE))
                DateTime.TryParse(extendedData[MessageDataExtensionKeys.BEGIN_DATE], out beginDate);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.END_DATE))
                DateTime.TryParse(extendedData[MessageDataExtensionKeys.END_DATE], out endDate);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.CUSTOMER_ID))
                long.TryParse(extendedData[MessageDataExtensionKeys.CUSTOMER_ID], out customerId);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.STATUS))
                status = extendedData[MessageDataExtensionKeys.STATUS];
            if (extendedData.ContainsKey(MessageDataExtensionKeys.BATCH_ID))
                long.TryParse(extendedData[MessageDataExtensionKeys.BATCH_ID], out batchID);


            List<QuickBooksInvoiceLog> result = invMan.List(beginDate, endDate, customerId, status, batchID);
            this.response.TransactionResult = result.Cast<ITransactionEntity>().ToList();

        }

        protected override void New()
        {
            throw new NotImplementedException();
        }

        protected override void Update()
        {

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
                case ActionType.Update:
                case ActionType.ExportQb:
                    break;
            }
        }

    }
}
