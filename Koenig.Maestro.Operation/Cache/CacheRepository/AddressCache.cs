using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class AddressCache : DbLoadCache<long, CustomerAddress>
    {
        static AddressCache instance = null;

        AddressCache() : base("ADDRESS_CACHE", "DAT.CUSTOMER_ADDRESS_SELECT_ALL")
        {
        }

        public static AddressCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new AddressCache();
                return instance;
            }
        }

        public List<CustomerAddress> GetByCustomerId(long customerId)
        {
            if (!Values.ToList().Exists(a => a.CustomerId == a.Id))
                return Values.Where(a => a.CustomerId == customerId).ToList();
            else
                return new List<CustomerAddress>();
        }

        public CustomerAddress GetByCode(string code)
        {
            return Values.ToList().Find(c => c.AddressCode == code);
        }

        public CustomerAddress GetByQbId(string qbId)
        {
            return Values.ToList().Find(c => c.QbListID == qbId);
        }

        protected override Tuple<long, CustomerAddress> GetItem(SqlReader reader)
        {
            CustomerAddress t = new CustomerAddress();
            t.Id = reader.GetInt64("ID");
            t.AddressCode = reader.GetString("ADDRESS_CODE");
            t.AddressType = reader.GetString("ADDRES_TYPE");
            t.City = reader.GetString("CITY");
            t.CustomerId = reader.GetInt64("CUSTOMER_ID");
            t.Line1 = reader.GetString("LINE1");
            t.Line2 = reader.GetString("LINE2");
            t.Line3 = reader.GetString("LINE3");
            t.Line4 = reader.GetString("LINE4");
            t.Line5 = reader.GetString("LINE5");
            t.PostalCode = reader.GetString("POSTAL_CODE");
            t.Province = reader.GetString("PROVINCE");
            t.QbListID = reader.GetString("QB_LIST_ID");
            t.QbName = reader.GetString("QB_NAME");

            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");
            return new Tuple<long, CustomerAddress>(t.Id, t);

        }
    }

}
