using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using OfficeOpenXml;

namespace Koenig.Maestro.Operation.Reporting
{
    public abstract class ExcelReportBase:ReportBase
    {

        protected ExcelWorkbook workbook;

        protected ExcelReportBase(TransactionContext context) : base(context) { }

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

        protected virtual void RenderWithoutTemplate()
        {

        }

        public override void Render()
        {
            LoadData();
            if (reportDefinition.Template != null)
                RenderWithTemplate();
            else
                RenderWithoutTemplate();


        }
    }
}
