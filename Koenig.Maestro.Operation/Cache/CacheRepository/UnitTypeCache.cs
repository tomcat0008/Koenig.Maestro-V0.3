using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class UnitTypeCache : DbLoadCache<long, MaestroUnitType>
    {
        static UnitTypeCache instance = null;

        UnitTypeCache() : base("UNIT_TYPE_CACHE", "DAT.UNIT_TYPE_SELECT_ALL")
        {
        }

        public static UnitTypeCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new UnitTypeCache();
                return instance;
            }
        }

        public MaestroUnitType GetByName(string name)
        {
            MaestroUnitType unitType = Values.ToList().Find(r => r.Name.Equals(name));
            return unitType;
        }

        protected override Tuple<long, MaestroUnitType> GetItem(SqlReader reader)
        {
            MaestroUnitType t = new MaestroUnitType();
            t.Id = reader.GetInt64("ID");
            t.CanHaveUnits = reader.GetBool("CAN_HAVE_UNITS");
            t.Name = reader.GetString("UNIT_TYPE_NAME");
            t.Description = reader.GetString("UNIT_TYPE_DESCRIPTION");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroUnitType>(t.Id, t);

        }
    }
}
