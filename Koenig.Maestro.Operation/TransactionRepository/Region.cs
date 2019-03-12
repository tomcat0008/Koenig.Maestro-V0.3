using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;


namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal sealed class Region : TransactionBase
    {
        public Region(TransactionContext context) : base("REGION", context) { }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            try
            {
                Context.TransactionObject = RegionCache.Instance[id];
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Region id {0} could not be found", id), ex);
            }
            SpCall spCall = new SpCall("DAT.REGION_DELETE");
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
            response.TransactionResult = RegionCache.Instance[id];
        }

        protected override void ImportQb()
        {
            throw new NotImplementedException();
        }

        protected override void List()
        {
            response.TransactionResult = RegionCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            MaestroRegion region = (MaestroRegion)request.TransactionEntityList[0];

            SpCall call = new SpCall("DAT.REGION_INSERT");
            call.SetVarchar("@POSTAL_CODE", region.PostalCode);
            call.SetVarchar("@REGION_NAME", region.Name);
            call.SetVarchar("@REGION_DESCRIPTION", region.Description);
            call.SetDateTime("@CREATE_DATE", DateTime.Now);
            call.SetVarchar("@CREATE_USER", Context.UserName);
            region.Id = db.ExecuteScalar<long>(call);

            response.TransactionResult = region;
        }

        protected override void Update()
        {
            MaestroRegion region = (MaestroRegion)request.TransactionEntityList[0];

            SpCall call = new SpCall("DAT.REGION_UPDATE");
            call.SetBigInt("@ID", region.Id);
            call.SetVarchar("@POSTAL_CODE", region.PostalCode);
            call.SetVarchar("@REGION_NAME", region.Name);
            call.SetVarchar("@REGION_DESCRIPTION", region.Description);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(call);
            Context.TransactionObject = region;
            RegionCache.Instance.Reload(true);
        }

        public override void RefreshCache(ActionType at)
        {
            RegionCache.Instance.Reload(true);
            if (at == ActionType.Delete)
                CustomerCache.Instance.Reload(true);
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
