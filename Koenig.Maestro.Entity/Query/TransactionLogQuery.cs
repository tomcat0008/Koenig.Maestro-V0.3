using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity.Query
{
    [Serializable]
    public class TransactionLogQuery
    {
        public long Id { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public string TransactionCode { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
