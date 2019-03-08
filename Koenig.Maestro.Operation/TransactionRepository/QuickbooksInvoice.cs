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
        public QuickbooksInvoice(TransactionContext context) : base("QUICKBOOKS_INVOICE", context) { }
        QuickBooksInvoiceManager invMan;
        protected override void Delete()
        {
            invMan = new QuickBooksInvoiceManager(Context);
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
            throw new NotImplementedException();
        }

        protected override void New()
        {
            throw new NotImplementedException();
        }

        protected override void Update()
        {

        }
    }
}
