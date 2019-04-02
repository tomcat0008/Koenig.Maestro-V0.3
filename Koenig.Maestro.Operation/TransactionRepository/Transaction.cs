using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Koenig.Maestro.Entity.Enums;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal sealed class Transaction : TransactionBase
    {
        public Transaction(TransactionContext context) : base("TRAN_DEFINITION", context)
        {
            this.MainEntitySample = new TransactionDefinition();
        }

        protected override void DeserializeLog(byte[] logData)
        {
            throw new NotImplementedException();
        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            try
            {
                Context.TransactionObject = TransactionCache.Instance[id];
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Transaction id {0} could not be found", id), ex);
            }
            SpCall spCall = new SpCall("COR.TRANSACTION_DEFINITION_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", Context.UserName);
            db.ExecuteNonQuery(spCall);
        }

        protected override void ExportQb()
        {
            throw new NotImplementedException();
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            try
            {
                Context.TransactionObject = TransactionCache.Instance[id];
                response.TransactionResult = Context.TransactionObject;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Transaction id {0} could not be found", id), ex);
            }
        }

        protected override void ImportQb()
        {
            throw new NotImplementedException();
        }

        protected override void List()
        {
            List<TransactionDefinition> list = TransactionCache.Instance.Values.ToList();
            response.TransactionResult = list.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            TransactionDefinition td = (TransactionDefinition)request.TransactionEntityList[0];
            td.CreateDate = DateTime.Now;
            td.CreatedUser = Context.UserName;

            if (TransactionCache.Instance.Get(td.TranCode, false) != null)
                throw new Exception(string.Format("Trancode `{0}` exists", td.TranCode));

            SpCall spCall = new SpCall("COR.TRANSACTION_DEFINITION_INSERT");
            spCall.SetVarchar("@TRAN_CODE", td.TranCode);
            spCall.SetVarchar("@TRAN_DESCRIPTION", td.TranDescription);
            spCall.SetBit("@IS_CANCELLABLE", td.IsCancellable);

            spCall.SetInt("@MENU_ID", td.MenuId);
            spCall.SetVarchar("@GROUP_CODE", td.GroupCode);
            spCall.SetBit("@QB_RELATED", td.IsQbRelated);
            spCall.SetVarchar("@CODE_BASE", td.CodeBase); 
            spCall.SetDateTime("@CREATE_DATE", td.CreateDate);
            spCall.SetVarchar("@CREATE_USER", td.CreatedUser);

            long id = db.ExecuteScalar<long>(spCall);
            td.Id = id;
            Context.TransactionObject = td;

        }

        protected override void Update()
        {
            TransactionDefinition td = (TransactionDefinition)request.TransactionEntityList[0];
            td.UpdateDate = DateTime.Now;
            td.UpdatedUser = Context.UserName;

            SpCall spCall = new SpCall("COR.TRANSACTION_DEFINITION_UPDATE");
            spCall.SetBigInt("ID", td.Id);
            spCall.SetVarchar("@TRAN_CODE", td.TranCode);
            spCall.SetVarchar("@TRAN_DESCRIPTION", td.TranDescription);
            spCall.SetBit("@IS_CANCELLABLE", td.IsCancellable);
            spCall.SetInt("@MENU_ID", td.MenuId);
            spCall.SetVarchar("@GROUP_CODE", td.GroupCode);
            spCall.SetBit("@QB_RELATED", td.IsQbRelated);
            spCall.SetVarchar("@CODE_BASE", td.CodeBase);
            spCall.SetDateTime("@UPDATE_DATE", td.UpdateDate);
            spCall.SetVarchar("@UPDATE_USER", td.UpdatedUser);
            db.ExecuteNonQuery(spCall);

            Context.TransactionObject = td;

        }

        public override void RefreshCache(ActionType at)
        {
            TransactionCache.Instance.Reload(true);
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
