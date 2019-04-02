using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class CustomerProductUnitManager: ManagerBase
    {
        public CustomerProductUnitManager(TransactionContext context) : base(context)
        {
        }

        public void BackUp(Guid guid)
        {
            SpCall call = new SpCall("BCK.BACK_UP_CUSTOMER_PRODUCT_UNIT");
            call.SetVarchar("@BATCH_ID", guid.ToString());
            call.SetDateTime("@BATCH_DATE", DateTime.Now);
            db.ExecuteNonQuery(call);
        }

    }
}
