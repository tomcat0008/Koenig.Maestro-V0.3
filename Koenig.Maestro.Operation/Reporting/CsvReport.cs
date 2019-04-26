using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
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
        public CsvReport(TransactionContext context) :base(context) {  }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void Export()
        {
            throw new NotImplementedException();
        }

        public override void LoadData()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }
    }
}
