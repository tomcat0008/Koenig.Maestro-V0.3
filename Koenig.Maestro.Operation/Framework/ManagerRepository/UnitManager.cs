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
    internal class UnitManager : ManagerBase
    {
        public UnitManager(TransactionContext context) : base(context) { }

        public void InsertNewItem(MaestroUnit item)
        {
            SpCall call = new SpCall("DAT.UNIT_INSERT");
            call.SetBigInt("@UNIT_TYPE_ID", item.UnitType.Id);
            call.SetVarchar("@UNIT_NAME", item.Name);
            call.SetVarchar("@QB_UNIT", item.QuickBooksUnit);
            call.SetDateTime("@CREATE_DATE", item.CreateDate);
            call.SetVarchar("@CREATE_USER", item.CreatedUser);
            item.Id = db.ExecuteNonQuery(call);
        }

        public MaestroUnit GetUnknownItem()
        {
            MaestroUnit unit = UnitCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);

            if (unit == null)
            {
                unit = new MaestroUnit()
                {
                    Name = MaestroApplication.Instance.UNKNOWN_ITEM_NAME,
                    QuickBooksUnit = string.Empty,
                    UnitType = new UnitTypeManager(context).GetUnknownItem(),
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedUser = "MAESTRO",
                    UpdatedUser = "MAESTRO",
                    RecordStatus = "A"
                };

                InsertNewItem(unit);
                UnitCache.Instance.Reload(true);
            }

            return unit;
        }


    }
}
