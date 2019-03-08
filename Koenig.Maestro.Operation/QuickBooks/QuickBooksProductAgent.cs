using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interop.QBFC13;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Framework.ManagerRepository;

namespace Koenig.Maestro.Operation.QuickBooks
{
    internal class QuickBooksProductAgent : QuickBooksAgent
    {
        QuickBooksProductMapManager qmanager;
        ProductManager pm;
        UnitManager um;
        public QuickBooksProductAgent(TransactionContext context) : base(context)
        {
            pm = new ProductManager(context);
            um = new UnitManager(context);
            qmanager = new QuickBooksProductMapManager(context);
        }

        public override List<ITransactionEntity> Import()
        {
            StartSession();

            IMsgSetRequest request = GetLatestMsgSetRequest();
            IItemNonInventoryQuery query = request.AppendItemNonInventoryQueryRq();
            //IItemQuery query = request.AppendItemQueryRq();
            IResponse res = GetResponse(request);

            //IORItemRetList returnList = res.Detail as IORItemRetList;
            IItemNonInventoryRetList returnList = res.Detail as IItemNonInventoryRetList;

            List<MaestroProduct> plist = new List<MaestroProduct>();

            List<IItemNonInventoryRet> mapSourceList = new List<IItemNonInventoryRet>();

            MaestroUnitType unknowUnitType = new UnitTypeManager(context).GetUnknownItem();
            

            int cnt = returnList.Count;
            for (int i = 0; i <= cnt - 1; i++)
            {
                IItemNonInventoryRet inv = returnList.GetAt(i);
                if (inv != null)
                    if (ReadBool(inv.IsActive))
                    {
                        string parentId = string.Empty;
                        if(inv.ParentRef !=null)
                            parentId = ReadQbId(inv.ParentRef.ListID);

                        if (string.IsNullOrEmpty(parentId))
                        {
                            MaestroProduct product = GetMaestroProduct(inv);
                            plist.Add(product);
                        }
                        
                        mapSourceList.Add(inv);//for late processing
                    }

                #region commented
                //WalkItemNonInventoryRet(inv);

                //IORItemRet qbc = returnList.GetAt(i);
                /*
                if (qbc.ItemNonInventoryRet != null)
                {
                    WalkItemNonInventoryRet(qbc.ItemNonInventoryRet);
                    //GetMaestroProduct(qbc.ItemNonInventoryRet);
                }*/
                #endregion commented
            }

            ExecuteBulkInsert(plist);
            ImportMaps(mapSourceList, plist);
            QuickBooksProductMapCache.Instance.Reload(true);

            return plist.Cast<ITransactionEntity>().ToList();
        }

        void ImportMaps(List<IItemNonInventoryRet>  mapSourceList, List<MaestroProduct> plist)
        {

            if (mapSourceList.Count > 0)
            {
                List<QuickBooksProductMapDef> mapList = GetMapList(mapSourceList, plist);
                if (mapList.Count > 0)
                    qmanager.BulkInsert(mapList.Cast<ITransactionEntity>().ToList());
            }
        }

        void ExecuteBulkInsert(List<MaestroProduct> plist)
        {
            UnitTypeManager um = new UnitTypeManager(context);
            MaestroUnitType unknownUnitType = um.GetUnknownItem();

            List<MaestroProduct> updatedProducts = new List<MaestroProduct>();
            foreach (MaestroProduct product in plist)
            {
                MaestroProduct existing = ProductCache.Instance.GetByQbId(product.QuickBooksProductId);
                if (existing != null)
                {
                    product.Id = existing.Id;
                    product.UnitType = existing.UnitType;
                    pm.Update(product);
                    updatedProducts.Add(existing);
                }
                else
                    product.UnitType = unknownUnitType;
            }

            List<ITransactionEntity> clist = plist.Where(q => !updatedProducts.Select(u => u.QuickBooksProductId).Contains(q.QuickBooksProductId)).Cast<ITransactionEntity>().ToList();
            if (clist.Count > 0)
                pm.BulkInsert(clist);

            if(clist.Count > 0 || updatedProducts.Count>0)
                ProductCache.Instance.Reload(true);

        }

