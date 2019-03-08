using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Koenig.Maestro.Operation.Cache
{
    [Serializable]
    [DebuggerDisplay("CacheId={CacheId}, Count{Count}")]
    public class CacheInfo
    {
        public string CacheId { get; set; }
        public int Count { get; set; }
        public DateTime ReloadTime { get; set; }
        public int ReloadDuration { get; set; }

        public CacheInfo()
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CacheInfo:GlobalCacheName:{0} {1}", CacheId, Environment.NewLine);
            sb.AppendFormat("CacheId:{0} {1}", CacheId, Environment.NewLine);
            sb.AppendFormat("Count:{0} {1}", Count, Environment.NewLine);
            sb.AppendFormat("ReloadTime:{0} {1}", ReloadTime, Environment.NewLine);
            sb.AppendFormat("ReloadDuration:{0} {1}", ReloadDuration, Environment.NewLine);
            return sb.ToString();
        }
    }
}
