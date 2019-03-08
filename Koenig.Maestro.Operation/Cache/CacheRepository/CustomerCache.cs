using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class CustomerCache : DbLoadCache<long, MaestroCustomer>
    {
        static CustomerCache instance = null;

        CustomerCache() : base("CUSTOMER_CACHE", "DAT.CUSTOMER_SELECT_ALL")
        {
        }

        public static CustomerCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomerCache();
                return instance;
            }
        }

        public MaestroCustomer GetByQbId(string qbId)
        {
            return Values.ToList().Find(c => c.QuickBooksId == qbId);
        }

        protected override Tuple<long, MaestroCustomer> GetItem(SqlReader reader)
        {
            MaestroCustomer t = new MaestroCustomer();
            t.Id = reader.GetInt64("ID");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.Address = reader.GetString("CUSTOMER_ADDRESS");
            t.DefaultPaymentType = reader.GetString("DEFAULT_PAYMENT_TYPE");
            t.Email = reader.GetString("CUSTOMER_EMAIL");
            t.Name = reader.GetString("CUSTOMER_NAME");
            t.Phone = reader.GetString("CUSTOMER_PHONE");
            t.QuickBooksId = reader.GetString("QB_CUSTOMER_ID");
            t.QuickBoosCompany = reader.GetString("QB_COMPANY");
            long regionId = reader.GetInt64("REGION_ID");
            try
            {
                t.Region = RegionCache.Instance[regionId];
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Invalid region id {0}", regionId), ex);
            }
            t.Title = reader.GetString("CUSTOMER_TITLE");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroCustomer>(t.Id, t);

        }
    }
}
