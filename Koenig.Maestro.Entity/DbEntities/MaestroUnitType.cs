using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroUnitType : DbEntityBase, ITransactionEntity
    {
        [JsonConversionTarget]
        [DisplayProperty(Text = "Unit Type Name", DataField = "Name",Sort = true, DisplayOrder = 1)]
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConversionTarget]
        [DisplayProperty(Text = "Have Sub-units", DataField = "CanHaveUnits",Sort = true, DisplayOrder = 2)]
        public bool CanHaveUnits { get; set; }
        public override string ToString()
        {
            return string.Format(TostringTemplate + ", Name: `{0}`, CanHaveUnits: `{1}`", Name, CanHaveUnits);

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
