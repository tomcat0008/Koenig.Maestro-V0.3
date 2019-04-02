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
using Newtonsoft.Json.Linq;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal sealed class Unit : TransactionBase
    {
        public Unit(TransactionContext context) : base("UNIT", context) {
            this.MainEntitySample = new MaestroUnit();
        }

        protected override void Delete()
        {
            string userName = request.MessageHeader.UserName;
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = UnitCache.Instance[id];

            SpCall spCall = new SpCall("DAT.UNIT_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", userName);
            db.ExecuteNonQuery(spCall);
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
            response.TransactionResult = UnitCache.Instance[id];
        }

        protected override void ImportQb()
        {
            throw new NotImplementedException();
        }

        protected override void List()
        {
            response.TransactionResult = UnitCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            MaestroUnit item = (MaestroUnit)request.TransactionEntityList[0];

            new UnitManager(Context).InsertNewItem(item);

            response.TransactionResult = item;
            
        }

        public override void Deserialize(JToken token)
        {
            MaestroUnit resultObj = new MaestroUnit();

            JObject entityObj = JObject.Parse(token.ToString());

            resultObj.Id = entityObj["Id"].ToObject<long>();
            resultObj.Name = entityObj["Name"].ToString();

            resultObj.QuickBooksUnit = entityObj["QuickBooksUnit"].ToString();
            resultObj.UnitType = UnitTypeCache.Instance[entityObj["UnitTypeId"].ToObject<long>()];


            Context.TransactionObject = resultObj;
        }

        protected override void Update()
        {
            MaestroUnit item = (MaestroUnit)request.TransactionEntityList[0];
            Context.TransactionObject = item;
            SpCall call = new SpCall("DAT.UNIT_UPDATE");
            call.SetBigInt("@ID", item.Id);
            call.SetBigInt("@UNIT_TYPE_ID", item.UnitType.Id);
            call.SetVarchar("@UNIT_NAME", item.Name);
            call.SetVarchar("@QB_UNIT", item.QuickBooksUnit);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(call);
            

        }

        public override void RefreshCache(ActionType at)
        {
            UnitCache.Instance.Reload(true);
            if (at == ActionType.Delete)
                CustomerProductUnitCache.Instance.Reload(true);
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
