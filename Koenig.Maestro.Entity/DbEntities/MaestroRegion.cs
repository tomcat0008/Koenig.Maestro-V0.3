using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{

    [Serializable]
    public class MaestroRegion : DbEntityBase, ITransactionEntity
    {
        [JsonConversionTarget]
        [DisplayProperty(Text = "Region Name", DataField = "Name",Sort = true, DisplayOrder = 1)]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayProperty(Text = "Postal Code", DataField = "PostalCode",Sort = true, DisplayOrder = 2)]
        public string PostalCode { get; set; }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ", Name: `{0}`, PostalCode: `{1}`", Name, PostalCode);

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
