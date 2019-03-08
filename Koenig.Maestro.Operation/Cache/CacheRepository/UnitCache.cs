using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class UnitCache : DbLoadCache<long, MaestroUnit>
    {
        static UnitCache instance = null;

        UnitCache() : base("UNIT_CACHE", "DAT.UNIT_SELECT_ALL")
        {
        }

        public static UnitCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new UnitCache();
                return instance;
            }
        }

        public MaestroUnit GetByName(string name)
        {
            MaestroUnit unit = Values.ToList().Find(r => r.Name.Equals(name));
            return unit;
        }

        protected override Tuple<long, MaestroUnit> GetItem(SqlReader reader)
        {
            MaestroUnit t = new MaestroUnit();
            t.Id = reader.GetInt64("ID");
            long id = reader.GetInt64("UNIT_TYPE_ID");
            try
            {
                t.UnitType = UnitTypeCache.Instance[id];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Invalid unit type id {0}", id), ex);
            }
            t.Name = reader.GetString("UNIT_NAME");
            t.QuickBooksUnit = reader.GetString("QB_UNIT");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroUnit>(t.Id, t);

        }
    }
}
