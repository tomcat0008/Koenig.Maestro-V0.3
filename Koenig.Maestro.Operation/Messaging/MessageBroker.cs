using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.DbEntities;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Messaging
{
    public sealed class MessageBroker
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        TransactionBase tranBase;
        
        public ResponseMessage Execute(RequestMessage message)
        {
            DateTime start = DateTime.Now;

            TransactionContext context = TransactionManager.CreateContext(message.MessageHeader.UserName, message);
            
            ResponseMessage response = null;
            ActionType at = ActionType.Undefined;
            string tranCode = string.Empty;
            try
            {
                if(at != ActionType.Get && at != ActionType.List && at != ActionType.Undefined)
                    context.Database.BeginTransaction();

                tranCode = message.MessageHeader.TransactionCode;
                at = message.MessageHeader.ActionType;
                tranBase = new TransactionManager(context).GetTransaction(tranCode);
                
                response = tranBase.Execute(message);
                
                if(context.Database.InTransaction)
                    context.Database.CommitTransaction();

                response.TransactionStatus = "OK";
                if (at == ActionType.Delete || at == ActionType.New || at == ActionType.ImportQb || at == ActionType.Update)
                    tranBase.RefreshCache(at);

                

            }
            catch (Exception ex)
            {
                string error = string.Format("Exception occured while executing transaction {0}", new object[] { message.MessageHeader.TransactionCode });
                if (context.Database.InTransaction)
                    context.Database.RollbackTransaction();
                logger.Fatal(ex, error);
                if (response == null)
                    response = new ResponseMessage();
                response.ErrorInfo = PrepareErrorMessage(message, ex);
                response.ResultMessage = error + Environment.NewLine;
                response.TransactionStatus = "ERROR";
            }
            finally
            {
                double duration = DateTime.Now.Subtract(start).TotalMilliseconds;
                string resultMessage = string.Format("Transaction complete in {0} ms", duration);
                
                response.ResultMessage = string.IsNullOrWhiteSpace(response.ResultMessage) ? resultMessage :
                    resultMessage + Environment.NewLine + response.ResultMessage;
                response.TransactionCode = tranCode;
                response.TransactionDuration = duration;
                response.ActionType = at.ToString();
                if (at != ActionType.Get && at != ActionType.List)
                {
                    try
                    {
                        LogTransaction(context, message, response, duration);
                    }
                    catch (Exception logException)
                    {
                        string exceptionMsg = string.Format("Transaction log could not be written. Transaction info:{0}{1}", Environment.NewLine, tranBase);
                        logger.Fatal(logException, exceptionMsg);
                        response.ResultMessage += Environment.NewLine + exceptionMsg;
                    }
                }
                if(tranBase != null)
                    TransactionManager.DisposeTransaction(tranBase);
            }
            return response;

        }

        ErrorInfo PrepareErrorMessage(RequestMessage request, Exception ex)
        {
            ErrorInfo errorInfo = new ErrorInfo();
            errorInfo.StackTrace = ex.ToString();
            errorInfo.UserFriendlyMessage = ex.Message;
            errorInfo.TransactionCode = request.MessageHeader.TransactionCode;
            errorInfo.ActionType = request.MessageHeader.ActionType.ToString();
            return errorInfo;

        }

        void LogTransaction(TransactionContext comtext, RequestMessage request, ResponseMessage response, double duration)
        {

            TranLogManager tman = new TranLogManager(comtext);
            MaestroLogContainer container = new MaestroLogContainer();
            container.Request = request;
            container.Response = response;
            TransactionLog log = new TransactionLog()
            {
                CreatedUser = request.MessageHeader.UserName,
                TransactionCode = request.MessageHeader.TransactionCode,
                ActionType = request.MessageHeader.ActionType.ToString(),
                RequestType = request.RequestType,
                LogObjectId = tranBase.TranactionObjectId,
                LogJson = JsonConvert.SerializeObject(container),
                Status = response.ErrorInfo != null ? "ERROR" : "OK",
                Duration = Convert.ToDecimal(duration)
            };
            
            tman.InsertTransactionLog(log);
            response.LogID = log.Id;
        }


    }
}
