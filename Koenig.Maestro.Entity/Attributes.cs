using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayProperty : Attribute
    {
        public int DisplayOrder { get; set; }
        public string Text { get; set; }
        public bool Sort { get; set; }
        public string DataField { get; set; }
        public string Align { get; set; }
        public bool Filter { get; set; }
        public string ColumnWidth { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class JsonConversionTarget : Attribute
    {

    }

}
