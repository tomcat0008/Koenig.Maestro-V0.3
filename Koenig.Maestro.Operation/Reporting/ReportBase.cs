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

namespace Koenig.Maestro.Operation.Reporting
{
    internal abstract class ReportBase : DbEntityBase
    {
        protected ReportDefinition reportDefinition;
        protected DataSet reportData;

        protected ReportBase(ReportDefinition reportDefinition, DataSet reportData)
        {
            this.reportData = reportData;
            this.reportDefinition = reportDefinition;
        }

        

        public abstract void Render();

        public abstract void Export();

    }
}
