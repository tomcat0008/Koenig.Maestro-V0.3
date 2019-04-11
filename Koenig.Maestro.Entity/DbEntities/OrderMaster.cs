using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity
{
    [Serializable]
    public class OrderMaster : DbEntityBase, ITransactionEntity
    {
        [JsonConverter(typeof(EntityJsonConverter))]
        public MaestroCustomer Customer { get; set; }
        [DisplayProperty(Text = "Order date", DataField = "OrderDate",Sort = true, DisplayOrder = 10, Filter = true)]
        public DateTime OrderDate { get; set; }
        [DisplayProperty(Text = "Delivery date", DataField = "DeliveryDate",Sort = true, DisplayOrder = 20)]
        public DateTime DeliveryDate { get; set; }
        public string PaymentType { get; set; }
        [DisplayProperty(Text = "Notes", DataField = "Notes",Sort = true, DisplayOrder = 40)]
        public string Notes { get; set; }
        [DisplayProperty(Text = "Order status", DataField = "OrderStatus",Sort = true, DisplayOrder = 30, Filter =true)]
        public string OrderStatus { get; set; }
        
        public List<OrderItem> OrderItems { get; set; }
        [DisplayProperty(Text = "Customer", DataField = "CustomerName", Sort = true, DisplayOrder = 5, Filter = true)]
        public string CustomerName
        {
            get { return this.Customer.Name; }
        }

        [DisplayProperty(Text = "Nr.of Items", DataField = "ItemCount", Sort = true, DisplayOrder = 60)]
        public int ItemCount
        {
            get { return this.OrderItems.Count; }
        }

        public long CustomerId { get { return this.Customer != null ? this.Customer.Id : 0; } }

        [DisplayProperty(Text = "Integration status", DataField = "IntegrationStatus",Sort = true, DisplayOrder = 50, Filter = true)]
        public string IntegrationStatus
        {
            get
            {
                string result = Enums.QbIntegrationLogStatus.UNKNOWN;
                if (InvoiceLog != null)
                    if(!string.IsNullOrWhiteSpace(InvoiceLog.IntegrationStatus))
                        result = InvoiceLog.IntegrationStatus;
                return result;
            }
        }

        public long ShippingAddressId { get; set; }

        public QuickBooksInvoiceLog InvoiceLog { get; set; }

        public override string ToString()
        {
            string result = string.Format(TostringTemplate + ", Customer: `{0}`, OrderDate: `{1}`, Item Count: {2}", Customer.Id, OrderDate, OrderItems.Count);
            StringBuilder builder = new StringBuilder(result);
            builder.AppendLine("Order items:");
            foreach (OrderItem oi in OrderItems)
                builder.AppendLine(oi.ToString());
            return builder.ToString() ;

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
