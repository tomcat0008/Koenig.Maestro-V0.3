using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroReportDefinition : DbEntityBase, ITransactionEntity
    {
        [JsonConversionTarget]
        [DisplayProperty(Text = "Report Code", DataField = "ReportCode", Sort = true, DisplayOrder = 10)]
        public string ReportCode { get; set; }
        public string Description { get; set; }

        [DisplayProperty(Text = "Procedure", DataField = "ProcedureName", Sort = true, DisplayOrder = 20)]
        public string ProcedureName { get; set; }

        [DisplayProperty(Text = "Transaction", DataField = "TransactionCode", Sort = true, DisplayOrder = 50)]
        public string TransactionCode { get; set; }

        public byte[] Template { get; set; }

        public string MetaDefinition { get; set; }

        [DisplayProperty(Text = "File Name", DataField = "FileName", Sort = true, DisplayOrder = 30)]
        public string FileName { get; set; }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ", ReportCode: `{0}`, ProcedureName: `{1}`", ReportCode, ProcedureName);

        }

        [DisplayProperty(Text = "Report Type", DataField = "ReportType", Sort = true, DisplayOrder = 40)]
        public string ReportType { get; set; }
        

        public string CodeBase { get; set; }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }

}
