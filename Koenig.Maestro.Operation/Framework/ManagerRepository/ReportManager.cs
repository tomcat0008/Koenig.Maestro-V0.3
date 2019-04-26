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

            MaestroReportDefinition reportDef = ReportDefinitionCache.Instance.GetByReportCode(reportCode);

            context.Bag.Add("REPORT_DEF", reportDef);

            Type type = Type.GetType(reportDef.CodeBase);
            if (type == null)
                throw new Exception(string.Format("Codebase `{0}` of report definition `{1}` could not be found", reportDef.CodeBase, reportCode));

            ReportBase report = (ReportBase)Activator.CreateInstance(type, new object[] { context });

            report.Render();

        }



    }
}
