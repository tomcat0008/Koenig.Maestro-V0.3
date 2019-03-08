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
    internal class RegionManager : ManagerBase
    {
        public RegionManager(TransactionContext context) : base(context) { }

        protected override DataTable PrepareTable(List<ITransactionEntity> itemList)
        {
            throw new NotImplementedException();
        }

        public MaestroRegion GetUnknownItem()
        {
            MaestroRegion region = RegionCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);

            if (region == null)
            {
                region = new MaestroRegion()
                {
                    PostalCode = "",
                    Name = MaestroApplication.Instance.UNKNOWN_ITEM_NAME,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedUser = "MAESTRO",
                    UpdatedUser = "MAESTRO",
                    Description = ""
                };


                SpCall call = new SpCall("DAT.REGION_INSERT");
                call.SetVarchar("@POSTAL_CODE", region.PostalCode);
                call.SetVarchar("@REGION_NAME", region.Name);
                call.SetVarchar("@REGION_DESCRIPTION", region.Description);
                call.SetDateTime("@CREATE_DATE", region.CreateDate);
                call.SetVarchar("@CREATE_USER", "MAESTRO");
                region.Id = db.ExecuteScalar<long>(call);


                RegionCache.Instance.Reload(true);
            }

            return region;
        }

    }
}
