using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class ProductCache : DbLoadCache<long, MaestroProduct>
    {
        static ProductCache instance = null;

        ProductCache() : base("PRODUCT_CACHE", "DAT.PRODUCT_SELECT_ALL")
        {
        }

        public static ProductCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProductCache();
                return instance;
            }
        }

        public MaestroProduct GetByName(string name)
        {
            MaestroProduct region = Values.ToList().Find(r => r.Name.Equals(name));
            return region;
        }

        public MaestroProduct GetByQbId(string qbId)
        {
            List<MaestroProduct> values = Values.ToList();
            MaestroProduct region = values.Find(r => r.QuickBooksProductId.Equals(qbId));
            return region;
        }

        protected override Tuple<long, MaestroProduct> GetItem(SqlReader reader)
        {
            MaestroProduct product = new MaestroProduct();
            product.Id = reader.GetInt64("ID");

            product.Description = reader.GetString("PRODUCT_DESCRIPTION");
            product.MinimumOrderQuantity = reader.GetInt32("MINIMUM_ORDER_QUANTITY");
            product.Name = reader.GetString("PRODUCT_NAME");
            product.Price = reader.GetDecimal("PRICE");
            product.QuickBooksProductId = reader.GetString("QB_PRODUCT_ID");
            product.UnitType = UnitTypeCache.Instance[reader.GetInt64("UNIT_TYPE_ID")];
            /*product.QbProductMaps = QuickBooksProductMapCache.Instance.GetByProductId(product.Id);
            product.QbProductMaps.ForEach(q => q.Product = product);*/

            product.ProductGroup = ProductGroupCache.Instance[reader.GetInt64("PRODUCT_GROUP_ID")];
            product.CreateDate = reader.GetDateTime("CREATE_DATE");
            product.RecordStatus = reader.GetString("RECORD_STATUS");
            product.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            product.UpdatedUser = reader.GetString("UPDATE_USER");
            product.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroProduct>(product.Id, product);

        }
    }
}
