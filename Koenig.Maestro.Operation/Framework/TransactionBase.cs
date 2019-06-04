using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.DbEntities;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Koenig.Maestro.Operation.Messaging;
using Koenig.Maestro.Operation.QuickBooks;
using Koenig.Maestro.Operation.Utility;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Koenig.Maestro.Operation.Framework
{

    public delegate void TransactionProgressEventHandler(object sender, TransactionProgressEventArgs e);

    internal abstract class TransactionBase : IDisposable
    {

        public event TransactionProgressEventHandler TransactionProgress;
        public bool IsProgressing { get; protected set; }
        protected ITransactionEntity MainEntitySample;
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


        protected void OnTransactionProgress(TransactionProgressEventArgs e)
        {
            if (TransactionProgress != null)
                this.TransactionProgress(this, e);
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
                case ActionType.Clone:
                    Clone();
                    break;
                case ActionType.Backup:
                    BackUp();
                    break;
                case ActionType.Report:
                    Report();
                    break;
                case ActionType.Merge:
                    Merge();
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

                List<Dictionary<string, object>> gcd = new List<Dictionary<string, object>>();
                ITransactionEntity entity = this.MainEntitySample;

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
                        dic.Add("align", dp.Align ?? "right");

                        if (dp.Filter)
                            dic.Add("filterType", pi.PropertyType.Name);

                        if (!string.IsNullOrWhiteSpace(dp.ColumnWidth))
                            dic.Add("columnWidth", dp.ColumnWidth);

                        gcd.Add(dic);
                    });
                ;
                response.GridDisplayMembers = gcd.OrderBy(d => Convert.ToInt32(d["columnOrder"])).ToList();
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
        protected virtual void Clone() { }
        protected virtual void BackUp() { }
        protected virtual void Report() { }
        protected virtual void Merge() { }

        public virtual void Deserialize(JToken token) { }

        public virtual void RefreshCache(ActionType at) { }

        protected void ExtractTransactionCriteria()
        {
            ExtractDateCriteria();

            long customerId = -1, batchID = 0;
            string status = string.Empty;
            string dateField = OrderRequestType.ListByOrderDate.ToString();
            bool notIntegrated = false;
            string reportCode = string.Empty;
            bool saveFile = false;
            string listCode = string.Empty;
            string invoiceGroup = string.Empty;

            if (extendedData.ContainsKey(MessageDataExtensionKeys.CUSTOMER_ID))
                long.TryParse(extendedData[MessageDataExtensionKeys.CUSTOMER_ID], out customerId);
            if (extendedData.ContainsKey(MessageDataExtensionKeys.STATUS))
                status = extendedData[MessageDataExtensionKeys.STATUS];
            if (extendedData.ContainsKey(MessageDataExtensionKeys.REQUEST_TYPE))
                dateField = extendedData[MessageDataExtensionKeys.REQUEST_TYPE];

            if (extendedData.ContainsKey(MessageDataExtensionKeys.BATCH_ID))
                long.TryParse(extendedData[MessageDataExtensionKeys.BATCH_ID], out batchID);

            if (extendedData.ContainsKey(MessageDataExtensionKeys.NOT_INTEGRATED))
                bool.TryParse(extendedData[MessageDataExtensionKeys.NOT_INTEGRATED], out notIntegrated);

            if (extendedData.ContainsKey(MessageDataExtensionKeys.SAVE_FILE))
                bool.TryParse(extendedData[MessageDataExtensionKeys.SAVE_FILE], out saveFile);
            
            if (extendedData.ContainsKey(MessageDataExtensionKeys.REPORT_CODE))
                reportCode = extendedData[MessageDataExtensionKeys.REPORT_CODE];

            if (extendedData.ContainsKey(MessageDataExtensionKeys.LIST_CODE))
                listCode = extendedData[MessageDataExtensionKeys.LIST_CODE];

            if (extendedData.ContainsKey(MessageDataExtensionKeys.INVOICE_GROUP))
                invoiceGroup = extendedData[MessageDataExtensionKeys.INVOICE_GROUP];

            Context.Bag.Add(MessageDataExtensionKeys.SAVE_FILE, saveFile);
            Context.Bag.Add(MessageDataExtensionKeys.REPORT_CODE, reportCode);
            Context.Bag.Add(MessageDataExtensionKeys.CUSTOMER_ID, customerId);
            Context.Bag.Add(MessageDataExtensionKeys.BATCH_ID, batchID);
            Context.Bag.Add(MessageDataExtensionKeys.STATUS, status);
            Context.Bag.Add(MessageDataExtensionKeys.REQUEST_TYPE, dateField);
            Context.Bag.Add(MessageDataExtensionKeys.NOT_INTEGRATED, notIntegrated);
            Context.Bag.Add(MessageDataExtensionKeys.LIST_CODE, listCode);
            Context.Bag.Add(MessageDataExtensionKeys.INVOICE_GROUP, invoiceGroup);
            


        }

        void ExtractDateCriteria()
        {
            DateTime endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, 1).AddDays(-1);
            DateTime beginDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);


            if (extendedData.ContainsKey(MessageDataExtensionKeys.PERIOD))
            {
                DatePeriod period = EnumUtils.GetEnum<DatePeriod>(extendedData[MessageDataExtensionKeys.PERIOD]);
                switch (period)
                {

                    case DatePeriod.All:
                        beginDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                        endDate = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                        break;
                    case DatePeriod.Today:
                        beginDate = DateTime.Today;
                        endDate = DateTime.Now.AddDays(1);
                        break;
                    case DatePeriod.Week:
                        beginDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                        endDate = DateTime.Now.AddDays(1);
                        break;
                    case DatePeriod.Month:
                        beginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        endDate = beginDate.AddMonths(1);
                        break;
                    case DatePeriod.Year:
                        beginDate = new DateTime(DateTime.Now.Year, 1, 1);
                        endDate = beginDate.AddYears(1);
                        break;
                }

            }
            else
            {
                if (extendedData.ContainsKey(MessageDataExtensionKeys.BEGIN_DATE))
                    DateTime.TryParse(extendedData[MessageDataExtensionKeys.BEGIN_DATE], null, System.Globalization.DateTimeStyles.RoundtripKind, out beginDate);
                    //DateTime.TryParseExact(extendedData[MessageDataExtensionKeys.BEGIN_DATE], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out beginDate);
                if (extendedData.ContainsKey(MessageDataExtensionKeys.END_DATE))
                    DateTime.TryParse(extendedData[MessageDataExtensionKeys.END_DATE], null, System.Globalization.DateTimeStyles.RoundtripKind, out endDate);
                //DateTime.TryParseExact(extendedData[MessageDataExtensionKeys.END_DATE], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out endDate);
            }

            this.Context.Bag.Add(MessageDataExtensionKeys.BEGIN_DATE, beginDate);
            this.Context.Bag.Add(MessageDataExtensionKeys.END_DATE, endDate);

        }




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
                case ActionType.Report:
                    if (!extendedData.ContainsKey(MessageDataExtensionKeys.REPORT_CODE))
                        throw new Exception(string.Format("MessageDataExtension does not contain key {0}", MessageDataExtensionKeys.REPORT_CODE));
                    break;
            }
        }

        public virtual void Dispose()
        {
            if (qbAgent != null)
                qbAgent.Dispose();

        }
    }
}
