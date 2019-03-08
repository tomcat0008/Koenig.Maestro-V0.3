using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.DbEntities;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Koenig.Maestro.Operation.Messaging;
using Koenig.Maestro.Operation.QuickBooks;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Koenig.Maestro.Operation.Framework
{
    internal abstract class TransactionBase : IDisposable
    {
        public TransactionContext Context { get; private set; }
        protected Database db;
        protected QuickBooksAgent qbAgent;
        public string TransactionCode { get; internal set; }
        
        protected ResponseMessage response;
        protected RequestMessage request;

        protected Dictionary<string, string> extendedData;

        protected string responseMessage = string.Empty;

        
        protected List<string> warnings; 
        public List<string> Warnings { get { return warnings; } }

        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected TransactionBase(string tranCode, TransactionContext context)
        {
            TransactionCode = tranCode;
            Context = context;
            context.TransactionCode = tranCode;
            db = context.Database;
            warnings = context.Warnings;
        }

        protected TransactionBase(string tranCode, string userName)
        {
            TransactionCode = tranCode;
            Context = TransactionManager.CreateContext(userName);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TransactionCode:{0} {1}", TransactionCode, Environment.NewLine);
            sb.AppendFormat("RequestMessage:{0} {1}", request.ToString() ?? "null", Environment.NewLine);
            sb.AppendFormat("ResponseMessage:{0} {1}", response.ToString() ?? "null", Environment.NewLine);
            return sb.ToString();
        }

        public long TranactionObjectId
        {
            get
            {
                long result = -1;
                if (Context.TransactionObject != null)
                    if(Context.TransactionObject is DbEntityBase)
                    result = ((DbEntityBase)Context.TransactionObject).Id;
                return result;
            }
        }

        public ResponseMessage Execute(RequestMessage requestMessage)
        {
            request = requestMessage;
            extendedData = request.MessageDataExtension;
            response = new ResponseMessage();
            ValidateRequest();
            switch (request.MessageHeader.ActionType)
            {
                case ActionType.Delete:
                    Delete();
                    break;
                case ActionType.Get:
                    Get();
                    break;
                case ActionType.List:
                    List();
                    break;
                case ActionType.New:
                    New();
                    break;
                case ActionType.Update :
                    Update();
                    break;
                case ActionType.ExportQb:
                    ExportQb();
                    break;
                case ActionType.ImportQb:
                    ImportQb();
                    break;
                case ActionType.Erase:
                    Erase();
                    break;
                case ActionType.Undelete:
                    Undelete();
                    break;
            }
            response.ResultMessage = responseMessage;
            response.Warnings = Warnings;
            SetGridDisplay();
            return response;
        }

        protected void SetGridDisplay()
        {

            if (this.response.TransactionResult is List<ITransactionEntity>)
            {
                List<ITransactionEntity> lst = (List<ITransactionEntity>)response.TransactionResult;
                if(lst.Count > 0)
                {
                    List<Dictionary<string, object>> gcd = new List<Dictionary<string, object>>();
                    ITransactionEntity entity = lst[0];

                    entity.GetType().GetProperties().ToList()
                        .Where(p => p.GetCustomAttributes(typeof(DisplayProperty), true).Any())
                        .ToList().ForEach(delegate (PropertyInfo pi)
                        {
                            DisplayProperty dp = pi.GetCustomAttribute<DisplayProperty>();
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            dic.Add("text", dp.Text);
                            dic.Add("dataField", dp.DataField);
                            dic.Add("sort", dp.Sort);
                            dic.Add("columnOrder", dp.DisplayOrder);
                            gcd.Add(dic);
                        });
                    ;
                    response.GridDisplayMembers = gcd.OrderBy(d => Convert.ToInt32(d["columnOrder"])).ToList();
                }

            }

        }

        protected abstract void Get();
        protected abstract void List();
        protected abstract void New();
        protected abstract void Update();
        protected abstract void Delete();

        protected virtual void ImportQb() { }
        protected virtual void ExportQb() { }
        protected virtual void DeserializeLog(byte[] logData) { }
        protected virtual void Undelete() { }
        protected virtual void Erase() { }

        public virtual void Deserialize(JToken token) { }

        public virtual void RefreshCache(ActionType at) { }

        protected virtual long ValidateEntityIdFromDataExtension()
        {
            long id = request.ObjectId;
            if (id == -1)
                throw new Exception(string.Format("Invalid entity id `{0}`", extendedData[MessageDataExtensionKeys.ID]));

            return id;
        }


        protected virtual void ValidateRequest()
        {
            switch (request.MessageHeader.ActionType)
            {
                case ActionType.Undelete:
                case ActionType.Erase:
                case ActionType.Get:
                case ActionType.Delete:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.ID))
                        throw new Exception(string.Format("MessageDataExtension does not contain key {0}", MessageDataExtensionKeys.ID));
                    break;
                case ActionType.List:
                    break;
                case ActionType.ImportQb:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.IMPORT_TYPE))
                        throw new Exception(string.Format("MessageDataExtension does not contain key {0}", MessageDataExtensionKeys.IMPORT_TYPE));
                    break;
                case ActionType.ExportQb:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.EXPORT_TYPE))
                        throw new Exception(string.Format("MessageDataExtension does not contain key {0}", MessageDataExtensionKeys.EXPORT_TYPE));
                    break;
                case ActionType.New:
                case ActionType.Update:
                    if (request.TransactionEntityList.Count == 0)
                        throw new Exception("TransactionEntityList list empty");
                    break;
            }
        }

        public void Dispose()
        {
            if (qbAgent != null)
                qbAgent.Dispose();
        }
    }
}
