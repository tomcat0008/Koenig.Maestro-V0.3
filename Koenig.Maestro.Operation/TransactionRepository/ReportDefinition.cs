using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Newtonsoft.Json.Linq;


namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class ReportDefinition : TransactionBase
    {
        public ReportDefinition(TransactionContext context) : base("REPORT_DEFINITION", context)
        {
            this.IsProgressing = false;
            this.MainEntitySample = new MaestroReportDefinition();
        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            try
            {
                Context.TransactionObject = ReportDefinitionCache.Instance[id];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Report Definition id {0} could not be found", id), ex);
            }
            SpCall spCall = new SpCall("RPT.REPORT_DEFINITION_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(spCall);
        }

        protected override void DeserializeLog(byte[] logData)
        {
            throw new NotImplementedException();
        }

        protected override void ExportQb()
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(JToken token)
        {
            MaestroReportDefinition resultObj = new MaestroReportDefinition();

            JObject entityObj = JObject.Parse(token.ToString());

            resultObj.Id = entityObj["Id"].ToObject<long>();
            resultObj.Description = entityObj["Name"].ToString();

            resultObj.ReportCode = entityObj["ReportCode"].ToString();
            resultObj.ProcedureName = entityObj["ProcedureName"].ToString();
            resultObj.MetaDefinition = entityObj["MetaDefinition"].ToString();

            resultObj.FileName = entityObj["FileName"].ToString();
            
            resultObj.ReportType = entityObj["ReportType"].ToString();
            
            resultObj.CodeBase = entityObj["CodeBase"].ToString();

            resultObj.TransactionCode = entityObj["TransactionCode"].ToString();

            Context.TransactionObject = resultObj;
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = ReportDefinitionCache.Instance[id];
        }

        protected override void ImportQb()
        {
            throw new NotImplementedException();
        }

        protected override void List()
        {
            response.TransactionResult = ReportDefinitionCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            MaestroReportDefinition rdef = (MaestroReportDefinition)request.TransactionEntityList[0];

            SpCall call = new SpCall("RPT.REPORT_DEFINITION_INSERT");

            call.SetVarchar("@REPORT_CODE", rdef.ReportCode);
            call.SetVarchar("@REPORT_DESCRIPTION", rdef.Description);
            call.SetBinary("@TEMPLATE", rdef.Template);
            call.SetVarchar("@META_DEFINITION", rdef.MetaDefinition);
            call.SetVarchar("@REPORT_FILE_NAME", rdef.FileName);
            call.SetVarchar("@REPORT_PROCEDURE_NAME", rdef.ProcedureName);
            call.SetVarchar("@REPORT_TYPE", rdef.ReportType);
            call.SetVarchar("@CODE_BASE", rdef.CodeBase);

            call.SetDateTime("@CREATE_DATE", DateTime.Now);
            call.SetVarchar("@CREATE_USER", Context.UserName);

            call.SetVarchar("@TRAN_CODE", rdef.TransactionCode);

            rdef.Id = db.ExecuteScalar<long>(call);

            response.TransactionResult = rdef;
        }

        protected override void Update()
        {
            MaestroReportDefinition rdef = (MaestroReportDefinition)request.TransactionEntityList[0];

            SpCall call = new SpCall("RPT.REPORT_DEFINITION_UPDATE");
            call.SetBigInt("@ID", rdef.Id);
            call.SetVarchar("@REPORT_CODE", rdef.ReportCode);
            call.SetVarchar("@REPORT_DESCRIPTION", rdef.Description);
            call.SetBinary("@TEMPLATE", rdef.Template);
            call.SetVarchar("@META_DEFINITION", rdef.MetaDefinition);
            call.SetVarchar("@REPORT_FILE_NAME", rdef.FileName);
            call.SetVarchar("@REPORT_PROCEDURE_NAME", rdef.ProcedureName);
            call.SetVarchar("@REPORT_TYPE", rdef.ReportType);
            call.SetVarchar("@CODE_BASE", rdef.CodeBase);

            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", Context.UserName);
            call.SetVarchar("@TRAN_CODE", rdef.TransactionCode);

            db.ExecuteNonQuery(call);
            Context.TransactionObject = rdef;
            ReportDefinitionCache.Instance.Reload(true);
        }

        public override void RefreshCache(ActionType at)
        {
            ReportDefinitionCache.Instance.Reload(true);
        }

        protected override void Undelete()
        {
            throw new NotImplementedException();
        }

        protected override void Erase()
        {
            throw new NotImplementedException();
        }


    }
}
