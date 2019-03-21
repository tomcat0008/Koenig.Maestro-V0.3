using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroProductGroup : DbEntityBase, ITransactionEntity
    {
        [DisplayProperty(Text = "Group Name", DataField = "Name", Sort = true, DisplayOrder = 10)]
        [JsonConversionTarget]
        public string Name { get; set; }
        [DisplayProperty(Text = "Description", DataField = "GroupDescription", Sort = true, DisplayOrder = 20)]
        [JsonConversionTarget]
        public string GroupDescription { get; set; }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
