using Koenig.Maestro.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using System.Data;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;

namespace Koenig.Maestro.Operation.Reporting
{
    public abstract class ReportBase : DbEntityBase
    {
        protected MaestroReportDefinition reportDefinition;
        protected DataSet reportData;
        protected TransactionContext context;
        protected ReportBase(TransactionContext context)
        {
            this.context = context;
            reportDefinition = (MaestroReportDefinition)context.Bag["REPORT_DEF"];
        }

        public abstract void LoadData();
        

        public abstract void Render();

        public abstract void Export();

    }
}
