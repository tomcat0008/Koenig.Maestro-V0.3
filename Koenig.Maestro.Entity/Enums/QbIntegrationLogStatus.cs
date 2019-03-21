using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity.Enums
{
    public struct QbIntegrationLogStatus
    {
        public const string WAITING = "WAITING";
        public const string ERROR = "ERROR";
        public const string OK = "OK";
        public const string UNKNOWN = "UNKNOWN";
        public const string REVOKED = "REVOKED";
        public const string CANCELLED = "CANCELLED";
    }
}
