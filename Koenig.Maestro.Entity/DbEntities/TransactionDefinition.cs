using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class TransactionDefinition : DbEntityBase, ITransactionEntity
    {
        [DisplayProperty(Text = "Transaction code", DataField = "TranCode",Sort = true, DisplayOrder = 1)]
        public string TranCode { get; set; }
        [DisplayProperty(Text = "Transaction description", DataField = "TranDescription",Sort = true, DisplayOrder = 2)]
        public string TranDescription { get; set; }
        public bool IsCancellable { get; set; }
        public int MenuId { get; set; }
        public string GroupCode { get; set; }
        public bool IsQbRelated { get; set; }
        [DisplayProperty(Text = "Code base", DataField = "CodeBase",Sort = true, DisplayOrder = 3)]
        public string CodeBase { get; set; }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ", TranCode: `{0}`, CodeBase: `{1}`", TranCode, CodeBase);
        }


        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
