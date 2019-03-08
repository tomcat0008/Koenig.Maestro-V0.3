using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Koenig.Maestro.Web.Models
{
    public class MaestroViewRequestBase
    {
        public string TransactionCode { get; set; }
        public string AgentInfo { get; set; }
        public DateTime ClientDate { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; }

    }
}