using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity.DbEntities
{
    [Serializable]
    public class TransactionLog : DbEntityBase
    {
        [DisplayProperty(Text = "Transaction code", DataField = "TransactionCode",Sort = true, DisplayOrder = 1)]
        public string TransactionCode { get; set; }
        public byte[] Log { get; set; }
        public string LogJson { get; set; }
        [DisplayProperty(Text = "Status", DataField = "Status",Sort = true, DisplayOrder = 2)]
        public string Status { get; set; }
        [DisplayProperty(Text = "Log id", DataField = "LogObjectId",Sort = true, DisplayOrder = 3)]
        public long LogObjectId { get; set; }
        public Dictionary<string, string> DisplayProperties { get; set; }
        [DisplayProperty(Text = "Action", DataField = "ActionType",Sort = true, DisplayOrder = 4)]
        public string ActionType { get; set; }
        [DisplayProperty(Text = "Request type", DataField = "RequestType",Sort = true, DisplayOrder = 5)]
        public string RequestType { get; set; }
        [DisplayProperty(Text = "Duration", DataField = "Duration",Sort = true, DisplayOrder = 6)]
        public decimal Duration { get; set; }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ", TransactionCode: `{0}`, ActionType: `{1}`", TransactionCode, ActionType);

        }
    }
}
