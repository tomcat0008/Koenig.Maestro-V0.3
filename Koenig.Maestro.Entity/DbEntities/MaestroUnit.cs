using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroUnit : DbEntityBase, ITransactionEntity
    {
        [JsonConversionTarget]
        [DisplayProperty(Text ="Unit Name", DataField = "Name",Sort = true, DisplayOrder = 1)]
        public string Name { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroUnitType UnitType { get; set; }

        public string QuickBooksUnit { get; set; }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ", Name: `{0}`, UnitType: `{1}`", Name, UnitType.Id);

        }
    }
}
