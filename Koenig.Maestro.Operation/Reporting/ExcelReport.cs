using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Koenig.Maestro.Entity;
using OfficeOpenXml;

namespace Koenig.Maestro.Operation.Reporting
{
    internal class ExcelReport:ReportBase
    {

        public ExcelReport(ReportDefinition reportDefinition, DataSet reportData) : base(reportDefinition, reportData) { }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void Export()
        {
            throw new NotImplementedException();
        }

        protected virtual void RenderWithTemplate()
        {
            using (MemoryStream ms = new MemoryStream(reportDefinition.Template))
            {
                using (ExcelPackage p = new ExcelPackage(ms))
                {



                }
            }
        }

        public override void Render()
        {

            if (reportDefinition.Template != null)
                RenderWithTemplate();

        }
    }
}
