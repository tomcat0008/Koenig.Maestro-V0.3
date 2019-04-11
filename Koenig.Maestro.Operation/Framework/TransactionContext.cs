using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework
{
    public class TransactionContext
    {
        public Guid TransactionId { get; set; }
        public Database Database { get; set; }
        public object TransactionObject { get; set; }
        public string UserName { get; set; }
        public string TransactionCode { get; set; }
        public RequestMessage RequestMessage {get;set;}
        public List<string> Warnings { get; set; }

        public Dictionary<string, object> Bag { get; set; }

    }
}
