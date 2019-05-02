using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroTaskSchedule : DbEntityBase, ITransactionEntity
    {

        [JsonConversionTarget]
        [DisplayProperty(Text = "Task Name", DataField = "Name", Sort = true, DisplayOrder = 10)]
        public string Name { get; set; }

        public string CodeBase { get; set; }

        [DisplayProperty(Text = "Method", DataField = "Method", Sort = true, DisplayOrder = 30)]
        public string MethodName { get; set; }

        [DisplayProperty(Text = "Class Name", DataField = "ClassName", Sort = true, DisplayOrder = 20)]
        public string ClassName { get; set; }

        [DisplayProperty(Text = "Intervall", DataField = "Intervall", Sort = true, DisplayOrder = 40)]
        public int Intervall { get; set; }

        [DisplayProperty(Text = "Intervall Unit", DataField = "IntervallUnit", Sort = true, DisplayOrder = 50)]
        public string IntervallUnit { get; set; }


        public override string ToString()
        {
            return string.Format(TostringTemplate + ", Name: `{0}`, Intervall: `{1}`, IntervallUnit: `{2}`", Name, Intervall, IntervallUnit);

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
