using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Koenig.Maestro.Operation.QuickBooks;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework.ManagerRepository;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class QuickBooksProductMap : TransactionBase
    {
        QuickBooksProductMapManager qm;
        public QuickBooksProductMap(TransactionContext context) : base("QB_PRODUCT_MAP", context)
        {
            this.MainEntitySample = new QuickBooksProductMapDef();
            qm = new QuickBooksProductMapManager(context);
        }

        protected override void DeserializeLog(byte[] logData)
        {
            throw new NotImplementedException();
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = QuickBooksProductMapCache.Instance[id];
        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = QuickBooksProductMapCache.Instance[id];
            qm.Delete(id);
        }

        protected override void List()
        {
            response.TransactionResult = QuickBooksProductMapCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {

            Entity.QuickBooksProductMapDef map = (Entity.QuickBooksProductMapDef)request.TransactionEntityList[0];

            SpCall call = new SpCall("DAT.QB_PRODUCT_MAP_INSERT");
            call.SetBigInt("@PRODUCT_ID", map.Product.Id);
            call.SetVarchar("@QB_CODE", map.QuickBooksCode);
            call.SetVarchar("@QB_LIST_ID", map.QuickBooksListId);
            call.SetVarchar("@QB_PARENT_CODE", map.QuickBooksParentCode);
            call.SetVarchar("@QB_PARENT_LIST_ID", map.QuickBooksParentListId);
            call.SetVarchar("@QB_DESCRIPTION", map.QuickBooksDescription);
            call.SetBigInt("@UNIT_ID", map.Unit.Id);
            call.SetDecimal("@PRICE", map.Price);
            call.SetDateTime("@CREATE_DATE", DateTime.Now);
            call.SetVarchar("@CREATE_USER", Context.UserName);

            map.Id = db.ExecuteScalar<long>(call);
            response.TransactionResult = map;

        }

        protected override void BackUp()
        {
            Guid guid = Guid.NewGuid();
            qm.BackUp(guid);
        }

        protected override void Update()
        {
            Entity.QuickBooksProductMapDef map = (Entity.QuickBooksProductMapDef)request.TransactionEntityList[0];
            qm.Update(map);
            Context.TransactionObject = map;

        }

        protected override void ImportQb()
        {
            /*
            ImportRequestType importRequestType = GetEnum<ImportRequestType>(extendedData[MessageDataExtensionKeys.IMPORT_TYPE]);
            string qbId = string.Empty;
            if (importRequestType == ImportRequestType.Single)
                qbId = extendedData[MessageDataExtensionKeys.ID];
            */
        }

        protected override void ExportQb()
        {
            throw new NotImplementedException();
        }

        public override void RefreshCache(ActionType at)
        {
            QuickBooksProductMapCache.Instance.Reload(true);
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
