using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class UnitTypeManager : ManagerBase
    {
        public UnitTypeManager(TransactionContext context) : base(context) { }


        public void InsertNewItem(MaestroUnitType item)
        {

            SpCall call = new SpCall("DAT.UNIT_TYPE_INSERT");
            call.SetVarchar("@UNIT_TYPE_NAME", item.Name);
            call.SetVarchar("@UNIT_TYPE_DESCRIPTION", item.Description);
            call.SetBit("@CAN_HAVE_UNITS", item.CanHaveUnits);
            call.SetDateTime("@CREATE_DATE", item.CreateDate);
            call.SetVarchar("@CREATE_USER", item.CreatedUser);
            item.Id = db.ExecuteNonQuery(call);
        }

        public MaestroUnitType GetUnknownItem()
        {
            MaestroUnitType unitType = UnitTypeCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);

            if (unitType == null)
            {
                unitType = new MaestroUnitType()
                {
                    Name = MaestroApplication.Instance.UNKNOWN_ITEM_NAME,
                    CanHaveUnits = false,
                    Description = string.Empty,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedUser = "MAESTRO",
                    UpdatedUser = "MAESTRO",
                    RecordStatus = "A"
                };
                InsertNewItem(unitType);
            }

            return unitType;
        }
    }
}
