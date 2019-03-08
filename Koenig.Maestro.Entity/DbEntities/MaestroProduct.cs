using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class MaestroProduct : DbEntityBase, ITransactionEntity
    {
        [DisplayProperty(Text = "Product Name", DataField = "Name",Sort = true, DisplayOrder = 1)]
        [JsonConversionTarget]
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayProperty(Text = "Quickbooks Product Id", DataField = "QuickBooksProductId",Sort = true, DisplayOrder = 2)]
        public string QuickBooksProductId { get; set; }

        [DisplayProperty(Text = "Price", DataField = "Price",Sort = true, DisplayOrder = 3)]
        public decimal Price { get; set; }
        public int MinimumOrderQuantity { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroUnitType UnitType { get; set; }
        public long GroupId { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public List<QuickBooksProductMapDef> QbProductMaps { get; set; }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ",Name: `{0}`, Price: `{1}`, UnitType: {2}", Name, Price, UnitType.Id );

        }

    }
}
