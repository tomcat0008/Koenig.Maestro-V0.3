using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.TransactionRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class TransactionCache : DbLoadCache<long, TransactionDefinition>
    {
        TransactionCache(): base("TRANSACTION_CACHE", "COR.TRANSACTION_DEFINITION_SELECT_ALL")
        {
        }

        static TransactionCache instance = null;

        public static TransactionCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new TransactionCache();
                return instance;
            }
        }

        public TransactionDefinition Get(string transactionCode)
        {
            return Get(transactionCode, true);
        }

        public TransactionDefinition Get(string transactionCode, bool throwException)
        {
            TransactionDefinition result = Values.ToList().Find(t => t.TranCode.Equals(transactionCode));
            if(throwException)
                if (result == null)
                    throw new Exception(string.Format("Invalid transaction code `{0}`", transactionCode));
            return result;
        }


        protected override Tuple<long, TransactionDefinition> GetItem(SqlReader reader)
        {
            TransactionDefinition t = new TransactionDefinition();
            t.Id = reader.GetInt64("ID");
            t.GroupCode = reader.GetString("GROUP_CODE");
            t.IsCancellable = reader.GetBool("IS_CANCELLABLE");
            t.IsQbRelated = reader.GetBool("QB_RELATED");
            t.MenuId = reader.GetInt32("MENU_ID");
            t.TranCode = reader.GetString("TRAN_CODE");
            t.TranDescription = reader.GetString("TRAN_DESCRIPTION");
            t.CodeBase = reader.GetString("CODE_BASE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            return new Tuple<long, TransactionDefinition>(t.Id, t);

        }

    }
}
