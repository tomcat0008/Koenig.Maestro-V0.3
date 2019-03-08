using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class RegionCache : DbLoadCache<long, MaestroRegion>
    {
        static RegionCache instance = null;

        RegionCache() : base("REGION_CACHE", "DAT.REGION_SELECT_ALL")
        {
        }

        public static RegionCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new RegionCache();
                return instance;
            }
        }

        public MaestroRegion GetByName(string name)
        {
            MaestroRegion region = Values.ToList().Find(r => r.Name.Equals(name));
            return region;
        }

        public MaestroRegion GetByPostalCode(string pk)
        {
            MaestroRegion region = Values.ToList().Find(r => r.PostalCode.Equals(pk));
            return region;
        }

        protected override Tuple<long, MaestroRegion> GetItem(SqlReader reader)
        {
            MaestroRegion t = new MaestroRegion();
            t.Id = reader.GetInt64("ID");


            t.Name = reader.GetString("REGION_NAME");
            t.Description = reader.GetString("REGION_DESCRIPTION");
            t.PostalCode = reader.GetString("POSTAL_CODE");

            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroRegion>(t.Id, t);

        }
    }
}
