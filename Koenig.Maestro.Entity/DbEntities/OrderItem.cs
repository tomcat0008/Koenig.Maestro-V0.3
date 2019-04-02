using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class OrderItem : DbEntityBase, ITransactionEntity
    {
        public long OrderId { get; set; }
        
        public MaestroProduct Product { get; set; }

        [DisplayProperty(Text = "Quantity", DataField = "Quantity",Sort = true, DisplayOrder = 20)]
        public int Quantity { get; set; }
        
        public MaestroUnit Unit { get;set; }


        public long UnitId { get { return this.Unit.Id; } }

        public long MapId { get { return this.QbProductMap.Id; } }

        [JsonConverter(typeof(EntityJsonConverter))]
        public QuickBooksProductMapDef QbProductMap { get; set; }

        public decimal Price { get; set; }

        [DisplayProperty(Text = "Product", DataField = "ProductName", Sort = true, DisplayOrder = 10)]
        public string ProductName
        {
            get { return Product.Name; }
        }

        [DisplayProperty(Text = "Unit", DataField = "UnitName", Sort = true, DisplayOrder = 40)]
        public string UnitName { get { return Unit.Name; } }

        [DisplayProperty(Text = "Price", DataField = "ItemPrice", Sort = true, DisplayOrder = 30)]
        public decimal ItemPrice { get { return QbProductMap.Price; } }

        [DisplayProperty(Text = "Amount", DataField = "Amount", Sort = true, DisplayOrder = 50)]
        public decimal Amount { get; set; }


        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ", Product:{0}, Unit:{1}, Quantity:{2}", Product.Id, Unit.Id, Quantity);
        }
    }
}
