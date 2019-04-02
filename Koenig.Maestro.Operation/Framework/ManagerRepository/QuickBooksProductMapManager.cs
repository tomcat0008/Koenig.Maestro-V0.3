using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Koenig.Maestro.Operation.Cache.CacheRepository;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class QuickBooksProductMapManager : ManagerBase
    {

        public QuickBooksProductMapManager(TransactionContext context) : base(context)
        {
        }

        protected override DataTable PrepareTable(List<ITransactionEntity> itemList)
        {
            SpCall spCall = new SpCall("DAT.GET_QB_PRODUCT_MAP_SCHEMA");
            DataTable dt = db.ExecuteDataTable(spCall);
            MaestroProduct unknownProduct = new ProductManager(context).GetUnknownItem();
            MaestroUnit unknownUnit = new UnitManager(context).GetUnknownItem();
            itemList.Cast<QuickBooksProductMapDef>().ToList().ForEach(m =>
            {
                DataRow row = dt.NewRow();
                row["PRODUCT_ID"] = m.Product == null ? unknownProduct.Id : m.Product.Id;
                row["QB_CODE"] = m.QuickBooksCode;
                row["QB_LIST_ID"] = m.QuickBooksListId;
                row["QB_PARENT_CODE"] = m.QuickBooksParentCode;
                row["QB_PARENT_LIST_ID"] = m.QuickBooksParentListId;
                row["QB_DESCRIPTION"] = m.QuickBooksDescription;
                row["PRICE"] = m.Price;
                row["UNIT_ID"] = m.Unit == null ? unknownUnit.Id : m.Unit.Id ;
                row["CREATE_DATE"] = m.CreateDate;
                row["CREATE_USER"] = m.CreatedUser;
                row["UPDATE_DATE"] = m.UpdateDate;
                row["UPDATE_USER"] = m.UpdatedUser;
                row["RECORD_STATUS"] = "A";
                dt.Rows.Add(row);
            });

            dt.TableName = "DAT.QB_PRODUCT_MAP";
            return dt;

        }

        public void Delete(long id)
        {
            SpCall spCall = new SpCall("DAT.QB_PRODUCT_MAP_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(spCall);

        }

        public void Update(QuickBooksProductMapDef map)
        {
            SpCall call = new SpCall("DAT.QB_PRODUCT_MAP_UPDATE");
            call.SetBigInt("@ID", map.Id);
            call.SetBigInt("@PRODUCT_ID", map.Product.Id);
            call.SetVarchar("@QB_CODE", map.QuickBooksCode);
            call.SetVarchar("@QB_LIST_ID", map.QuickBooksListId);
            call.SetVarchar("@QB_PARENT_CODE", map.QuickBooksParentCode);
            call.SetVarchar("@QB_PARENT_LIST_ID", map.QuickBooksParentListId);
            call.SetVarchar("@QB_DESCRIPTION", map.QuickBooksDescription);
            call.SetBigInt("@UNIT_ID", map.Unit.Id);
            call.SetDecimal("@PRICE", map.Price);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(call);
        }

        public void BackUp(Guid guid)
        {
            SpCall call = new SpCall("BCK.BACK_UP_QB_PRODUCT_MAP");
            call.SetVarchar("@BATCH_ID", guid.ToString());
            call.SetDateTime("@BATCH_DATE", DateTime.Now);
            db.ExecuteNonQuery(call);
        }


    }
}