        void WalkItemNonInventoryRet(IItemNonInventoryRet ItemNonInventoryRet)
        {
            if (ItemNonInventoryRet == null) return;

            //Go through all the elements of IItemNonInventoryRetList
            //Get value of ListID
            string ListID48 = (string)ItemNonInventoryRet.ListID.GetValue();
            //Console.WriteLine(string.Format("ListID48:{0}", ListID48));
            //Get value of TimeCreated
            DateTime TimeCreated49 = (DateTime)ItemNonInventoryRet.TimeCreated.GetValue();
            //Console.WriteLine(string.Format("TimeCreated49:{0}", TimeCreated49));

            DateTime TimeModified50 = (DateTime)ItemNonInventoryRet.TimeModified.GetValue();
            //Get value of EditSequence
            string EditSequence51 = (string)ItemNonInventoryRet.EditSequence.GetValue();
            //Get value of Name
            string Name52 = (string)ItemNonInventoryRet.Name.GetValue();
            //Get value of FullName
            string FullName53 = (string)ItemNonInventoryRet.FullName.GetValue();
            //Get value of BarCodeValue
            string BarCodeValue54 = string.Empty;
            if (ItemNonInventoryRet.BarCodeValue != null)
            {
                BarCodeValue54 = (string)ItemNonInventoryRet.BarCodeValue.GetValue();
            }
            //Get value of IsActive
            bool IsActive55 = false;
            if (ItemNonInventoryRet.IsActive != null)
            {
                IsActive55 = (bool)ItemNonInventoryRet.IsActive.GetValue();
            }

            string ListID56 = string.Empty;
            string FullName57 = string.Empty;

            if (ItemNonInventoryRet.ClassRef != null)
            {
                //Get value of ListID
                if (ItemNonInventoryRet.ClassRef.ListID != null)
                {
                    ListID56 = (string)ItemNonInventoryRet.ClassRef.ListID.GetValue();
                }
                //Get value of FullName
                if (ItemNonInventoryRet.ClassRef.FullName != null)
                {
                    FullName57 = (string)ItemNonInventoryRet.ClassRef.FullName.GetValue();
                }
            }

            string ListID58 = string.Empty;
            string FullName59 = string.Empty;

            if (ItemNonInventoryRet.ParentRef != null)
            {
                //Get value of ListID
                if (ItemNonInventoryRet.ParentRef.ListID != null)
                {
                    ListID58 = (string)ItemNonInventoryRet.ParentRef.ListID.GetValue();
                }
                //Get value of FullName
                if (ItemNonInventoryRet.ParentRef.FullName != null)
                {
                    FullName59 = (string)ItemNonInventoryRet.ParentRef.FullName.GetValue();
                }
            }
            //Get value of Sublevel
            int Sublevel60 = (int)ItemNonInventoryRet.Sublevel.GetValue();
            //Get value of ManufacturerPartNumber
            string ManufacturerPartNumber61 = string.Empty;
            if (ItemNonInventoryRet.ManufacturerPartNumber != null)
            {
                ManufacturerPartNumber61 = (string)ItemNonInventoryRet.ManufacturerPartNumber.GetValue();
            }
            string ListID62, FullName63;
            if (ItemNonInventoryRet.UnitOfMeasureSetRef != null)
            {
                //Get value of ListID
                if (ItemNonInventoryRet.UnitOfMeasureSetRef.ListID != null)
                {
                    ListID62 = (string)ItemNonInventoryRet.UnitOfMeasureSetRef.ListID.GetValue();
                }
                //Get value of FullName
                if (ItemNonInventoryRet.UnitOfMeasureSetRef.FullName != null)
                {
                    FullName63 = (string)ItemNonInventoryRet.UnitOfMeasureSetRef.FullName.GetValue();
                }
            }
            string ListID64, FullName65;
            if (ItemNonInventoryRet.SalesTaxCodeRef != null)
            {
                //Get value of ListID
                if (ItemNonInventoryRet.SalesTaxCodeRef.ListID != null)
                {
                    ListID64 = (string)ItemNonInventoryRet.SalesTaxCodeRef.ListID.GetValue();
                }
                //Get value of FullName
                if (ItemNonInventoryRet.SalesTaxCodeRef.FullName != null)
                {
                    FullName65 = (string)ItemNonInventoryRet.SalesTaxCodeRef.FullName.GetValue();
                }
            }
            string Desc66, ListID69, FullName70;
            double Price67 = 0, PricePercent68 = 0;
            string SalesDesc71, ListID73, FullName74, PurchaseDesc75, ListID77;
            string FullName78, ListID79, FullName80;
            double SalesPrice72 = 0, PurchaseCost76 = 0;
            if (ItemNonInventoryRet.ORSalesPurchase != null)
            {
                if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase != null)
                {
                    if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase != null)
                    {
                        //Get value of Desc
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.Desc != null)
                        {
                            Desc66 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.Desc.GetValue();
                        }
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice != null)
                        {
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price != null)
                            {
                                //Get value of Price
                                if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price != null)
                                {
                                    Price67 = (double)ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price.GetValue();
                                }
                            }
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice.PricePercent != null)
                            {
                                //Get value of PricePercent
                                if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice.PricePercent != null)
                                {
                                    PricePercent68 = (double)ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.ORPrice.PricePercent.GetValue();
                                }
                            }
                        }
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.AccountRef != null)
                        {
                            //Get value of ListID
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.AccountRef.ListID != null)
                            {
                                ListID69 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.AccountRef.ListID.GetValue();
                            }
                            //Get value of FullName
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.AccountRef.FullName != null)
                            {
                                FullName70 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesOrPurchase.AccountRef.FullName.GetValue();
                            }
                        }
                    }
                }

                if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase != null)
                {
                    if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase != null)
                    {
                        //Get value of SalesDesc
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.SalesDesc != null)
                        {
                            SalesDesc71 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.SalesDesc.GetValue();
                        }
                        //Get value of SalesPrice
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.SalesPrice != null)
                        {
                            SalesPrice72 = (double)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.SalesPrice.GetValue();
                        }
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.IncomeAccountRef != null)
                        {
                            //Get value of ListID
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.IncomeAccountRef.ListID != null)
                            {
                                ListID73 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.IncomeAccountRef.ListID.GetValue();
                            }
                            //Get value of FullName
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.IncomeAccountRef.FullName != null)
                            {
                                FullName74 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.IncomeAccountRef.FullName.GetValue();
                            }
                        }
                        //Get value of PurchaseDesc
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PurchaseDesc != null)
                        {
                            PurchaseDesc75 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PurchaseDesc.GetValue();
                        }
                        //Get value of PurchaseCost
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PurchaseCost != null)
                        {
                            PurchaseCost76 = (double)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PurchaseCost.GetValue();
                        }
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.ExpenseAccountRef != null)
                        {
                            //Get value of ListID
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.ExpenseAccountRef.ListID != null)
                            {
                                ListID77 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.ExpenseAccountRef.ListID.GetValue();
                            }
                            //Get value of FullName
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.ExpenseAccountRef.FullName != null)
                            {
                                FullName78 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.ExpenseAccountRef.FullName.GetValue();
                            }
                        }
                        if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PrefVendorRef != null)
                        {
                            //Get value of ListID
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PrefVendorRef.ListID != null)
                            {
                                ListID79 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PrefVendorRef.ListID.GetValue();
                            }
                            //Get value of FullName
                            if (ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PrefVendorRef.FullName != null)
                            {
                                FullName80 = (string)ItemNonInventoryRet.ORSalesPurchase.SalesAndPurchase.PrefVendorRef.FullName.GetValue();

                            }
                        }
                    }
                }
            }
            //Get value of ExternalGUID
            string ExternalGUID81;
            if (ItemNonInventoryRet.ExternalGUID != null)
            {
                ExternalGUID81 = (string)ItemNonInventoryRet.ExternalGUID.GetValue();
            }
            if (ItemNonInventoryRet.DataExtRetList != null)
            {
                for (int i82 = 0; i82 < ItemNonInventoryRet.DataExtRetList.Count; i82++)
                {
                    IDataExtRet DataExtRet = ItemNonInventoryRet.DataExtRetList.GetAt(i82);
                    //Get value of OwnerID
                    if (DataExtRet.OwnerID != null)
                    {
                        string OwnerID83 = (string)DataExtRet.OwnerID.GetValue();
                    }
                    //Get value of DataExtName
                    string DataExtName84 = (string)DataExtRet.DataExtName.GetValue();
                    //Get value of DataExtType
                    ENDataExtType DataExtType85 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                    //Get value of DataExtValue
                    string DataExtValue86 = (string)DataExtRet.DataExtValue.GetValue();
                }
            }
        }

        List<QuickBooksProductMapDef> GetMapList(List<IItemNonInventoryRet> mapSourceList, List<MaestroProduct> plist)
        {
            
            List<QuickBooksProductMapDef> mapList = new List<QuickBooksProductMapDef>();
            mapSourceList.ForEach(ms =>
            {
                QuickBooksProductMapDef map = GetMap(ms);
                MaestroProduct product = null;
                if (string.IsNullOrWhiteSpace(map.QuickBooksParentListId))
                    product = ProductCache.Instance.GetByQbId(map.QuickBooksListId);
                else
                    product = ProductCache.Instance.GetByQbId(map.QuickBooksParentListId);
                if (product == null)
                    product = pm.GetUnknownItem();
                map.Product = product;
                map.Price = product.Price;
                QuickBooksProductMapDef existing = QuickBooksProductMapCache.Instance.GetByQbId(map.QuickBooksListId);
                if (existing != null)
                {
                    map.Id = existing.Id;
                    map.Unit = existing.Unit;
                    qmanager.Update(map);
                }
                else
                {
                    mapList.Add(map);
                }
            });

            return mapList;
        }

        QuickBooksProductMapDef GetMap(IItemNonInventoryRet item)
        {
            QuickBooksProductMapDef result = new QuickBooksProductMapDef()
            {
                QuickBooksCode = ReadString(item.Name),
                QuickBooksDescription = ReadString(item.ORSalesPurchase.SalesOrPurchase.Desc),
                QuickBooksListId = ReadQbId(item.ListID),
                QuickBooksParentCode = item.ParentRef == null ? string.Empty : ReadString(item.ParentRef.FullName),
                QuickBooksParentListId = item.ParentRef == null ? string.Empty : ReadQbId(item.ParentRef.ListID),
                //Price = ReadPrice(item.ORSalesPurchase.SalesOrPurchase.ORPrice.Price),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreatedUser = context.UserName,
                UpdatedUser = context.UserName,
                RecordStatus = "A",
                Unit = um.GetUnknownItem()
            };
            return result;
        }
        
        MaestroProduct GetMaestroProduct(IItemNonInventoryRet item)
        {
            MaestroProduct result = new MaestroProduct()
            {
                Name = ReadString(item.ORSalesPurchase.SalesOrPurchase.Desc),
                Description = ReadString(item.Name),
                GroupId = 0,
                MinimumOrderQuantity = 0,
                Price = ReadPrice(item.ORSalesPurchase.SalesOrPurchase.ORPrice.Price),
                QuickBooksProductId = ReadQbId(item.ListID),
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                UpdatedUser = context.UserName,
                CreatedUser = context.UserName,
                RecordStatus = "A"
                
        };

            return result;

        }



    }
}
