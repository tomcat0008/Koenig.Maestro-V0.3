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
        [DisplayProperty(Text = "Product Name", DataField = "Name",Sort = true, DisplayOrder = 10)]
        [JsonConversionTarget]
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayProperty(Text = "Quickbooks Product Id", DataField = "QuickBooksProductId",Sort = true, DisplayOrder = 20)]
        public string QuickBooksProductId { get; set; }

        [DisplayProperty(Text = "Price", DataField = "Price",Sort = true, DisplayOrder = 30)]
        public decimal Price { get; set; }
        public int MinimumOrderQuantity { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroUnitType UnitType { get; set; }

        [JsonConversionTarget]
        public long GroupId { get { return this.ProductGroup == null ? 0 : this.ProductGroup.Id; } }
        [JsonConversionTarget]
        public string GroupDescription { get { return this.ProductGroup == null ? string.Empty: this.ProductGroup.GroupDescription; } }

        [DisplayProperty(Text = "Product Group", DataField = "GroupName", Sort = true, DisplayOrder = 40)]
        public string GroupName { get { return this.ProductGroup == null ? string.Empty : this.ProductGroup.Name; } }

        /*
        [JsonConverter(typeof(EntityJsonConverter))]
        public List<QuickBooksProductMapDef> QbProductMaps { get; set; }*/

        [JsonConversionTarget]
        public string UnitTypeName { get { return UnitType == null ? string.Empty: UnitType.Name; } }
        [JsonConversionTarget]
        public long UnitTypeId { get { return UnitType == null ? 0 : UnitType.Id; } }
        [JsonConversionTarget]
        public bool UnitTypeCanHaveUnits { get { return this.UnitType == null ? false : this.UnitType.CanHaveUnits; } }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ",Name: `{0}`, Price: `{1}`, UnitType: {2}", Name, Price, UnitType.Id );

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroProductGroup ProductGroup{get;set;}

        public decimal CostBase { get; set; }
    }
}
