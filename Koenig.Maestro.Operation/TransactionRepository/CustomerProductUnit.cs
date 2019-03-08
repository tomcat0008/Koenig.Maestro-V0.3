using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class CustomerProductUnit : TransactionBase
    {
        public CustomerProductUnit(TransactionContext context) : base("CUSTOMER_PRODUCT_UNIT", context)
        {

        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = CustomerProductUnitCache.Instance[id];
            SpCall spCall = new SpCall("DAT.CUSTOMER_PRODUCT_UNIT_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", Context.UserName);
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
            response.TransactionResult = CustomerProductUnitCache.Instance[id];
        }

        protected override void ImportQb()
        {
            throw new NotImplementedException();
        }

        protected override void List()
        {
            response.TransactionResult = CustomerProductUnitCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            Entity.CustomerProductUnit item = (Entity.CustomerProductUnit)request.TransactionEntityList[0];

            if (CustomerProductUnitCache.Instance.Exists(item))
                throw new Exception("Item already exists");

            SpCall call = new SpCall("DAT.CUSTOMER_PRODUCT_UNIT_INSERT");

            call.SetBigInt("@CUSTOMER_ID", item.Customer.Id);
            call.SetBigInt("@PRODUCT_ID", item.Product.Id);
            call.SetBigInt("@UNIT_ID", item.Unit.Id);
            call.SetDateTime("@CREATE_DATE", DateTime.Now);
            call.SetVarchar("@CREATE_USER", Context.UserName);
            long id = db.ExecuteScalar<long>(call);
            item.Id = id;
            Context.TransactionObject = item;
            
        }

        protected override void Update()
        {
            Entity.CustomerProductUnit item = (Entity.CustomerProductUnit)request.TransactionEntityList[0];

            if (CustomerProductUnitCache.Instance.Exists(item))
                throw new Exception("Item already exists");

            SpCall call = new SpCall("DAT.CUSTOMER_PRODUCT_UNIT_UPDATE");
            call.SetBigInt("@ID", item.Id);
            call.SetBigInt("@CUSTOMER_ID", item.Customer.Id);
            call.SetBigInt("@PRODUCT_ID", item.Product.Id);
            call.SetBigInt("@UNIT_ID", item.Unit.Id);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(call);
            Context.TransactionObject = item;
            
        }

        public override void RefreshCache(ActionType at)
        {
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
