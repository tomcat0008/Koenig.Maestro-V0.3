using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class CustomerProductUnitCache : DbLoadCache<long, CustomerProductUnit>
    {
        static CustomerProductUnitCache instance = null;

        CustomerProductUnitCache() : base("CUSTOMER_CACHE", "DAT.CUSTOMER_PRODUCT_UNIT_SELECT_ALL")
        {
        }

        public static CustomerProductUnitCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new CustomerProductUnitCache();
                return instance;
            }
        }

        public bool Exists(CustomerProductUnit cpu)
        {
            bool result = Values.ToList().Exists(item => item.Customer.Id == cpu.Customer.Id &&
                item.Product.Id == cpu.Product.Id && item.Unit.Id == cpu.Unit.Id);
            return result;
        }

        protected override Tuple<long, CustomerProductUnit> GetItem(SqlReader reader)
        {
            CustomerProductUnit t = new CustomerProductUnit();
            t.Id = reader.GetInt64("ID");
            long id = reader.GetInt64("CUSTOMER_ID");
            try
            {
                t.Customer = CustomerCache.Instance[id];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Invalid customer id {0}", id), ex);
            }
            id = reader.GetInt64("PRODUCT_ID");
            try
            {
                t.Product = ProductCache.Instance[id];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Invalid product id {0}", id), ex);
            }
            id = reader.GetInt64("UNIT_ID");
            try
            {
                t.Unit = UnitCache.Instance[id];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Invalid unit id {0}", id), ex);
            }
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            return new Tuple<long, CustomerProductUnit>(t.Id, t);

        }
    }

}
