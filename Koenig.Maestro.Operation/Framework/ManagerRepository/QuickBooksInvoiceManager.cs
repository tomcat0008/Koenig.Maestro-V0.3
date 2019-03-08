using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.DbEntities;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.QuickBooks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class QuickBooksInvoiceManager : ManagerBase
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public QuickBooksInvoiceManager(TransactionContext context) : base(context)
        {
        }

        public long CreateIntegrationBatch(List<long> orderIds)
        {

            SpCall spCall = new SpCall("DAT.QB_BATCH_INTEGRATION_INSERT");
            spCall.SetStructured<long>("@ID_LIST", "COR.ID_LIST", "ID", orderIds);
            spCall.SetDateTime("@CREATE_DATE", DateTime.Now);
            spCall.SetVarchar("@CREATE_USER", context.UserName);

            long result = db.ExecuteScalar<long>(spCall);

            return result;
        }

        public void IntegrateOrderToQuickBooks(List<OrderMaster> omList)
        {

            bool sendToQb = false;

            if (context.RequestMessage.MessageDataExtension.ContainsKey(MessageDataExtensionKeys.SEND_TO_QB))
                bool.TryParse(context.RequestMessage.MessageDataExtension[MessageDataExtensionKeys.SEND_TO_QB], out sendToQb);

            List<QuickBooksInvoiceLog> logs = new List<QuickBooksInvoiceLog>();

            using (QuickBooksInvoiceAgent agent = new QuickBooksInvoiceAgent(context))
            {
                foreach (OrderMaster om in omList)
                {
                    string status = sendToQb ? (string.IsNullOrWhiteSpace(om.IntegrationStatus) ? QbIntegrationLogStatus.OK : QbIntegrationLogStatus.REVOKED ) : QbIntegrationLogStatus.WAITING;

                    QuickBooksInvoiceLog log = CreateInvoiceLog(om, status);
                    context.TransactionObject = om;
                    string qbInvoiceId = string.Empty;
                    string qbTxnId = string.Empty;
                    try
                    {
                        
                        agent.Export();
                        if (context.TransactionObject != null)
                            if (context.TransactionObject is Tuple<string, string>)
                            {
                                Tuple<string, string> tuple = context.TransactionObject as Tuple<string, string>;
                                qbInvoiceId = tuple.Item1;
                                qbTxnId = tuple.Item2;
                            }
                        log.QuickBooksInvoiceId = qbInvoiceId;
                        log.QuickBooksTxnId = qbTxnId;
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("Exception occured while integrating Order {0} to Quickbooks.", om.Id);
                        logger.Error(ex, message);
                        log.IntegrationStatus = QbIntegrationLogStatus.ERROR;
                        log.ErrorLog = ex.ToString();
                        context.Warnings.Add(message);
                    }

                    InsertIntegrationLog(log);

                    logs.Add(log);
                }

            }

            context.TransactionObject = logs;
        }
       


        QuickBooksInvoiceLog CreateInvoiceLog(OrderMaster om, string status)
        {
            QuickBooksInvoiceLog log = new QuickBooksInvoiceLog()
            {
                BatchId = 0,
                CreateDate = DateTime.Now,
                CreatedUser = context.UserName,
                Customer = om.Customer,
                QuickBooksCustomerId = om.Customer.QuickBooksId,
                ErrorLog = string.Empty,
                IntegrationDate = DateTime.Now,
                OrderId = om.Id,
                RecordStatus = "A",
                UpdateDate = DateTime.Now,
                UpdatedUser = context.UserName,
                IntegrationStatus = status
            };

            return log;
        }

        public void UpdateQuickBooksInvoice()
        {
            using (QuickBooksInvoiceAgent agent = new QuickBooksInvoiceAgent(context))
            {
                agent.Update();
            }
        }

        public QuickBooksInvoiceLog GetInvoiceLog(long orderId)
        {
            SpCall spCall = new SpCall("DAT.QB_INVOICE_LOG_SELECT_BY_ORDER_ID");
            spCall.SetBigInt("@ORDER_ID", orderId);

            QuickBooksInvoiceLog result = null;

            using (SqlReader reader = db.ExecuteReader(spCall))
            {
                if (reader.HasRows)
                {
                    reader.Read();


                    result = new QuickBooksInvoiceLog()
                    {
                        Id = reader.GetInt64("ID"),
                        BatchId = reader.GetInt64("BATCH_ID"),
                        CreateDate = reader.GetDateTime("CREATE_DATE"),
                        CreatedUser = reader.GetString("CREATE_USER"),
                        Customer = CustomerCache.Instance[reader.GetInt64("MAESTRO_CUSTOMER_ID")],
                        ErrorLog = reader.GetString("ERROR_LOG"),
                        IntegrationDate = reader.GetDateTime("INTEGRATION_DATE"),
                        IntegrationStatus = reader.GetString("INTEGRATION_STATUS"),
                        OrderId = reader.GetInt64("ORDER_ID"),
                        QuickBooksCustomerId = reader.GetString("QB_CUSTOMER_ID"),
                        QuickBooksInvoiceId = reader.GetString("QB_INVOICE_NO"),
                        QuickBooksTxnId = reader.GetString("QB_TXN_ID"),
                        RecordStatus = reader.GetString("RECORD_STATUS"),
                        UpdateDate = reader.GetDateTime("UPDATE_DATE"),
                        UpdatedUser = reader.GetString("UPDATE_USER")
                    };
                }
                else
                    throw new Exception(string.Format("Integration log for order {0} could not be found.", orderId));
                reader.Close();
            }
            return result;

        }

        void InsertIntegrationLog(QuickBooksInvoiceLog log)
        {
            SpCall spCall = new SpCall("DAT.QB_INVOICE_LOG_INSERT");
            spCall.SetBigInt("@ORDER_ID", log.OrderId);
            spCall.SetBigInt("@MAESTRO_CUSTOMER_ID", log.Customer.Id);
            spCall.SetVarchar("@QB_CUSTOMER_ID", log.Customer.QuickBooksId);
            spCall.SetVarchar("@INTEGRATION_STATUS", log.IntegrationStatus);
            spCall.SetDateTime("@INTEGRATION_DATE", log.IntegrationDate);
            spCall.SetBigInt("@BATCH_ID", log.BatchId);
            spCall.SetVarchar("@QB_INVOICE_NO", log.QuickBooksInvoiceId);
            spCall.SetVarchar("@QB_TXN_ID", log.QuickBooksTxnId);
            spCall.SetVarchar("@ERROR_LOG", log.ErrorLog);
            spCall.SetDateTime("@CREATE_DATE", log.CreateDate);
            spCall.SetVarchar("@CREATE_USER", context.UserName);

            long result = db.ExecuteScalar<long>(spCall);
            log.Id = result;
        }

        public void UpdateInvoiceLog(QuickBooksInvoiceLog log)
        {
            SpCall spCall = new SpCall("DAT.QB_INVOICE_LOG_UPDATE");
            spCall.SetBigInt("@ID", log.Id);
            spCall.SetVarchar("@INTEGRATION_STATUS", log.IntegrationStatus);
            spCall.SetDateTime("@INTEGRATION_DATE", log.IntegrationDate);
            spCall.SetBigInt("@BATCH_ID", log.BatchId);
            spCall.SetVarchar("@QB_INVOICE_NO", log.QuickBooksInvoiceId);
            spCall.SetVarchar("@QB_TXN_ID", log.QuickBooksTxnId);
            spCall.SetVarchar("@ERROR_LOG", log.ErrorLog);
            spCall.SetDateTime("@UPDATE_DATE", log.CreateDate);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            spCall.SetVarchar("@RECORD_STATUS", log.RecordStatus);
            db.ExecuteNonQuery(spCall);

        }

        public void UpdateBatch(long batchId, string batchStatus)
        {
            SpCall spCall = new SpCall("DAT.QB_BATCH_INTEGRATION_UPDATE");
            spCall.SetBigInt("@ID", batchId);
            spCall.SetVarchar("@STATUS", batchStatus);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(spCall);
            
        }

        public void CancelInvoice(QuickBooksInvoiceLog log)
        {
            context.TransactionObject = log;
            using (QuickBooksInvoiceAgent agent = new QuickBooksInvoiceAgent(context))
            {
                agent.Cancel();
            }

            log.IntegrationStatus = QbIntegrationLogStatus.CANCELLED;
            log.UpdatedUser = context.UserName;
            log.UpdateDate = DateTime.Now;
            UpdateInvoiceLog(log);


        }

    }
}
