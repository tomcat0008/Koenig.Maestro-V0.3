using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class CustomerProductUnit : DbEntityBase
    {
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroProduct Product { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroCustomer Customer { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroUnit Unit { get; set; }


        public string UnitTypeName { get { return this.Unit.UnitType.Name; } }
        public long UnitTypeId { get { return this.Unit.UnitType.Id; } }
        [DisplayProperty(Text = "Unit", DataField = "UnitName", Sort = true, DisplayOrder = 30)]
        public string UnitName { get { return this.Unit.Name; } }
        public long UnitId { get { return this.Unit.Id; } }
        public long CustomerId { get { return this.Customer.Id; } }
        [DisplayProperty(Text = "Customer", DataField = "CustomerName", Sort = true, DisplayOrder = 10)]
        public string CustomerName { get { return this.Customer.Name; } }

        public long ProductId { get { return this.Product.Id; } }
        [DisplayProperty(Text = "Product", DataField = "ProductName", Sort = true, DisplayOrder = 20)]
        public string ProductName { get { return this.Product.Name; } }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ",Product: `{0}`, Customer: `{1}`, Unit: `{2}`", Product.Name, Customer.Name, Unit.Name);
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
