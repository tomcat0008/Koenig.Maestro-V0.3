using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{

    [Serializable]
    public class MaestroCustomer : DbEntityBase, ITransactionEntity
    {
        [JsonConversionTarget]
        [DisplayProperty(Text = "Name", DataField ="Name", Sort =true, DisplayOrder = 10)]
        public string Name { get; set; }
        public string Title { get; set; }
        //[DisplayProperty(Name = "Address", Selector = "Address",Sortable = true, Center = false)]
        public string Address { get; set; }
        public string Phone { get; set; }
        //[DisplayProperty(Text = "Email", DataField = "Email",Sort = true, DisplayOrder = 4)]
        public string Email { get; set; }
        //[DisplayProperty(Text = "QB Code", DataField = "QuickBooksId",Sort = true, DisplayOrder = 3)]
        public string QuickBooksId { get; set; }
        [DisplayProperty(Text = "QB Company", DataField = "QuickBoosCompany",Sort = true, DisplayOrder = 20)]
        public string QuickBoosCompany { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroRegion Region { get; set; }
        public string DefaultPaymentType { get; set; }
        [DisplayProperty(Text = "Customer Group", DataField = "CustomerGroup", Sort = true, DisplayOrder = 30)]
        public string CustomerGroup { get; set; }
        [DisplayProperty(Text = "Report Group", DataField = "ReportGroup", Sort = true, DisplayOrder = 40)]
        public string ReportGroup { get; set; }
        public long RegionId { get { return Region.Id; } }
        public string RegionName { get { return Region.Name; } }



        public List<CustomerAddress> AddressList
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format(TostringTemplate + ",Name: `{0}`, Title: `{1}`, Region: {2}", Name, Title, Region.Id);

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
