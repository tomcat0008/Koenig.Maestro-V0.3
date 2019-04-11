using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal abstract class ManagerBase
    {
        public event TransactionProgressEventHandler TransactionProgress;

        protected Database db;
        protected TransactionContext context;
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected ManagerBase() : this(null) { }

        protected ManagerBase(TransactionContext context)
        {
            this.context = context;
            if(context.Database != null)
                this.db = context.Database;
        }

        public void BulkInsert(List<ITransactionEntity> itemList, bool useTransaction)
        {
            DataTable dt = PrepareTable(itemList);
            if (useTransaction)
                db.ExecuteBulkInsert(dt);
            else
            {
                using (Database dbBulk = new Database())
                {
                    db.ExecuteBulkInsert(dt);
                }
            }
        }

        protected void OnTransactionProgress(TransactionProgressEventArgs e)
        {
            if (TransactionProgress != null)
                this.TransactionProgress(this, e);
        }



        public void BulkInsert(List<ITransactionEntity> itemList)
        {
            
            BulkInsert(itemList, true);
        }

        protected virtual DataTable PrepareTable(List<ITransactionEntity> itemList)
        {
            return null;
        }

    }
}
