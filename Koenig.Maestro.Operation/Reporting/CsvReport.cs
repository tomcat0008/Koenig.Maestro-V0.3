using Koenig.Maestro.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Reporting
{
    internal class CsvReport : ReportBase
    {
        public CsvReport(ReportDefinition reportDefinition, DataSet reportData) :base(reportDefinition, reportData) {  }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void Export()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }
    }
}
