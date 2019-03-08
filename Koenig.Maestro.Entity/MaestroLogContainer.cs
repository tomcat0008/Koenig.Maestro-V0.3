using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroLogContainer
    {
        public object Request { get; set; }
        public object Response { get; set; }
    }
}
