using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Koenig.Maestro.Operation.Framework.ManagerRepository;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal sealed class UnitType : TransactionBase
    {
        public UnitType(TransactionContext context) : base("UNIT_TYPE", context) { }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = UnitTypeCache.Instance[id];
            SpCall spCall = new SpCall("DAT.UNIT_TYPE_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", Context.UserName);
            Context.Database.ExecuteNonQuery(spCall);

        }

        protected override void DeserializeLog(byte[] logData)
        {
            throw new NotImplementedException();
        }


        protected override void ExportQb()
        {
            throw new NotImplementedException();
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = UnitTypeCache.Instance[id];
        }

        protected override void ImportQb()
        {
            throw new NotImplementedException();
        }

        protected override void List()
        {
            response.TransactionResult = UnitTypeCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            MaestroUnitType item = (MaestroUnitType)request.TransactionEntityList[0];
            item.CreateDate = DateTime.Now;
            item.CreatedUser = Context.UserName;

            new UnitTypeManager(Context).InsertNewItem(item);

            response.TransactionResult = item;
 
        }

        protected override void Update()
        {
            MaestroUnitType item = (MaestroUnitType)request.TransactionEntityList[0];
            Context.TransactionObject = item;
            SpCall call = new SpCall("DAT.UNIT_TYPE_UPDATE");
            call.SetBigInt("@ID", item.Id);
            call.SetVarchar("@UNIT_TYPE_NAME", item.Name);
            call.SetVarchar("@UNIT_TYPE_DESCRIPTION", item.Description);
            call.SetBit("@CAN_HAVE_UNITS", item.CanHaveUnits);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(call);
        }


        public override void RefreshCache(ActionType at)
        {
            UnitTypeCache.Instance.Reload(true);
            if (at == ActionType.Delete)
            {
                UnitCache.Instance.Reload(true);
                CustomerProductUnitCache.Instance.Reload(true);
                ProductCache.Instance.Reload(true);
            }
        }

        protected override void Undelete()
        {
            throw new NotImplementedException();
        }

        protected override void Erase()
        {
            throw new NotImplementedException();
        }

    }
}
