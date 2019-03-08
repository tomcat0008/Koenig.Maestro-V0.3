using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class GridColumnDefinition
    {
        public string name { get; set; }
        public bool sortable { get; set; }
        public string selector { get; set; }
        public bool right { get; set; }
        public bool center { get; set; }
    }
}
