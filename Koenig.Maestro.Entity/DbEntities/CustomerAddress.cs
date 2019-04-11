using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class CustomerAddress : DbEntityBase, ITransactionEntity
    {

        public long CustomerId { get; set; }
        public string AddressType { get; set; }
        public string AddressCode { get; set; }
        public string QbName { get; set; }
        public string QbListID { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }
        public string Line5 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
