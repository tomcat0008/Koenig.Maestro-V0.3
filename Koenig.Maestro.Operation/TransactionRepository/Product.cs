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
    internal class Product : TransactionBase
    {
        ProductManager pm;
        public Product(TransactionContext context) :base("PRODUCT", context)
        {
            this.IsProgressing = false;
            this.MainEntitySample = new MaestroProduct();
            pm = new ProductManager(context);
        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = ProductCache.Instance[id];

            pm.Delete(id);
            
        }


        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = ProductCache.Instance[id];
        }

        protected override void ImportQb()
        {
            //ImportRequestType importRequestType = EnumUtils.GetEnum<ImportRequestType>(extendedData[MessageDataExtensionKeys.IMPORT_TYPE]);
            /*
            string qbId = string.Empty;
            if (importRequestType == ImportRequestType.Single)
                qbId = extendedData[MessageDataExtensionKeys.ID];
            */
            List<MaestroProduct> qbProducts = null;

            using (qbAgent = new QuickBooksProductAgent(Context))
            {
                qbProducts = qbAgent.Import().Cast<MaestroProduct>().ToList();
            }

            if(qbProducts.Count > 0)
                responseMessage = string.Format("{0} products have been imported", qbProducts.Count);
            else
                responseMessage = "No products have been imported";
        }

        protected override void List()
        {
            response.TransactionResult = ProductCache.Instance.Values.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            
            MaestroProduct product = (MaestroProduct)request.TransactionEntityList[0];
            product.CreatedUser = Context.UserName;
            product.CreateDate = DateTime.Now;

            pm.InsertNewItem(product);
            response.TransactionResult = product;
            //Context.TransactionObject = product;
            
        }

        protected override void Update()
        {

            MaestroProduct product = (MaestroProduct)request.TransactionEntityList[0];

            pm.Update(product);
            Context.TransactionObject = product;

            
        }

        public override void RefreshCache(ActionType at)
        {
            ProductCache.Instance.Reload(true);
            if (at == ActionType.Delete)
                CustomerProductUnitCache.Instance.Reload(true);
        }

        protected override void Undelete()
        {
            throw new NotImplementedException();
        }

        protected override void Erase()
        {
            throw new NotImplementedException();
        }

        protected override void BackUp()
        {
            Guid guid = Guid.NewGuid();
            pm.BackUp(guid);

            new ProductGroupManager(Context).BackUp(guid);
            new QuickBooksProductMapManager(Context).BackUp(guid);
            new CustomerProductUnitManager(Context).BackUp(guid);


        }



    }
}
