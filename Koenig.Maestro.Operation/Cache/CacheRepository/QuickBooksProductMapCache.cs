using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class QuickBooksProductMapCache : DbLoadCache<long, QuickBooksProductMapDef>
    {
        public QuickBooksProductMapCache() : base("QB_PRODUCT_MAP", "DAT.QB_PRODUCT_MAP_SELECT_ALL") { }

        static QuickBooksProductMapCache instance;

        public static QuickBooksProductMapCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new QuickBooksProductMapCache();
                return instance;
            }
        }

        public List<QuickBooksProductMapDef> GetByProductId(long id)
        {
            return Values.ToList().Where(q => q.Product.Id == id).ToList();
        }

        public QuickBooksProductMapDef GetByQbId(string id)
        {
            return Values.ToList().Find(q => q.QuickBooksListId == id);
        }

        protected override Tuple<long, QuickBooksProductMapDef> GetItem(SqlReader reader)
        {
            QuickBooksProductMapDef t = new QuickBooksProductMapDef
            {
                Id = reader.GetInt64("ID"),
                QuickBooksCode = reader.GetString("QB_CODE"),
                QuickBooksDescription = reader.GetString("QB_DESCRIPTION"),
                QuickBooksListId = reader.GetString("QB_LIST_ID"),
                QuickBooksParentCode = reader.GetString("QB_PARENT_CODE"),
                QuickBooksParentListId = reader.GetString("QB_PARENT_LIST_ID"),
                Product = ProductCache.Instance[reader.GetInt64("PRODUCT_ID")],
                Price = reader.GetDecimal("PRICE"),
                Unit = UnitCache.Instance[reader.GetInt64("UNIT_ID")],
                CreateDate = reader.GetDateTime("CREATE_DATE"),
                RecordStatus = reader.GetString("RECORD_STATUS"),
                UpdateDate = reader.GetDateTime("UPDATE_DATE"),
                UpdatedUser = reader.GetString("UPDATE_USER"),
                CreatedUser = reader.GetString("CREATE_USER"),
                Label = reader.GetString("REPORT_LABEL")
            };

            return new Tuple<long, QuickBooksProductMapDef>(t.Id, t);

        }

    }
}
