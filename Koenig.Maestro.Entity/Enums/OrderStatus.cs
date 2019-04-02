using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity.Enums
{
    public struct OrderStatus
    {
        public const string CREATED = "CR"; //created
        public const string INTEGRATED = "QB"; //done
        public const string ERROR = "ER"; //error
        public const string CANCELLED = "CC"; //cancelled
        public const string DELIVERED = "DL";
        public const string DELETED = "DE";
    }
}
