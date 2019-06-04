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
            this.IsProgressing = true;
            this.MainEntitySample = new QuickBooksInvoiceLog();
            invMan = new QuickBooksInvoiceManager(Context);
            invMan.TransactionProgress += InvMan_TransactionProgress;
        }

        private void InvMan_TransactionProgress(object sender, TransactionProgressEventArgs e)
        {
            OnTransactionProgress(e);
        }

        protected override void Delete()
        {
            
        }

        protected override void Merge()
        {

        }

        protected override void ExportQb()
        {

            string invoiceIdString = request.MessageDataExtension["INVOICE_LIST"];
            string[] invoiceIdListStr = invoiceIdString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            List<long> invoiceIdList = invoiceIdListStr.Select(i => long.Parse(i)).ToList();

            OrderManager orderMan = new OrderManager(Context);

            List<QuickBooksInvoiceLog> list = invMan.List(invoiceIdList);

            List<OrderMaster> orders = orderMan.GetOrders(list.Select(l => l.OrderId).ToList());

            orders = orders.Where(om => om.IntegrationStatus == QbIntegrationLogStatus.WAITING || om.IntegrationStatus == QbIntegrationLogStatus.ERROR).ToList();
            long counter = 0;
            long batchId = CreateBatch(orders.Select(o => o.Id).ToList());
            string batchStatus = QbIntegrationLogStatus.OK;
            using (qbAgent = new QuickBooksInvoiceAgent(Context))
            {
                counter++;
                foreach (OrderMaster orderMaster in orders)
                {
                    Context.TransactionObject = orderMaster;
                    orderMaster.InvoiceLog.BatchId = batchId;
                    string eventMessage = string.Empty;
                    try
                    {
                        qbAgent.Export();
                        orderMaster.InvoiceLog.IntegrationStatus = orderMaster.InvoiceLog.IntegrationStatus == QbIntegrationLogStatus.ERROR ? QbIntegrationLogStatus.REVOKED : QbIntegrationLogStatus.OK;
                        orderMaster.InvoiceLog.ErrorLog = string.Empty;
                        orderMaster.InvoiceLog.QuickBooksTxnId = Context.Bag["TXN_ID"].ToString();
                        orderMaster.InvoiceLog.QuickBooksInvoiceId = Context.Bag["REF_NUMBER"].ToString();
                        orderMaster.OrderStatus = OrderStatus.INTEGRATED;
                        eventMessage = string.Format("Invoice nr {0} created for order {1}", orderMaster.InvoiceLog.QuickBooksInvoiceId, orderMaster.Id);
                        
                    }
                    catch(Exception ex)
                    {
                        string msg = string.Format("Exception occured while integrationg order to Quickbooks. Order id:{0}", orderMaster.Id);
                        logger.Error(ex, msg);
                        warnings.Add(msg);
                        orderMaster.InvoiceLog.ErrorLog = ex.ToString();
                        orderMaster.InvoiceLog.IntegrationStatus = QbIntegrationLogStatus.ERROR;
                        orderMaster.OrderStatus = OrderStatus.ERROR;
                        batchStatus = QbIntegrationLogStatus.ERROR;
                        eventMessage = msg;
                        
                    }
                    finally
                    {
                        invMan.UpdateInvoiceLog(orderMaster.InvoiceLog);
                        orderMan.UpdateOrder(orderMaster, false);
                    }
                    responseMessage += eventMessage + Environment.NewLine;
                    
                    OnTransactionProgress(new TransactionProgressEventArgs(orders.Count, counter, eventMessage));
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
            DateTime dt = DateTime.MinValue;
            if (!DateTime.TryParse(extendedData[MessageDataExtensionKeys.BEGIN_DATE], out dt))
                throw new Exception(string.Format("Invalid begin date criteria `{0}`", extendedData[MessageDataExtensionKeys.BEGIN_DATE]));
            Context.Bag.Add(MessageDataExtensionKeys.BEGIN_DATE, dt);

            if (!DateTime.TryParse(extendedData[MessageDataExtensionKeys.END_DATE], out dt))
                throw new Exception(string.Format("Invalid end date criteria `{0}`", extendedData[MessageDataExtensionKeys.END_DATE]));

            
            Context.Bag.Add(MessageDataExtensionKeys.END_DATE, dt);
            
            using (qbAgent = new QuickBooksInvoiceAgent(Context))
            {
                
                qbAgent.Import();
            }
        }

        protected override void List()
        {
            ExtractTransactionCriteria();

            DateTime endDate = (DateTime)Context.Bag[MessageDataExtensionKeys.END_DATE];
            DateTime beginDate = (DateTime)Context.Bag[MessageDataExtensionKeys.BEGIN_DATE];

            long customerId = (long)Context.Bag[MessageDataExtensionKeys.CUSTOMER_ID];
            string status = Context.Bag[MessageDataExtensionKeys.STATUS].ToString();
            long batchID = (long)Context.Bag[MessageDataExtensionKeys.BATCH_ID];
            bool notIntegrated = (bool)Context.Bag[MessageDataExtensionKeys.NOT_INTEGRATED];

            List<QuickBooksInvoiceLog> result = invMan.List(beginDate, endDate, customerId, status, batchID, notIntegrated);
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
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.STATUS)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.PERIOD)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.NOT_INTEGRATED)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.BATCH_ID))
                        throw new Exception("MessageDataExtension does not contain any of order listing keys");

                    break;
                case ActionType.New:
                case ActionType.Update:
                case ActionType.ExportQb:
                    break;
                case ActionType.ImportQb:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.BEGIN_DATE)
                        && !extendedData.ContainsKey(MessageDataExtensionKeys.END_DATE))
                        throw new Exception("MessageDataExtension does not contain BEGIN_DATE and END_DATE keys");
                    break;


            }
        }

        public override void Dispose()
        {
            base.Dispose();
            invMan.TransactionProgress -= InvMan_TransactionProgress;
        }

    }
}
