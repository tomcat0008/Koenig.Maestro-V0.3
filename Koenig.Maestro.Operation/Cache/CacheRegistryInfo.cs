using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Koenig.Maestro.Operation.Cache
{
    [Serializable]
    [DebuggerDisplay("AHostName={HostName}, CacheId={CacheId}")]
    public class CacheRegistryInfo
    {
        public string HostName { get; set; }
        public string CacheId { get; set; }
    }
}
