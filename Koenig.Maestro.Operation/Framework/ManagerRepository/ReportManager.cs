using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class ReportManager : ManagerBase
    {
        public ReportManager(TransactionContext context) : base(context) { }

        public void RunReport()
        {
            string reportCode = context.Bag[MessageDataExtensionKeys.REPORT_CODE].ToString();

            ReportDefinition reportDef = ReportDefinitionCache.Instance.GetByReportCode(reportCode);

            DataSet reportData = LoadReportData(reportDef);

            ReportBase report = null;

            switch (reportDef.ReportType)
            {
                case "XLS":
                    report = new ExcelReport(reportDef, reportData);
                    break;
                case "CSV":
                    report = new CsvReport(reportDef, reportData);
                    break;
                case "CUSTOM":

                    break;
            }

            

            report.Render();

        }

        DataSet LoadReportData(ReportDefinition reportDef)
        {
            SpCall call = new SpCall(reportDef.ProcedureName);

            return context.Database.ExecuteDataSet(call);

        }



    }
}
