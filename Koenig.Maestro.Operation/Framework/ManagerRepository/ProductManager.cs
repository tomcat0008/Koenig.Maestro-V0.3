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
    internal class ProductManager : ManagerBase
    {
        public ProductManager(TransactionContext context) : base(context)
        {
        }

        public void Delete(long id)
        {
            SpCall spCall = new SpCall("DAT.PRODUCT_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(spCall);
        }

        public void Update(MaestroProduct product)
        {
            SpCall call = new SpCall("DAT.PRODUCT_UPDATE");
            call.SetBigInt("@ID", product.Id);
            call.SetVarchar("@PRODUCT_NAME", product.Name);
            call.SetVarchar("@PRODUCT_DESCRIPTION", product.Description);

            call.SetVarchar("@QB_PRODUCT_ID", product.QuickBooksProductId);
            call.SetDecimal("@PRICE", product.Price);

            call.SetInt("@MINIMUM_ORDER_QUANTITY", product.MinimumOrderQuantity);
            call.SetBigInt("@UNIT_TYPE_ID", product.UnitType.Id);
            call.SetBigInt("@PRODUCT_GROUP_ID", product.GroupId);
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", context.UserName);

            db.ExecuteNonQuery(call);
        }

        protected override DataTable PrepareTable(List<ITransactionEntity> itemList)
        {
            SpCall spCall = new SpCall("DAT.GET_PRODUCT_SCHEMA");
            DataTable dt = db.ExecuteDataTable(spCall);

            itemList.Cast<MaestroProduct>().ToList().ForEach(p =>
            {
                DataRow row = dt.NewRow();
                row["PRODUCT_NAME"] = p.Name;
                row["PRODUCT_DESCRIPTION"] = p.Description;
                row["QB_PRODUCT_ID"] = p.QuickBooksProductId;
                row["PRICE"] = p.Price;
                row["MINIMUM_ORDER_QUANTITY"] = p.MinimumOrderQuantity;
                row["UNIT_TYPE_ID"] = p.UnitType.Id;
                row["PRODUCT_GROUP_ID"] = p.GroupId;
                row["CREATE_DATE"] = p.CreateDate;
                row["CREATE_USER"] = p.CreatedUser;
                row["UPDATE_DATE"] = p.UpdateDate;
                row["UPDATE_USER"] = p.UpdatedUser;
                row["RECORD_STATUS"] = "A";
                dt.Rows.Add(row);
            });
            dt.TableName = "DAT.PRODUCT";
            return dt;
        }

        public void InsertNewItem(MaestroProduct product)
        {
            SpCall call = new SpCall("DAT.PRODUCT_INSERT");

            call.SetVarchar("@PRODUCT_NAME", product.Name);
            call.SetVarchar("@PRODUCT_DESCRIPTION", product.Description);

            call.SetVarchar("@QB_PRODUCT_ID", product.QuickBooksProductId);
            call.SetDecimal("@PRICE", product.Price);

            call.SetInt("@MINIMUM_ORDER_QUANTITY", product.MinimumOrderQuantity);
            call.SetBigInt("@UNIT_TYPE_ID", product.UnitType.Id);
            call.SetBigInt("@PRODUCT_GROUP_ID", product.GroupId);
            call.SetDateTime("@CREATE_DATE", product.CreateDate);
            call.SetVarchar("@CREATE_USER", product.CreatedUser);
            product.Id = db.ExecuteScalar<long>(call);
        }

        public MaestroProduct GetUnknownItem()
        {
            MaestroProduct unknowProduct = ProductCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);

            if (unknowProduct == null)
            {
                unknowProduct = new MaestroProduct()
                {
                    ProductGroup = new ProductGroupManager(context).GetUnknownItem(),
                    Name = MaestroApplication.Instance.UNKNOWN_ITEM_NAME,
                    MinimumOrderQuantity = 0,
                    QuickBooksProductId = string.Empty,
                    Price = 0M,
                    UnitType = new UnitTypeManager(context).GetUnknownItem(),
                    Description = string.Empty,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedUser = "MAESTRO",
                    UpdatedUser = "MAESTRO",
                    RecordStatus = "A"
                    
                };
                InsertNewItem(unknowProduct);
                ProductCache.Instance.Reload(true);
            }

            return unknowProduct;
        }

    }
}
