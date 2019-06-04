using Interop.QBFC13;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.QuickBooks
{
    internal abstract class QuickBooksAgent : IDisposable
    {
        protected TransactionContext context;
        protected QBSessionManager sessionManager;
        protected Database db;
        protected bool sessionOpen = false;
        bool connectionOpen = false;
        protected Dictionary<string, string> extendedData;
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected QuickBooksAgent(TransactionContext context)
        {
            this.context = context;
            db = context.Database;
            if (context.RequestMessage != null)
                extendedData = context.RequestMessage.MessageDataExtension;
            else
                extendedData = new Dictionary<string, string>();
        }

        protected void StartSession()
        {
            try
            {
                if (sessionManager == null)
                    sessionManager = new QBSessionManager();
                
                if(!connectionOpen)
                    sessionManager.OpenConnection(MaestroApplication.Instance.QuickBooksAppId, MaestroApplication.Instance.QuickBooksAppName);
                connectionOpen = true;
            }
            catch(Exception ex)
            {
                FinishSession(true);
                throw new Exception("Exception while connecting to QuickBooks", ex);
            }

            try
            {
                if(!sessionOpen)
                    sessionManager.BeginSession(MaestroApplication.Instance.QuickBooksAppPath, ENOpenMode.omDontCare);
                sessionOpen = true;
            }
            catch (Exception ex)
            {
                FinishSession(true);
                throw new Exception("Exception while starting QuickBooks session", ex);
                
            }

            
        }

        protected void FinishSession(bool finishSession)
        {
            if (finishSession)
            {
                if(sessionOpen)
                    sessionManager.EndSession();
                sessionOpen = false;
            }

            if(connectionOpen)
                sessionManager.CloseConnection();
            connectionOpen = false;

            sessionManager = null;

        }

        protected IMsgSetRequest GetLatestMsgSetRequest()
        {
            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest(MaestroApplication.Instance.QuickBooksCountry,
               MaestroApplication.Instance.QuickBooksMajorVersion, MaestroApplication.Instance.QuickBooksMinorVersion);

            requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

            return requestMsgSet;

        }

        protected IResponse GetResponse(IMsgSetRequest request)
        {
            IResponse res = null;
            try
            {
                IMsgSetResponse responseSet = sessionManager.DoRequests(request);
                res = responseSet.ResponseList.GetAt(0);
                
            }
            catch(Exception ex)
            {
                throw new Exception("Exception while retrieving response from QuickBooks", ex);
            }
            return res;
        }

        public virtual void Export() { }
        public virtual List<ITransactionEntity> Import() { return null; }
        public virtual void Cancel() { }
        public virtual void Update() { }

        public void Dispose()
        {
            FinishSession(true);
        }


        protected bool ReadBool(IQBBoolType value)
        {
            bool result = true;

            if (value != null)
                result = value.GetValue();

            return result;
        }

        protected string ReadQbId(IQBIDType value)
        {
            string result = string.Empty;

            if (value != null)
                result = value.GetValue();

            return result;
        }


        protected string ReadString(IQBStringType value)
        {
            string result = string.Empty;

            if (value != null)
                result = value.GetValue();
            return result;
        }

        protected decimal ReadPrice(IQBPriceType value)
        {
            decimal result = 0M;

            if (value != null)
                result = Convert.ToDecimal(value.GetValue());

            return result;
        }

    }
}
