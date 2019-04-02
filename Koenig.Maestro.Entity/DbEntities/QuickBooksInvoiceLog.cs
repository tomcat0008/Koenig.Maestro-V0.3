using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class QuickBooksInvoiceLog : DbEntityBase
    {
        [DisplayProperty(Text = "Order Id", DataField = "OrderId",Sort = true, DisplayOrder = 1)]
        public long OrderId { get; set; }
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroCustomer Customer { get; set; }
        [DisplayProperty(Text = "Integration status", DataField = "IntegrationStatus",Sort = true, DisplayOrder = 25, Filter =true)]
        public string IntegrationStatus { get; set; }
        [DisplayProperty(Text = "Integration date", DataField = "IntegrationDate",Sort = true, DisplayOrder = 30)]
        public DateTime IntegrationDate { get; set; }
        [DisplayProperty(Text = "Batch Id", DataField = "BatchId",Sort = true, DisplayOrder = 40)]
        public long BatchId { get; set; }
        [DisplayProperty(Text = "Quickbooks Invoice Id", DataField = "QuickBooksInvoiceId",Sort = true, DisplayOrder = 50, Filter =true)]
        public string QuickBooksInvoiceId { get; set; }
        [DisplayProperty(Text = "Error log", DataField = "ErrorLog",Sort = true, DisplayOrder = 60)]
        public string ErrorLog { get; set; }
        public string QuickBooksTxnId { get; set; }

        public string QuickBooksCustomerId { get; set; }
        public long CustomerId { get { return this.Customer.Id; } }
        [DisplayProperty(Text = "Customer", DataField = "CustomerName", Sort = true, DisplayOrder = 20, Filter =true)]
        public string CustomerName { get { return this.Customer.Name; } }

        

        public override string ToString()
        {
            string result = string.Format(TostringTemplate + ", OrderId: {0}, Customer: {1}, IntegrationStatus: `{2}`, BatchId: {3}", OrderId, Customer.Id, IntegrationStatus, BatchId);

            return result;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
