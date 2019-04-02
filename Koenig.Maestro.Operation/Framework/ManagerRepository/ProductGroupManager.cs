using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class ProductGroupManager : ManagerBase
    {
        public ProductGroupManager(TransactionContext context) : base(context)
        {
        }

        public MaestroProductGroup GetUnknownItem()
        {
            MaestroProductGroup unitType = ProductGroupCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);

            if (unitType == null)
            {
                unitType = new MaestroProductGroup()
                {
                    Name = MaestroApplication.Instance.UNKNOWN_ITEM_NAME,
                    GroupDescription = string.Empty,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedUser = "MAESTRO",
                    UpdatedUser = "MAESTRO",
                    RecordStatus = "A"
                };
                InsertNewItem(unitType);
            }

            return unitType;
        }


        public void Delete(long id)
        {
            SpCall spCall = new SpCall("DAT.PRODUCT_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(spCall);
        }

        public void Update(MaestroProductGroup productGroup)
        {
            SpCall call = new SpCall("DAT.PRODUCT_GROUP_UPDATE");
            call.SetBigInt("@ID", productGroup.Id);
            call.SetVarchar("@PRODUCT_GROUP_NAME", productGroup.Name);
            call.SetVarchar("@GROUP_DESCRIPTION", productGroup.GroupDescription);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", context.UserName);

            db.ExecuteNonQuery(call);
        }


        public void InsertNewItem(MaestroProductGroup productGroup)
        {
            SpCall call = new SpCall("DAT.PRODUCT_GROUP_INSERT");

            call.SetVarchar("@PRODUCT_GROUP_NAME", productGroup.Name);
            call.SetVarchar("@GROUP_DESCRIPTION", productGroup.GroupDescription);
            call.SetDateTime("@CREATE_DATE", productGroup.CreateDate);
            call.SetVarchar("@CREATE_USER", productGroup.CreatedUser);
            productGroup.Id = db.ExecuteScalar<long>(call);
        }

        public void BackUp(Guid guid)
        {
            SpCall call = new SpCall("BCK.BACK_UP_PRODUCT_GROUPS");
            call.SetVarchar("@BATCH_ID", guid.ToString());
            call.SetDateTime("@BATCH_DATE", DateTime.Now);
            db.ExecuteNonQuery(call);

        }

    }
}
