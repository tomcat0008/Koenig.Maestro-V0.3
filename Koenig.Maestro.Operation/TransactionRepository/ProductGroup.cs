using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Koenig.Maestro.Operation.QuickBooks;
using Koenig.Maestro.Operation.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class ProductGroup: TransactionBase
    {
        ProductGroupManager pm;
        public ProductGroup(TransactionContext context) : base("PRODUCT_GROUP", context)
        {
            pm = new ProductGroupManager(context);
        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = ProductGroupCache.Instance[id];

            pm.Delete(id);

        }


        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = ProductGroupCache.Instance[id];
        }



        protected override void List()
        {
            response.TransactionResult = ProductGroupCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {

            MaestroProductGroup productGroup = (MaestroProductGroup)request.TransactionEntityList[0];
            productGroup.CreatedUser = Context.UserName;
            productGroup.CreateDate = DateTime.Now;

            pm.InsertNewItem(productGroup);
            response.TransactionResult = productGroup;
            //Context.TransactionObject = product;

        }

        protected override void Update()
        {

            MaestroProductGroup productGroup = (MaestroProductGroup)request.TransactionEntityList[0];

            pm.Update(productGroup);
            Context.TransactionObject = productGroup;


        }

        public override void RefreshCache(ActionType at)
        {
            ProductGroupCache.Instance.Reload(true);
            if (at == ActionType.Delete)
                ProductGroupCache.Instance.Reload(true);
        }

    }
}
