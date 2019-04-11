using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class QuickBooksProductMapDef : DbEntityBase, ITransactionEntity
    {
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroProduct Product { get; set; }
        public string QuickBooksCode { get; set; }
        [DisplayProperty(Text = "Quickbooks List Id", DataField = "QuickBooksListId", Sort = true, DisplayOrder = 20)]
        public string QuickBooksListId { get; set; }
        public string QuickBooksParentCode { get; set; }
        public string QuickBooksParentListId { get; set; }
        public string QuickBooksDescription { get; set; }
        [DisplayProperty(Text = "Price", DataField = "Price", Sort = true, DisplayOrder = 30)]
        public decimal Price { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroUnit Unit { get; set; }

        public string Label { get; set; }

        public bool UnitTypeCanHaveUnits { get { return this.Product == null ? false : this.Product.UnitTypeCanHaveUnits; } }
        public long ProductGroupId { get { return this.Product == null ? 0 : this.Product.GroupId; } }
        [DisplayProperty(Text = "Quickbooks full name", DataField = "FullName", Sort = true, DisplayOrder = 10)]
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(QuickBooksParentCode))
                    return QuickBooksCode;
                else
                    return QuickBooksParentCode + ":" + QuickBooksCode;
            }
        }

        public long ProductId
        {
            get
            {
                return this.Product.Id;
            }
        }


        public long UnitId { get { return this.Unit.Id; } }
        [DisplayProperty(Text = "Unit", DataField = "UnitName", Sort = true, DisplayOrder = 50)]
        public string UnitName { get { return this.Unit.Name; } }

        [DisplayProperty(Text = "Produce Name", DataField = "ProductName", Sort = true, DisplayOrder = 40)]
        public string ProductName { get { return this.Product.Name; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Product:`{0}`, QbCode;`{1}`, QbParent:`{2}`, QbDesc;`{3}`, Unit:`{4}`",
                new object[] { Product.Id, QuickBooksCode, QuickBooksParentCode, QuickBooksDescription, Unit.Id });

            return sb.ToString();
        }

        public long UnitTypeId { get { return this.Product == null ? 0 : (this.Product.UnitType == null ? 0 : this.Product.UnitType.Id); } }
        public string UnitTypeName { get { return this.Product == null ? string.Empty : (this.Product.UnitType ==null ? string.Empty :this.Product.UnitType.Name); } }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
