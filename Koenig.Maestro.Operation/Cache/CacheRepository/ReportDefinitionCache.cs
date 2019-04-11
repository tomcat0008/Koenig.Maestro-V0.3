using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class ReportDefinitionCache : DbLoadCache<long, ReportDefinition>
    {
        static ReportDefinitionCache instance = null;

        ReportDefinitionCache() : base("REPORT_CACHE", "RPT.REPORT_DEFINITION_SELECT_ALL")
        {
        }

        public static ReportDefinitionCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReportDefinitionCache();
                return instance;
            }
        }

        public ReportDefinition GetByReportCode(string reportCode)
        {
            ReportDefinition result = Values.ToList().Find(r => r.ReportCode.Equals(reportCode));
            return result;
        }


        protected override Tuple<long, ReportDefinition> GetItem(SqlReader reader)
        {
            ReportDefinition t = new ReportDefinition();
            t.Id = reader.GetInt64("ID");


            t.ReportCode = reader.GetString("REPORT_CODE");
            t.Description = reader.GetString("REPORT_DESCRIPTION");
            t.FileName = reader.GetString("REPORT_FILE_NAME");
            t.MetaDefinition = reader.GetString("META_DEFINITION");
            t.ProcedureName = reader.GetString("REPORT_PROCEDURE_NAME");
            t.Template = reader.GetByteArray("TEMPLATE");
            t.ReportType = reader.GetString("REPORT_TYPE");

            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, ReportDefinition>(t.Id, t);

        }
    }

}
