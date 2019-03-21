using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class ProductGroupCache : DbLoadCache<long, MaestroProductGroup>
    {
        static ProductGroupCache instance = null;

        ProductGroupCache() : base("PRODUCT_GROUP_CACHE", "DAT.PRODUCT_GROUP_SELECT_ALL")
        {
        }

        public static ProductGroupCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProductGroupCache();
                return instance;
            }
        }

        public MaestroProductGroup GetByName(string name)
        {
            MaestroProductGroup pg = Values.ToList().Find(r => r.Name.Equals(name));
            return pg;
        }


        protected override Tuple<long, MaestroProductGroup> GetItem(SqlReader reader)
        {
            MaestroProductGroup item = new MaestroProductGroup();
            item.Id = reader.GetInt64("ID");

            item.GroupDescription = reader.GetString("GROUP_DESCRIPTION");
            item.Name = reader.GetString("PRODUCT_GROUP_NAME");

            item.CreateDate = reader.GetDateTime("CREATE_DATE");
            item.RecordStatus = reader.GetString("RECORD_STATUS");
            item.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            item.UpdatedUser = reader.GetString("UPDATE_USER");
            item.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroProductGroup>(item.Id, item);

        }


    }
}
