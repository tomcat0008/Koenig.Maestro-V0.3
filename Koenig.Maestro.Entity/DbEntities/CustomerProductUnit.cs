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

        public override string ToString()
        {
            return string.Format(TostringTemplate + ",Product: `{0}`, Customer: `{1}`, Unit: `{2}`", Product.Name, Customer.Name, Unit.Name);
        }
    }
}
