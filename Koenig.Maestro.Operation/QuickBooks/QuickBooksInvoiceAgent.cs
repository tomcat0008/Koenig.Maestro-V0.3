using System;
using System.Collections.Generic;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using Interop.QBFC13;
using System.Linq;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Koenig.Maestro.Operation.Cache.CacheRepository;

namespace Koenig.Maestro.Operation.QuickBooks
{
    internal class QuickBooksInvoiceAgent : QuickBooksAgent
    {

        OrderManager orderManager = null;
        QuickBooksInvoiceManager invoiceManager = null;

        public QuickBooksInvoiceAgent(TransactionContext context) : base(context)
        {
            orderManager = new OrderManager(context);
            invoiceManager = new QuickBooksInvoiceManager(context);
        }

        public override void Export()
        {
            //ExportRequestType exportRequestType = EnumUtils.GetEnum<ExportRequestType>(extendedData[MessageDataExtensionKeys.EXPORT_TYPE]);
            OrderMaster om = (OrderMaster)context.TransactionObject;
            long orderId = om.Id;

            
            StartSession();

            IMsgSetRequest request = GetLatestMsgSetRequest();

            BuildInvoice(request, om);

            IResponse responseMsgSet = GetResponse(request);
            if (responseMsgSet.StatusCode != 0)
                throw new Exception(string.Format("Exception occured while requesting export operation on Quickbooks:{0}{1} (StatusCode:{2})", Environment.NewLine, responseMsgSet.StatusMessage, responseMsgSet.StatusCode));
            IInvoiceRet qbInvoice = responseMsgSet.Detail as IInvoiceRet;

            if (context.Bag.ContainsKey("REF_NUMBER"))
                context.Bag["REF_NUMBER"] = qbInvoice.RefNumber.GetValue();
            else
                context.Bag.Add("REF_NUMBER", qbInvoice.RefNumber.GetValue());

            if (context.Bag.ContainsKey("TXN_ID"))
                context.Bag["TXN_ID"] = qbInvoice.TxnID.GetValue();
            else
                context.Bag.Add("TXN_ID", qbInvoice.TxnID.GetValue());

            
        }

        


        IInvoiceAdd BuildInvoice(IMsgSetRequest request, OrderMaster om)
        {
            IInvoiceAdd invoice = request.AppendInvoiceAddRq();
            invoice.CustomerRef.ListID.SetValue(om.Customer.QuickBooksId);

            foreach (OrderItem oi in om.OrderItems)
            {
                IORInvoiceLineAdd item = invoice.ORInvoiceLineAddList.Append();
                item.InvoiceLineAdd.ItemRef.ListID.SetValue(oi.QbProductMap.QuickBooksListId);
                item.InvoiceLineAdd.Quantity.SetValue(oi.Quantity);
                //item.InvoiceLineAdd.ORRatePriceLevel.RatePercent.SetValue(3.2D);
                //item.InvoiceLineAdd.ORRatePriceLevel.Rate.SetValue(4.2D);

                IDataExt extendedData = item.InvoiceLineAdd.DataExtList.Append();
                extendedData.DataExtName.SetValue("QTY ORD");
                extendedData.DataExtValue.SetValue(oi.Quantity.ToString());
                extendedData.OwnerID.SetValue("0");
            }

            return invoice;
        }


        public override void Update()
        {
            StartSession();
            IMsgSetRequest request = GetLatestMsgSetRequest();

            OrderMaster om = (OrderMaster)context.TransactionObject;

            IInvoiceRet qbInvoice = GetInvoiceFromQuickBooks(om.InvoiceLog.QuickBooksInvoiceId.ToString());

            IInvoiceMod modify = request.AppendInvoiceModRq();
            modify.TxnID.SetValue(qbInvoice.TxnID.GetValue());
            modify.EditSequence.SetValue(qbInvoice.EditSequence.GetValue());


            PrepareLineModifications(om.OrderItems, modify, qbInvoice, request);

            IResponse responseMsgSet = GetResponse(request);
            if (responseMsgSet.StatusCode != 0)
                throw new Exception(string.Format("Exception occured while requesting update operation on Quickbooks:{0}{1} (StatusCode:{2})", Environment.NewLine, responseMsgSet.StatusMessage, responseMsgSet.StatusCode));

        }

        void PrepareLineModifications(List<OrderItem> items, IInvoiceMod modification, IInvoiceRet qbInvoice, IMsgSetRequest request)
        {
            //these are either updated or deleted, compare to add new lines to invoice
            List<string> qbProductsList = new List<string>();
            IORInvoiceLineRetList qbInvoiceLines = qbInvoice.ORInvoiceLineRetList;
            string invoiceTxn = qbInvoice.TxnID.GetValue();
            //1 remove && update deleted items
            for (int i=0;i<qbInvoiceLines.Count; i++)
            {
                IORInvoiceLineRet qbInvoiceLine = qbInvoiceLines.GetAt(i);
                string qbProductId = (string)qbInvoiceLine.InvoiceLineRet.ItemRef.ListID.GetValue();
                string txnId = qbInvoiceLine.InvoiceLineRet.TxnLineID.GetValue();
                qbProductsList.Add(qbProductId);
                
                OrderItem orderItem = items.Find(oi=>oi.QbProductMap.QuickBooksListId.Equals(qbProductId));
                if (orderItem != null)//updates are there
                {
                    double oldQuantity = qbInvoiceLine.InvoiceLineRet.Quantity.GetValue();

                    //only quantity change counts
                    if (Convert.ToDouble(orderItem.Quantity) != oldQuantity)
                        AppendUpdateModification(request, orderItem, qbProductId, txnId, modification, invoiceTxn);
                }

            }

            //add new lines
            items.Where(oi => !qbProductsList.Contains(oi.QbProductMap.QuickBooksListId)).ToList().ForEach(oi =>
              {
                  AppendInsertModifications(modification, request, oi);
              });
            

        }

        void AppendInsertModifications(IInvoiceMod modification, IMsgSetRequest request, OrderItem item)
        {
            IORInvoiceLineMod lineMod = modification.ORInvoiceLineModList.Append();
            lineMod.InvoiceLineMod.Quantity.SetValue(item.Quantity);
            lineMod.InvoiceLineMod.ItemRef.ListID.SetValue(item.QbProductMap.QuickBooksListId);
            lineMod.InvoiceLineMod.TxnLineID.SetValue("-1");

            IDataExtMod extendMod = request.AppendDataExtModRq();
            extendMod.DataExtName.SetValue("QTY ORD");
            extendMod.DataExtValue.SetValue(item.Quantity.ToString());
            extendMod.OwnerID.SetValue("0");
            extendMod.ORListTxn.ListDataExt.ListDataExtType.SetValue(ENListDataExtType.ldetItem);
            extendMod.ORListTxn.ListDataExt.ListObjRef.ListID.SetValue(item.QbProductMap.QuickBooksListId);

            /*
            extendMod.ORListTxn.TxnDataExt.TxnLineID.SetValue(lineTxnId);
            extendMod.ORListTxn.TxnDataExt.TxnID.SetValue(invoiceTxn);
            extendMod.ORListTxn.TxnDataExt.TxnDataExtType.SetValue(ENTxnDataExtType.tdetInvoice);
            */

        }


        void AppendUpdateModification(IMsgSetRequest request, OrderItem orderItem, string qbProductId, string lineTxnId, IInvoiceMod modification, string invoiceTxn)
        {
            IDataExtMod extendMod = request.AppendDataExtModRq();
            extendMod.DataExtName.SetValue("QTY ORD");
            extendMod.DataExtValue.SetValue(orderItem.Quantity.ToString());
            extendMod.OwnerID.SetValue("0");
            extendMod.ORListTxn.TxnDataExt.TxnLineID.SetValue(lineTxnId);
            extendMod.ORListTxn.TxnDataExt.TxnID.SetValue(invoiceTxn);
            extendMod.ORListTxn.TxnDataExt.TxnDataExtType.SetValue(ENTxnDataExtType.tdetInvoice);
            //extendMod.ORListTxn.ListDataExt.ListObjRef.ListID.SetValue(listIdVal);
            //extendMod.ORListTxn.ListDataExt.ListDataExtType.SetValue(ENListDataExtType.ldetCustomer);

            IORInvoiceLineMod lineMod = modification.ORInvoiceLineModList.Append();
            lineMod.InvoiceLineMod.TxnLineID.SetValue(lineTxnId);
            lineMod.InvoiceLineMod.Quantity.SetValue(orderItem.Quantity);
            lineMod.InvoiceLineMod.ItemRef.ListID.SetValue(qbProductId);
        }

        IInvoiceRet GetInvoiceFromQuickBooks(string qbRef)
        {
            IMsgSetRequest request = GetLatestMsgSetRequest();
            IInvoiceQuery query = request.AppendInvoiceQueryRq();
            query.IncludeLineItems.SetValue(true);
            query.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
            query.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(qbRef);
            query.ORInvoiceQuery.InvoiceFilter.MaxReturned.SetValue(1);

            IResponse res = GetResponse(request);
            IInvoiceRetList invoiceRetList = (IInvoiceRetList)res.Detail;

            return invoiceRetList.GetAt(0);

        }


        public override void Cancel()
        {
            StartSession();
            //OrderMaster om = (OrderMaster)context.TransactionObject;

            QuickBooksInvoiceLog log = (QuickBooksInvoiceLog)context.TransactionObject;

            IMsgSetRequest request = GetLatestMsgSetRequest();
            ITxnDel del = request.AppendTxnDelRq();
            del.TxnDelType.SetValue(ENTxnDelType.tdtInvoice);
            del.TxnID.SetValue(log.QuickBooksTxnId);

            IResponse response = GetResponse(request);

            if (response.StatusCode > 0)
                throw new Exception(string.Format("Exception occured while requesting delete operation on Quickbooks:{0}{1} (StatusCode:{2})", Environment.NewLine, response.StatusMessage, response.StatusCode));

        }

        public override List<ITransactionEntity> Import()
        {
            List<ITransactionEntity> result = new List<ITransactionEntity>();
            StartSession();
            IMsgSetRequest request = GetLatestMsgSetRequest();
            IInvoiceQuery query = request.AppendInvoiceQueryRq();
            query.IncludeLineItems.SetValue(true);
            query.ORInvoiceQuery.InvoiceFilter.MaxReturned.SetValue(1000);
            //query.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
            //query.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue("89315");

            query.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.FromTxnDate.SetValue((DateTime)context.Bag[MessageDataExtensionKeys.BEGIN_DATE]);
            query.ORInvoiceQuery.InvoiceFilter.ORDateRangeFilter.TxnDateRangeFilter.ORTxnDateRangeFilter.TxnDateFilter.ToTxnDate.SetValue((DateTime)context.Bag[MessageDataExtensionKeys.END_DATE]);

            IResponse res = GetResponse(request);
            IInvoiceRetList invoiceRetList = (IInvoiceRetList)res.Detail;

            for(int i=0;i<invoiceRetList.Count;i++)
            {
                IInvoiceRet invoice = invoiceRetList.GetAt(i);
                string txnID = invoice.TxnID.GetValue();
                QuickBooksInvoiceLog log = invoiceManager.GetInvoiceLog(txnID);
                if (log != null)
                {

                    UpdateOrder(invoice, log);
                    
                    //context.Warnings.Add(string.Format("Skipping txnId:{0}, order {1} already exists", txnID, log.OrderId));
                }
                else
                    GetOrder(invoice);

                //WalkInvoiceRet(invoice);

            }


            return result;
            
        }

        void UpdateOrder(IInvoiceRet qbInvoice, QuickBooksInvoiceLog log)
        {
            OrderMaster om = orderManager.GetOrder(log.OrderId);


            for (int i = 0; i < qbInvoice.ORInvoiceLineRetList.Count; i++)
            {
                IORInvoiceLineRet line = qbInvoice.ORInvoiceLineRetList.GetAt(i);

                double quantity = line.InvoiceLineRet.Quantity != null ? line.InvoiceLineRet.Quantity.GetValue() : 0;
                if (quantity > 0)
                {
                    string productRefID = line.InvoiceLineRet.ItemRef.ListID.GetValue();
                    QuickBooksProductMapDef map = QuickBooksProductMapCache.Instance.GetByQbId(productRefID);
                    OrderItem item = om.OrderItems.Find(oi => oi.MapId == map.Id);
                    decimal amount = line.InvoiceLineRet.Amount != null ? Convert.ToDecimal(line.InvoiceLineRet.Amount.GetValue()) : 0;
                    DateTime orderDate = qbInvoice.TimeCreated.GetValue();
                    if (item == null)
                    {
                        item = new OrderItem();
                        item.OrderId = om.Id;
                        item.CreateDate = orderDate;
                        item.CreatedUser = "IMPORT";

                        item.Amount = amount;
                        
                        item.Product = map.Product;
                        item.Price = map.Price;
                        item.QbProductMap = map;

                        item.Unit = map.Unit;
                        item.UpdateDate = DateTime.Now;
                        item.UpdatedUser = context.UserName;

                        orderManager.InsertOrderItem(item);
                    }
                    else
                    {
                        if (item.Quantity != quantity || item.Amount != amount)
                        {
                            item.Quantity = Convert.ToInt32(quantity);
                            item.Amount = amount;
                            orderManager.UpdateOrderItem(item);
                        }
                        
                    }

                }
            }


        }

        void GetOrder(IInvoiceRet qbInvoice)
        {
            string txnID = qbInvoice.TxnID.GetValue(); //log txn
            string qbInvoiceNo = qbInvoice.RefNumber.GetValue();

            if (!context.Bag.ContainsKey("QB_INVOICE_ID"))
                context.Bag.Add("QB_INVOICE_ID", qbInvoiceNo);
            else
                context.Bag["QB_INVOICE_ID"] = qbInvoiceNo;


            if (!context.Bag.ContainsKey("QB_TXN_ID"))
                context.Bag.Add("QB_TXN_ID", txnID);
            else
                context.Bag["QB_TXN_ID"] = txnID;

            string customerRef = qbInvoice.CustomerRef.ListID.GetValue(); //customer id
            
            DateTime orderDate = qbInvoice.TimeCreated.GetValue();
            DateTime shipDate = orderDate;
            if (qbInvoice.ShipDate != null)
                shipDate = qbInvoice.ShipDate.GetValue();

            MaestroCustomer customer = CustomerCache.Instance.GetByQbId(customerRef);
            if (customer == null)
                customer = new CustomerManager(context).GetUnknownItem();

            long orderId = orderManager.GetNewOrderId();

            OrderMaster order = new OrderMaster()
            {
                Id = orderId,
                CreateDate = DateTime.Now,
                CreatedUser = "IMPORT",
                Customer = customer,
                Notes = "Order imported from Quickbooks",
                PaymentType = string.Empty,
                UpdateDate = DateTime.Now,
                UpdatedUser = "IMPORT",
                OrderStatus = OrderStatus.IMPORTED,
                RecordStatus = "A",
                OrderDate = orderDate,
                DeliveryDate = orderDate
               
            };

            order.OrderItems = ExtractOrderItems(qbInvoice, orderId);
            orderManager.InsertOrder(order);

            QuickBooksInvoiceLog log = invoiceManager.CreateInvoiceLog(order, QbIntegrationLogStatus.OK);
            invoiceManager.InsertIntegrationLog(log);

        }


        List<OrderItem> ExtractOrderItems(IInvoiceRet qbInvoice, long orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            
            DateTime orderDate = qbInvoice.TimeCreated.GetValue();

            for (int i=0;i< qbInvoice.ORInvoiceLineRetList.Count;i++)
            {
                IORInvoiceLineRet line = qbInvoice.ORInvoiceLineRetList.GetAt(i);
                
                double quantity = line.InvoiceLineRet.Quantity != null ? line.InvoiceLineRet.Quantity.GetValue() : 0;
                if (quantity > 0)
                {
                    string productRefID = line.InvoiceLineRet.ItemRef.ListID.GetValue();
                    QuickBooksProductMapDef map = QuickBooksProductMapCache.Instance.GetByQbId(productRefID);

                    OrderItem item = new OrderItem();
                    item.OrderId = orderId;
                    item.CreateDate = orderDate;
                    item.Amount = line.InvoiceLineRet.Amount != null ? Convert.ToDecimal(line.InvoiceLineRet.Amount.GetValue()) : 0;
                    item.CreatedUser = "IMPORT";
                    item.Product = map.Product;
                    item.Price = map.Price;
                    item.QbProductMap = map;
                    item.Quantity = Convert.ToInt32(quantity);
                    item.Unit = map.Unit;
                    item.UpdateDate = orderDate;
                    item.UpdatedUser = context.UserName;
                    orderItems.Add(item);
                }
            }

            return orderItems;
        }



        void WalkInvoiceRet(IInvoiceRet InvoiceRet)
        {
            if (InvoiceRet == null) return;

            //Go through all the elements of IInvoiceRetList
            //Get value of TxnID
            string TxnID8 = (string)InvoiceRet.TxnID.GetValue();
            logger.Debug(string.Format("TxnID8 {0}", TxnID8));
            //Get value of TimeCreated
            DateTime TimeCreated9 = (DateTime)InvoiceRet.TimeCreated.GetValue();
            logger.Debug(string.Format("TimeCreated9 {0}", TimeCreated9));
            //Get value of TimeModified
            DateTime TimeModified10 = (DateTime)InvoiceRet.TimeModified.GetValue();
            logger.Debug(string.Format("TimeModified10 {0}", TimeModified10));
            //Get value of EditSequence
            string EditSequence11 = (string)InvoiceRet.EditSequence.GetValue();
            logger.Debug(string.Format("EditSequence11 {0}", EditSequence11));
            //Get value of TxnNumber
            int TxnNumber12;
            string RefNumber22, Addr123, Addr224, Addr325, Addr426, Addr527, City28, State29, PostalCode30, Country31, Note32;
            if (InvoiceRet.TxnNumber != null)
            {
                TxnNumber12 = (int)InvoiceRet.TxnNumber.GetValue();
                logger.Debug(string.Format("TxnNumber12 {0}", TxnNumber12));
            }
            //Get value of ListID
            string ListID13;
            if (InvoiceRet.CustomerRef.ListID != null)
            {
                ListID13 = (string)InvoiceRet.CustomerRef.ListID.GetValue();
                logger.Debug(string.Format("ListID13 {0}", ListID13));
            }
            string FullName14;
            //Get value of FullName
            if (InvoiceRet.CustomerRef.FullName != null)
            {
                FullName14 = (string)InvoiceRet.CustomerRef.FullName.GetValue();
                logger.Debug(string.Format("FullName14 {0}", FullName14));
            }
            string ListID15, FullName16, ListID17, FullName18, ListID19, FullName20;
            if (InvoiceRet.ClassRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.ClassRef.ListID != null)
                {
                    ListID15 = (string)InvoiceRet.ClassRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID15 {0}", ListID15));
                }
                //Get value of FullName
                if (InvoiceRet.ClassRef.FullName != null)
                {
                    FullName16 = (string)InvoiceRet.ClassRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName16 {0}", FullName16));
                }
            }
            if (InvoiceRet.ARAccountRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.ARAccountRef.ListID != null)
                {
                    ListID17 = (string)InvoiceRet.ARAccountRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID17 {0}", ListID17));
                }
                //Get value of FullName
                if (InvoiceRet.ARAccountRef.FullName != null)
                {
                    FullName18 = (string)InvoiceRet.ARAccountRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName18 {0}", FullName18));
                }
            }
            if (InvoiceRet.TemplateRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.TemplateRef.ListID != null)
                {
                    ListID19 = (string)InvoiceRet.TemplateRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID19 {0}", ListID19));
                }
                //Get value of FullName
                if (InvoiceRet.TemplateRef.FullName != null)
                {
                    FullName20 = (string)InvoiceRet.TemplateRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName20 {0}", FullName20));
                }
            }
            //Get value of TxnDate
            DateTime TxnDate21 = (DateTime)InvoiceRet.TxnDate.GetValue();
            logger.Debug(string.Format("TxnDate21 {0}", TxnDate21));
            //Get value of RefNumber
            if (InvoiceRet.RefNumber != null)
            {
                RefNumber22 = (string)InvoiceRet.RefNumber.GetValue();
                logger.Debug(string.Format("RefNumber22 {0}", RefNumber22));
            }
            if (InvoiceRet.BillAddress != null)
            {
                //Get value of Addr1
                if (InvoiceRet.BillAddress.Addr1 != null)
                {
                    Addr123 = (string)InvoiceRet.BillAddress.Addr1.GetValue();
                    logger.Debug(string.Format("Addr123 {0}", Addr123));
                }
                //Get value of Addr2
                if (InvoiceRet.BillAddress.Addr2 != null)
                {
                    Addr224 = (string)InvoiceRet.BillAddress.Addr2.GetValue();
                    logger.Debug(string.Format("Addr224 {0}", Addr224));
                }
                //Get value of Addr3
                if (InvoiceRet.BillAddress.Addr3 != null)
                {
                    Addr325 = (string)InvoiceRet.BillAddress.Addr3.GetValue();
                    logger.Debug(string.Format("Addr325 {0}", Addr325));
                }
                //Get value of Addr4
                if (InvoiceRet.BillAddress.Addr4 != null)
                {
                    Addr426 = (string)InvoiceRet.BillAddress.Addr4.GetValue();
                    logger.Debug(string.Format("Addr426 {0}", Addr426));
                }
                //Get value of Addr5
                if (InvoiceRet.BillAddress.Addr5 != null)
                {
                    Addr527 = (string)InvoiceRet.BillAddress.Addr5.GetValue();
                    logger.Debug(string.Format("Addr527 {0}", Addr527));
                }
                //Get value of City
                if (InvoiceRet.BillAddress.City != null)
                {
                    City28 = (string)InvoiceRet.BillAddress.City.GetValue();
                    logger.Debug(string.Format("City28 {0}", City28));
                }
                //Get value of State
                if (InvoiceRet.BillAddress.State != null)
                {
                    State29 = (string)InvoiceRet.BillAddress.State.GetValue();
                    logger.Debug(string.Format("State29 {0}", State29));
                }
                //Get value of PostalCode
                if (InvoiceRet.BillAddress.PostalCode != null)
                {
                    PostalCode30 = (string)InvoiceRet.BillAddress.PostalCode.GetValue();
                    logger.Debug(string.Format("PostalCode30 {0}", PostalCode30));
                }
                
                //Get value of Country
                if (InvoiceRet.BillAddress.Country != null)
                {
                    Country31 = (string)InvoiceRet.BillAddress.Country.GetValue();
                    logger.Debug(string.Format("Country31 {0}", Country31));
                }
                //Get value of Note
                if (InvoiceRet.BillAddress.Note != null)
                {
                    Note32 = (string)InvoiceRet.BillAddress.Note.GetValue();
                    logger.Debug(string.Format("Note32 {0}", Note32));
                }
            }

            string Addr133,Addr234,Addr335,Addr436,Addr537;
            if (InvoiceRet.BillAddressBlock != null)
            {
                //Get value of Addr1
                if (InvoiceRet.BillAddressBlock.Addr1 != null)
                {
                    Addr133 = (string)InvoiceRet.BillAddressBlock.Addr1.GetValue();
                    logger.Debug(string.Format("Addr133 {0}", Addr133));
                }
                //Get value of Addr2
                if (InvoiceRet.BillAddressBlock.Addr2 != null)
                {
                    Addr234 = (string)InvoiceRet.BillAddressBlock.Addr2.GetValue();
                    logger.Debug(string.Format("Addr234 {0}", Addr234));
                }
                //Get value of Addr3
                if (InvoiceRet.BillAddressBlock.Addr3 != null)
                {
                    Addr335 = (string)InvoiceRet.BillAddressBlock.Addr3.GetValue();
                    logger.Debug(string.Format("Addr335 {0}", Addr335));
                }
                //Get value of Addr4
                if (InvoiceRet.BillAddressBlock.Addr4 != null)
                {
                    Addr436 = (string)InvoiceRet.BillAddressBlock.Addr4.GetValue();
                    logger.Debug(string.Format("Addr436 {0}", Addr436));
                }
                //Get value of Addr5
                if (InvoiceRet.BillAddressBlock.Addr5 != null)
                {
                    Addr537 = (string)InvoiceRet.BillAddressBlock.Addr5.GetValue();
                    logger.Debug(string.Format("Addr537 {0}", Addr537));
                }
            }

            string Addr138, Addr239, Addr340, Addr441, Addr542;
            string City43, State44, PostalCode45, Country46, Note47;
            if (InvoiceRet.ShipAddress != null)
            {
                //Get value of Addr1
                if (InvoiceRet.ShipAddress.Addr1 != null)
                {
                    Addr138 = (string)InvoiceRet.ShipAddress.Addr1.GetValue();
                    logger.Debug(string.Format("Addr138 {0}", Addr138));
                }
                //Get value of Addr2
                if (InvoiceRet.ShipAddress.Addr2 != null)
                {
                    Addr239 = (string)InvoiceRet.ShipAddress.Addr2.GetValue();
                    logger.Debug(string.Format("Addr239 {0}", Addr239));
                }
                //Get value of Addr3
                if (InvoiceRet.ShipAddress.Addr3 != null)
                {
                    Addr340 = (string)InvoiceRet.ShipAddress.Addr3.GetValue();
                    logger.Debug(string.Format("Addr340 {0}", Addr340));
                }
                //Get value of Addr4
                if (InvoiceRet.ShipAddress.Addr4 != null)
                {
                    Addr441 = (string)InvoiceRet.ShipAddress.Addr4.GetValue();
                    logger.Debug(string.Format("Addr441 {0}", Addr441));
                }
                //Get value of Addr5
                if (InvoiceRet.ShipAddress.Addr5 != null)
                {
                    Addr542 = (string)InvoiceRet.ShipAddress.Addr5.GetValue();
                    logger.Debug(string.Format("Addr542 {0}", Addr542));
                }
                //Get value of City
                if (InvoiceRet.ShipAddress.City != null)
                {
                    City43 = (string)InvoiceRet.ShipAddress.City.GetValue();
                    logger.Debug(string.Format("City43 {0}", City43));
                }
                //Get value of State
                
                if (InvoiceRet.ShipAddress.State != null)
                {
                    State44 = (string)InvoiceRet.ShipAddress.State.GetValue();
                    logger.Debug(string.Format("State44 {0}", State44));
                }
                //Get value of PostalCode
                if (InvoiceRet.ShipAddress.PostalCode != null)
                {
                    PostalCode45 = (string)InvoiceRet.ShipAddress.PostalCode.GetValue();
                    logger.Debug(string.Format("PostalCode45 {0}", PostalCode45));
                }
                //Get value of Country
                if (InvoiceRet.ShipAddress.Country != null)
                {
                    Country46 = (string)InvoiceRet.ShipAddress.Country.GetValue();
                    logger.Debug(string.Format("Country46 {0}", Country46));
                }
                //Get value of Note
                if (InvoiceRet.ShipAddress.Note != null)
                {
                    Note47 = (string)InvoiceRet.ShipAddress.Note.GetValue();
                    logger.Debug(string.Format("Note47 {0}", Note47));
                }
            }
            if (InvoiceRet.ShipAddressBlock != null)
            {
                //Get value of Addr1
                if (InvoiceRet.ShipAddressBlock.Addr1 != null)
                {
                    string Addr148 = (string)InvoiceRet.ShipAddressBlock.Addr1.GetValue();
                    logger.Debug(string.Format("Addr148 {0}", Addr148));
                }
                //Get value of Addr2
                if (InvoiceRet.ShipAddressBlock.Addr2 != null)
                {
                    string Addr249 = (string)InvoiceRet.ShipAddressBlock.Addr2.GetValue();
                    logger.Debug(string.Format("Addr249 {0}", Addr249));
                }
                //Get value of Addr3
                if (InvoiceRet.ShipAddressBlock.Addr3 != null)
                {
                    string Addr350 = (string)InvoiceRet.ShipAddressBlock.Addr3.GetValue();
                    logger.Debug(string.Format("Addr350 {0}", Addr350));
                }
                //Get value of Addr4
                if (InvoiceRet.ShipAddressBlock.Addr4 != null)
                {
                    string Addr451 = (string)InvoiceRet.ShipAddressBlock.Addr4.GetValue();
                    logger.Debug(string.Format("Addr451 {0}", Addr451));
                }
                //Get value of Addr5
                if (InvoiceRet.ShipAddressBlock.Addr5 != null)
                {
                    string Addr552 = (string)InvoiceRet.ShipAddressBlock.Addr5.GetValue();
                    logger.Debug(string.Format("FullName18 {0}", Addr552));
                }
            }
            bool IsPending53, IsFinanceCharge54;
            string PONumber55, ListID56, FullName57;
            //Get value of IsPending
            if (InvoiceRet.IsPending != null)
            {
                IsPending53 = (bool)InvoiceRet.IsPending.GetValue();
                logger.Debug(string.Format("IsPending53 {0}", IsPending53));
            }
            //Get value of IsFinanceCharge
            if (InvoiceRet.IsFinanceCharge != null)
            {
                IsFinanceCharge54 = (bool)InvoiceRet.IsFinanceCharge.GetValue();
                logger.Debug(string.Format("IsFinanceCharge54 {0}", IsFinanceCharge54));
            }
            //Get value of PONumber
            if (InvoiceRet.PONumber != null)
            {
                PONumber55 = (string)InvoiceRet.PONumber.GetValue();
                logger.Debug(string.Format("PONumber55 {0}", PONumber55));
            }
            if (InvoiceRet.TermsRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.TermsRef.ListID != null)
                {
                    ListID56 = (string)InvoiceRet.TermsRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID56 {0}", ListID56));
                }
                //Get value of FullName
                if (InvoiceRet.TermsRef.FullName != null)
                {
                    FullName57 = (string)InvoiceRet.TermsRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName57 {0}", FullName57));
                }
            }
            DateTime DueDate58, ShipDate62;
            string ListID59, FullName60, FOB61, ListID63, FullName64;
            //Get value of DueDate
            if (InvoiceRet.DueDate != null)
            {
                DueDate58 = (DateTime)InvoiceRet.DueDate.GetValue();
                logger.Debug(string.Format("DueDate58 {0}", DueDate58));
            }
            if (InvoiceRet.SalesRepRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.SalesRepRef.ListID != null)
                {
                    ListID59 = (string)InvoiceRet.SalesRepRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID59 {0}", ListID59));
                }
                //Get value of FullName
                if (InvoiceRet.SalesRepRef.FullName != null)
                {
                    FullName60 = (string)InvoiceRet.SalesRepRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName60 {0}", FullName60));
                }
            }
            //Get value of FOB
            if (InvoiceRet.FOB != null)
            {
                FOB61 = (string)InvoiceRet.FOB.GetValue();
                logger.Debug(string.Format("FOB61 {0}", FOB61));
            }
            //Get value of ShipDate
            if (InvoiceRet.ShipDate != null)
            {
                ShipDate62 = (DateTime)InvoiceRet.ShipDate.GetValue();
                logger.Debug(string.Format("ShipDate62 {0}", ShipDate62));
            }
            if (InvoiceRet.ShipMethodRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.ShipMethodRef.ListID != null)
                {
                    ListID63 = (string)InvoiceRet.ShipMethodRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID63 {0}", ListID63));
                }
                //Get value of FullName
                if (InvoiceRet.ShipMethodRef.FullName != null)
                {
                    FullName64 = (string)InvoiceRet.ShipMethodRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName64 {0}", FullName64));
                }
            }
            double Subtotal65, SalesTaxPercentage68, SalesTaxTotal69, AppliedAmount70, BalanceRemaining71;
            string ListID66, FullName67;
            //Get value of Subtotal
            if (InvoiceRet.Subtotal != null)
            {
                Subtotal65 = (double)InvoiceRet.Subtotal.GetValue();
                logger.Debug(string.Format("Subtotal65 {0}", Subtotal65));
            }
            if (InvoiceRet.ItemSalesTaxRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.ItemSalesTaxRef.ListID != null)
                {
                    ListID66 = (string)InvoiceRet.ItemSalesTaxRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID66 {0}", ListID66));
                }
                //Get value of FullName
                if (InvoiceRet.ItemSalesTaxRef.FullName != null)
                {
                    FullName67 = (string)InvoiceRet.ItemSalesTaxRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName67 {0}", FullName67));
                }
            }
            //Get value of SalesTaxPercentage
            if (InvoiceRet.SalesTaxPercentage != null)
            {
                SalesTaxPercentage68 = (double)InvoiceRet.SalesTaxPercentage.GetValue();
                logger.Debug(string.Format("SalesTaxPercentage68 {0}", SalesTaxPercentage68));
            }
            //Get value of SalesTaxTotal
            if (InvoiceRet.SalesTaxTotal != null)
            {
                SalesTaxTotal69 = (double)InvoiceRet.SalesTaxTotal.GetValue();
                logger.Debug(string.Format("SalesTaxTotal69 {0}", SalesTaxTotal69));
            }
            //Get value of AppliedAmount
            if (InvoiceRet.AppliedAmount != null)
            {
                AppliedAmount70 = (double)InvoiceRet.AppliedAmount.GetValue();
                logger.Debug(string.Format("AppliedAmount70 {0}", AppliedAmount70));
            }
            //Get value of BalanceRemaining
            if (InvoiceRet.BalanceRemaining != null)
            {
                BalanceRemaining71 = (double)InvoiceRet.BalanceRemaining.GetValue();
                logger.Debug(string.Format("BalanceRemaining71 {0}", BalanceRemaining71));
            }
            string ListID72, FullName73;
            if (InvoiceRet.CurrencyRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.CurrencyRef.ListID != null)
                {
                    ListID72 = (string)InvoiceRet.CurrencyRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID72 {0}", ListID72));
                }
                //Get value of FullName
                if (InvoiceRet.CurrencyRef.FullName != null)
                {
                    FullName73 = (string)InvoiceRet.CurrencyRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName73 {0}", FullName73));
                }
            }
            float ExchangeRate74;
            double BalanceRemainingInHomeCurrency75;
            //Get value of ExchangeRate
            if (InvoiceRet.ExchangeRate != null)
            {
                ExchangeRate74 = InvoiceRet.ExchangeRate.GetValue();
                logger.Debug(string.Format("ExchangeRate74 {0}", ExchangeRate74));
            }
            //Get value of BalanceRemainingInHomeCurrency
            if (InvoiceRet.BalanceRemainingInHomeCurrency != null)
            {
                BalanceRemainingInHomeCurrency75 = (double)InvoiceRet.BalanceRemainingInHomeCurrency.GetValue();
                logger.Debug(string.Format("BalanceRemainingInHomeCurrency75 {0}", BalanceRemainingInHomeCurrency75));
            }
            
            bool IsPaid77;
            string ListID78, FullName79, Memo76;
            //Get value of Memo
            if (InvoiceRet.Memo != null)
            {
                Memo76 = (string)InvoiceRet.Memo.GetValue();
                logger.Debug(string.Format("Memo76 {0}", Memo76));
            }
            //Get value of IsPaid
            if (InvoiceRet.IsPaid != null)
            {
                IsPaid77 = (bool)InvoiceRet.IsPaid.GetValue();
                logger.Debug(string.Format("IsPaid77 {0}", IsPaid77));
            }
            if (InvoiceRet.CustomerMsgRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.CustomerMsgRef.ListID != null)
                {
                    ListID78 = (string)InvoiceRet.CustomerMsgRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID78 {0}", ListID78));
                }
                //Get value of FullName
                if (InvoiceRet.CustomerMsgRef.FullName != null)
                {
                    FullName79 = (string)InvoiceRet.CustomerMsgRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName79 {0}", FullName79));
                }
            }
            //Get value of IsToBePrinted
            bool IsToBePrinted80, IsToBeEmailed81;
            if (InvoiceRet.IsToBePrinted != null)
            {
                IsToBePrinted80 = (bool)InvoiceRet.IsToBePrinted.GetValue();
                logger.Debug(string.Format("IsToBePrinted80 {0}", IsToBePrinted80));
            }
            //Get value of IsToBeEmailed
            if (InvoiceRet.IsToBeEmailed != null)
            {
                IsToBeEmailed81 = (bool)InvoiceRet.IsToBeEmailed.GetValue();
                logger.Debug(string.Format("IsToBeEmailed81 {0}", IsToBeEmailed81));
            }
            string ListID82, FullName83;
            if (InvoiceRet.CustomerSalesTaxCodeRef != null)
            {
                //Get value of ListID
                if (InvoiceRet.CustomerSalesTaxCodeRef.ListID != null)
                {
                    ListID82 = (string)InvoiceRet.CustomerSalesTaxCodeRef.ListID.GetValue();
                    logger.Debug(string.Format("ListID82 {0}", ListID82));
                }
                //Get value of FullName
                if (InvoiceRet.CustomerSalesTaxCodeRef.FullName != null)
                {
                    FullName83 = (string)InvoiceRet.CustomerSalesTaxCodeRef.FullName.GetValue();
                    logger.Debug(string.Format("FullName83 {0}", FullName83));
                }
            }
            double SuggestedDiscountAmount84;
            DateTime SuggestedDiscountDate85;
            //Get value of SuggestedDiscountAmount
            if (InvoiceRet.SuggestedDiscountAmount != null)
            {
                SuggestedDiscountAmount84 = (double)InvoiceRet.SuggestedDiscountAmount.GetValue();
                logger.Debug(string.Format("SuggestedDiscountAmount84 {0}", SuggestedDiscountAmount84));
            }
            //Get value of SuggestedDiscountDate
            if (InvoiceRet.SuggestedDiscountDate != null)
            {
                SuggestedDiscountDate85 = (DateTime)InvoiceRet.SuggestedDiscountDate.GetValue();
                logger.Debug(string.Format("SuggestedDiscountDate85 {0}", SuggestedDiscountDate85));
            }
            string Other86, ExternalGUID87;
            //Get value of Other
            if (InvoiceRet.Other != null)
            {
                Other86 = (string)InvoiceRet.Other.GetValue();
                logger.Debug(string.Format("Other86 {0}", Other86));
            }
            //Get value of ExternalGUID
            if (InvoiceRet.ExternalGUID != null)
            {
                ExternalGUID87 = (string)InvoiceRet.ExternalGUID.GetValue();
                logger.Debug(string.Format("ExternalGUID87 {0}", ExternalGUID87));
            }

            if (InvoiceRet.LinkedTxnList != null)
            {
                for (int i88 = 0; i88 < InvoiceRet.LinkedTxnList.Count; i88++)
                {
                    logger.Debug(string.Format("---- InvoiceRet.LinkedTxnList {0} ----", i88));

                    ILinkedTxn LinkedTxn = InvoiceRet.LinkedTxnList.GetAt(i88);
                    //Get value of TxnID
                    string TxnID89 = (string)LinkedTxn.TxnID.GetValue();
                    logger.Debug(string.Format("TxnLineID96 {0}", TxnID89));
                    //Get value of TxnType
                    ENTxnType TxnType90 = (ENTxnType)LinkedTxn.TxnType.GetValue();
                    //Get value of TxnDate
                    DateTime TxnDate91 = (DateTime)LinkedTxn.TxnDate.GetValue();
                    logger.Debug(string.Format("TxnLineID96 {0}", TxnDate91));
                    //Get value of RefNumber
                    if (LinkedTxn.RefNumber != null)
                    {
                        string RefNumber92 = (string)LinkedTxn.RefNumber.GetValue();
                        logger.Debug(string.Format("TxnLineID96 {0}", RefNumber92));
                    }
                    //Get value of LinkType
                    if (LinkedTxn.LinkType != null)
                    {
                        ENLinkType LinkType93 = (ENLinkType)LinkedTxn.LinkType.GetValue();
                    }
                    //Get value of Amount
                    double Amount94 = (double)LinkedTxn.Amount.GetValue();
                    logger.Debug(string.Format("TxnLineID96 {0}", Amount94));
                }
            }
            
            if (InvoiceRet.ORInvoiceLineRetList != null)
            {
                for (int i95 = 0; i95 < InvoiceRet.ORInvoiceLineRetList.Count; i95++)
                {
                    logger.Debug(string.Format("------------------ ITEM {0} --------------------------", i95));
                    IORInvoiceLineRet ORInvoiceLineRet = InvoiceRet.ORInvoiceLineRetList.GetAt(i95);
                    
                    if (ORInvoiceLineRet.InvoiceLineRet != null)
                    {
                        if (ORInvoiceLineRet.InvoiceLineRet != null)
                        {
                            //Get value of TxnLineID
                            string TxnLineID96 = (string)ORInvoiceLineRet.InvoiceLineRet.TxnLineID.GetValue();
                            logger.Debug(string.Format("TxnLineID96 {0}", TxnLineID96));
                            if (ORInvoiceLineRet.InvoiceLineRet.ItemRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineRet.ItemRef.ListID != null)
                                {
                                    string ListID97 = (string)ORInvoiceLineRet.InvoiceLineRet.ItemRef.ListID.GetValue();
                                    logger.Debug(string.Format("ListID97 {0}", ListID97));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineRet.ItemRef.FullName != null)
                                {
                                    string FullName98 = (string)ORInvoiceLineRet.InvoiceLineRet.ItemRef.FullName.GetValue();
                                    logger.Debug(string.Format("FullName98 {0}", FullName98));
                                }
                            }
                            //Get value of Desc
                            if (ORInvoiceLineRet.InvoiceLineRet.Desc != null)
                            {
                                string Desc99 = (string)ORInvoiceLineRet.InvoiceLineRet.Desc.GetValue();
                                logger.Debug(string.Format("Desc99 {0}", Desc99));
                            }
                            //Get value of Quantity
                            if (ORInvoiceLineRet.InvoiceLineRet.Quantity != null)
                            {
                                int Quantity100 = (int)ORInvoiceLineRet.InvoiceLineRet.Quantity.GetValue();
                                logger.Debug(string.Format("Quantity100 {0}", Quantity100));
                            }
                            //Get value of UnitOfMeasure
                            if (ORInvoiceLineRet.InvoiceLineRet.UnitOfMeasure != null)
                            {
                                string UnitOfMeasure101 = (string)ORInvoiceLineRet.InvoiceLineRet.UnitOfMeasure.GetValue();
                                logger.Debug(string.Format("UnitOfMeasure101 {0}", UnitOfMeasure101));
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.OverrideUOMSetRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineRet.OverrideUOMSetRef.ListID != null)
                                {
                                    string ListID102 = (string)ORInvoiceLineRet.InvoiceLineRet.OverrideUOMSetRef.ListID.GetValue();
                                    logger.Debug(string.Format("ListID102 {0}", ListID102));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineRet.OverrideUOMSetRef.FullName != null)
                                {
                                    string FullName103 = (string)ORInvoiceLineRet.InvoiceLineRet.OverrideUOMSetRef.FullName.GetValue();
                                    logger.Debug(string.Format("FullName103 {0}", FullName103));
                                }
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.ORRate != null)
                            {
                                if (ORInvoiceLineRet.InvoiceLineRet.ORRate.Rate != null)
                                {
                                    //Get value of Rate
                                    if (ORInvoiceLineRet.InvoiceLineRet.ORRate.Rate != null)
                                    {
                                        double Rate104 = (double)ORInvoiceLineRet.InvoiceLineRet.ORRate.Rate.GetValue();
                                        logger.Debug(string.Format("Rate104={0}", Rate104));
                                    }
                                }
                                if (ORInvoiceLineRet.InvoiceLineRet.ORRate.RatePercent != null)
                                {
                                    //Get value of RatePercent
                                    if (ORInvoiceLineRet.InvoiceLineRet.ORRate.RatePercent != null)
                                    {
                                        double RatePercent105 = (double)ORInvoiceLineRet.InvoiceLineRet.ORRate.RatePercent.GetValue();
                                        logger.Debug(string.Format("RatePercent105={0}", RatePercent105));
                                    }
                                }
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.ClassRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineRet.ClassRef.ListID != null)
                                {
                                    string ListID106 = (string)ORInvoiceLineRet.InvoiceLineRet.ClassRef.ListID.GetValue();
                                    logger.Debug(string.Format("ListID106={0}", ListID106));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName != null)
                                {
                                    string FullName107 = (string)ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName.GetValue();
                                    logger.Debug(string.Format("FullName107={0}", FullName107));
                                }
                            }
                            //Get value of Amount
                            if (ORInvoiceLineRet.InvoiceLineRet.Amount != null)
                            {
                                double Amount108 = (double)ORInvoiceLineRet.InvoiceLineRet.Amount.GetValue();
                                logger.Debug(string.Format("Amount108={0}", Amount108));
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.InventorySiteRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineRet.InventorySiteRef.ListID != null)
                                {
                                    string ListID109 = (string)ORInvoiceLineRet.InvoiceLineRet.InventorySiteRef.ListID.GetValue();
                                    logger.Debug(string.Format("ListID109={0}", ListID109));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineRet.InventorySiteRef.FullName != null)
                                {
                                    string FullName110 = (string)ORInvoiceLineRet.InvoiceLineRet.InventorySiteRef.FullName.GetValue();
                                    logger.Debug(string.Format("FullName110={0}", FullName110));
                                }
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.InventorySiteLocationRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineRet.InventorySiteLocationRef.ListID != null)
                                {
                                    string ListID111 = (string)ORInvoiceLineRet.InvoiceLineRet.InventorySiteLocationRef.ListID.GetValue();
                                    logger.Debug(string.Format("ListID111={0}", ListID111));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineRet.InventorySiteLocationRef.FullName != null)
                                {
                                    string FullName112 = (string)ORInvoiceLineRet.InvoiceLineRet.InventorySiteLocationRef.FullName.GetValue();
                                    logger.Debug(string.Format("FullName112={0}", FullName112));
                                }
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber != null)
                            {
                                if (ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber.SerialNumber != null)
                                {
                                    //Get value of SerialNumber
                                    if (ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber.SerialNumber != null)
                                    {
                                        string SerialNumber113 = (string)ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber.SerialNumber.GetValue();
                                        logger.Debug(string.Format("SerialNumber113={0}", SerialNumber113));
                                    }
                                }
                                if (ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber.LotNumber != null)
                                {
                                    //Get value of LotNumber
                                    if (ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber.LotNumber != null)
                                    {
                                        string LotNumber114 = (string)ORInvoiceLineRet.InvoiceLineRet.ORSerialLotNumber.LotNumber.GetValue();
                                        logger.Debug(string.Format("LotNumber114={0}", LotNumber114));
                                    }
                                }
                            }
                            //Get value of ServiceDate
                            if (ORInvoiceLineRet.InvoiceLineRet.ServiceDate != null)
                            {
                                DateTime ServiceDate115 = (DateTime)ORInvoiceLineRet.InvoiceLineRet.ServiceDate.GetValue();
                                logger.Debug(string.Format("ServiceDate115={0}", ServiceDate115));
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.SalesTaxCodeRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineRet.SalesTaxCodeRef.ListID != null)
                                {
                                    string ListID116 = (string)ORInvoiceLineRet.InvoiceLineRet.SalesTaxCodeRef.ListID.GetValue();
                                    logger.Debug(string.Format("ListID116={0}", ListID116));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineRet.SalesTaxCodeRef.FullName != null)
                                {
                                    string FullName117 = (string)ORInvoiceLineRet.InvoiceLineRet.SalesTaxCodeRef.FullName.GetValue();
                                    logger.Debug(string.Format("FullName117={0}", FullName117));
                                }
                            }
                            //Get value of Other1
                            if (ORInvoiceLineRet.InvoiceLineRet.Other1 != null)
                            {
                                string Other1118 = (string)ORInvoiceLineRet.InvoiceLineRet.Other1.GetValue();
                                logger.Debug(string.Format("Other1118={0}", Other1118));
                            }
                            //Get value of Other2
                            if (ORInvoiceLineRet.InvoiceLineRet.Other2 != null)
                            {
                                string Other2119 = (string)ORInvoiceLineRet.InvoiceLineRet.Other2.GetValue();
                                logger.Debug(string.Format("Other2119={0}", Other2119));
                            }
                            if (ORInvoiceLineRet.InvoiceLineRet.DataExtRetList != null)
                            {
                                logger.Debug("ORInvoiceLineRet.InvoiceLineRet.DataExtRetList");

                                for (int i120 = 0; i120 < ORInvoiceLineRet.InvoiceLineRet.DataExtRetList.Count; i120++)
                                {
                                    logger.Debug(string.Format("ORInvoiceLineRet.InvoiceLineRet.DataExtRetList ", i120));
                                    IDataExtRet DataExtRet = ORInvoiceLineRet.InvoiceLineRet.DataExtRetList.GetAt(i120);
                                    //Get value of OwnerID
                                    if (DataExtRet.OwnerID != null)
                                    {
                                        string OwnerID121 = (string)DataExtRet.OwnerID.GetValue();
                                        logger.Debug(string.Format("OwnerID121={0}", OwnerID121));
                                    }
                                    //Get value of DataExtName
                                    string DataExtName122 = (string)DataExtRet.DataExtName.GetValue();
                                    logger.Debug(string.Format("DataExtName122={0}", DataExtName122));
                                    //Get value of DataExtType
                                    ENDataExtType DataExtType123 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                                    //Get value of DataExtValue
                                    string DataExtValue124 = (string)DataExtRet.DataExtValue.GetValue();
                                    logger.Debug(string.Format("DataExtValue124={0}", DataExtValue124));
                                }
                            }
                        }
                    }
                    if (ORInvoiceLineRet.InvoiceLineGroupRet != null)
                    {
                        if (ORInvoiceLineRet.InvoiceLineGroupRet != null)
                        {
                            //Get value of TxnLineID
                            string TxnLineID125 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.TxnLineID.GetValue();
                            logger.Debug(string.Format("TxnLineID125 {0}", TxnLineID125));
                            //Get value of ListID
                            if (ORInvoiceLineRet.InvoiceLineGroupRet.ItemGroupRef.ListID != null)
                            {
                                string ListID126 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.ItemGroupRef.ListID.GetValue();
                                logger.Debug(string.Format("ListID102 {0}", ListID126));
                            }
                            //Get value of FullName
                            if (ORInvoiceLineRet.InvoiceLineGroupRet.ItemGroupRef.FullName != null)
                            {
                                string FullName127 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.ItemGroupRef.FullName.GetValue();
                                logger.Debug(string.Format("DataExtValue124={0}", FullName127));
                            }
                            //Get value of Desc
                            if (ORInvoiceLineRet.InvoiceLineGroupRet.Desc != null)
                            {
                                string Desc128 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.Desc.GetValue();
                                logger.Debug(string.Format("DataExtValue124={0}", Desc128));
                            }
                            //Get value of Quantity
                            if (ORInvoiceLineRet.InvoiceLineGroupRet.Quantity != null)
                            {
                                int Quantity129 = (int)ORInvoiceLineRet.InvoiceLineGroupRet.Quantity.GetValue();
                                logger.Debug(string.Format("DataExtValue124={0}", Quantity129));
                            }
                            //Get value of UnitOfMeasure
                            if (ORInvoiceLineRet.InvoiceLineGroupRet.UnitOfMeasure != null)
                            {
                                string UnitOfMeasure130 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.UnitOfMeasure.GetValue();
                                logger.Debug(string.Format("DataExtValue124={0}", UnitOfMeasure130));
                            }
                            if (ORInvoiceLineRet.InvoiceLineGroupRet.OverrideUOMSetRef != null)
                            {
                                //Get value of ListID
                                if (ORInvoiceLineRet.InvoiceLineGroupRet.OverrideUOMSetRef.ListID != null)
                                {
                                    string ListID131 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.OverrideUOMSetRef.ListID.GetValue();
                                    logger.Debug(string.Format("DataExtValue124={0}", ListID131));
                                }
                                //Get value of FullName
                                if (ORInvoiceLineRet.InvoiceLineGroupRet.OverrideUOMSetRef.FullName != null)
                                {
                                    string FullName132 = (string)ORInvoiceLineRet.InvoiceLineGroupRet.OverrideUOMSetRef.FullName.GetValue();
                                    logger.Debug(string.Format("DataExtValue124={0}", FullName132));
                                }
                            }
                            //Get value of IsPrintItemsInGroup
                            bool IsPrintItemsInGroup133 = (bool)ORInvoiceLineRet.InvoiceLineGroupRet.IsPrintItemsInGroup.GetValue();
                            logger.Debug(string.Format("DataExtValue124={0}", IsPrintItemsInGroup133));
                            //Get value of TotalAmount
                            double TotalAmount134 = (double)ORInvoiceLineRet.InvoiceLineGroupRet.TotalAmount.GetValue();
                            logger.Debug(string.Format("DataExtValue124={0}", TotalAmount134));

                            if (ORInvoiceLineRet.InvoiceLineGroupRet.InvoiceLineRetList != null)
                            {

                                for (int i135 = 0; i135 < ORInvoiceLineRet.InvoiceLineGroupRet.InvoiceLineRetList.Count; i135++)
                                {
                                    logger.Debug(string.Format("---- ORInvoiceLineRet.InvoiceLineGroupRet.InvoiceLineRetList {0} ----", i135));
                                    IInvoiceLineRet InvoiceLineRet = ORInvoiceLineRet.InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i135);
                                    //Get value of TxnLineID
                                    string TxnLineID136 = (string)InvoiceLineRet.TxnLineID.GetValue();
                                    logger.Debug(string.Format("TxnLineID136={0}", TxnLineID136));
                                    if (InvoiceLineRet.ItemRef != null)
                                    {
                                        //Get value of ListID
                                        if (InvoiceLineRet.ItemRef.ListID != null)
                                        {
                                            string ListID137 = (string)InvoiceLineRet.ItemRef.ListID.GetValue();
                                            logger.Debug(string.Format("ListID137={0}", ListID137));
                                        }
                                        //Get value of FullName
                                        if (InvoiceLineRet.ItemRef.FullName != null)
                                        {
                                            string FullName138 = (string)InvoiceLineRet.ItemRef.FullName.GetValue();
                                            logger.Debug(string.Format("FullName138={0}", FullName138));
                                        }
                                    }
                                    //Get value of Desc
                                    if (InvoiceLineRet.Desc != null)
                                    {
                                        string Desc139 = (string)InvoiceLineRet.Desc.GetValue();
                                        logger.Debug(string.Format("Desc139={0}", Desc139));
                                    }
                                    //Get value of Quantity
                                    if (InvoiceLineRet.Quantity != null)
                                    {
                                        int Quantity140 = (int)InvoiceLineRet.Quantity.GetValue();
                                        logger.Debug(string.Format("Quantity140={0}", Quantity140));
                                    }
                                    //Get value of UnitOfMeasure
                                    if (InvoiceLineRet.UnitOfMeasure != null)
                                    {
                                        string UnitOfMeasure141 = (string)InvoiceLineRet.UnitOfMeasure.GetValue();
                                        logger.Debug(string.Format("UnitOfMeasure141={0}", UnitOfMeasure141));
                                    }
                                    if (InvoiceLineRet.OverrideUOMSetRef != null)
                                    {
                                        //Get value of ListID
                                        if (InvoiceLineRet.OverrideUOMSetRef.ListID != null)
                                        {
                                            string ListID142 = (string)InvoiceLineRet.OverrideUOMSetRef.ListID.GetValue();
                                            logger.Debug(string.Format("ListID142={0}", ListID142));
                                        }
                                        //Get value of FullName
                                        if (InvoiceLineRet.OverrideUOMSetRef.FullName != null)
                                        {
                                            string FullName143 = (string)InvoiceLineRet.OverrideUOMSetRef.FullName.GetValue();
                                            logger.Debug(string.Format("FullName143={0}", FullName143));
                                        }
                                    }
                                    if (InvoiceLineRet.ORRate != null)
                                    {
                                        if (InvoiceLineRet.ORRate.Rate != null)
                                        {
                                            //Get value of Rate
                                            if (InvoiceLineRet.ORRate.Rate != null)
                                            {
                                                double Rate144 = (double)InvoiceLineRet.ORRate.Rate.GetValue();
                                                logger.Debug(string.Format("Rate144={0}", Rate144));
                                            }
                                        }
                                        if (InvoiceLineRet.ORRate.RatePercent != null)
                                        {
                                            //Get value of RatePercent
                                            if (InvoiceLineRet.ORRate.RatePercent != null)
                                            {
                                                double RatePercent145 = (double)InvoiceLineRet.ORRate.RatePercent.GetValue();
                                                logger.Debug(string.Format("RatePercent145={0}", RatePercent145));
                                            }
                                        }
                                    }
                                    if (InvoiceLineRet.ClassRef != null)
                                    {
                                        //Get value of ListID
                                        if (InvoiceLineRet.ClassRef.ListID != null)
                                        {
                                            string ListID146 = (string)InvoiceLineRet.ClassRef.ListID.GetValue();
                                            logger.Debug(string.Format("ListID146={0}", ListID146));
                                        }
                                        //Get value of FullName
                                        if (InvoiceLineRet.ClassRef.FullName != null)
                                        {
                                            string FullName147 = (string)InvoiceLineRet.ClassRef.FullName.GetValue();
                                            logger.Debug(string.Format("FullName147={0}", FullName147));
                                        }
                                    }
                                    //Get value of Amount
                                    if (InvoiceLineRet.Amount != null)
                                    {
                                        double Amount148 = (double)InvoiceLineRet.Amount.GetValue();
                                        logger.Debug(string.Format("Amount148={0}", Amount148));
                                    }
                                    if (InvoiceLineRet.InventorySiteRef != null)
                                    {
                                        //Get value of ListID
                                        if (InvoiceLineRet.InventorySiteRef.ListID != null)
                                        {
                                            string ListID149 = (string)InvoiceLineRet.InventorySiteRef.ListID.GetValue();
                                            logger.Debug(string.Format("ListID149={0}", ListID149));
                                        }
                                        //Get value of FullName
                                        if (InvoiceLineRet.InventorySiteRef.FullName != null)
                                        {
                                            string FullName150 = (string)InvoiceLineRet.InventorySiteRef.FullName.GetValue();
                                            logger.Debug(string.Format("FullName150={0}", FullName150));
                                        }
                                    }
                                    if (InvoiceLineRet.InventorySiteLocationRef != null)
                                    {
                                        //Get value of ListID
                                        if (InvoiceLineRet.InventorySiteLocationRef.ListID != null)
                                        {
                                            string ListID151 = (string)InvoiceLineRet.InventorySiteLocationRef.ListID.GetValue();
                                            logger.Debug(string.Format("ListID151={0}", ListID151));
                                        }
                                        //Get value of FullName
                                        if (InvoiceLineRet.InventorySiteLocationRef.FullName != null)
                                        {
                                            string FullName152 = (string)InvoiceLineRet.InventorySiteLocationRef.FullName.GetValue();
                                            logger.Debug(string.Format("FullName152={0}", FullName152));
                                        }
                                    }
                                    if (InvoiceLineRet.ORSerialLotNumber != null)
                                    {
                                        if (InvoiceLineRet.ORSerialLotNumber.SerialNumber != null)
                                        {
                                            //Get value of SerialNumber
                                            if (InvoiceLineRet.ORSerialLotNumber.SerialNumber != null)
                                            {
                                                string SerialNumber153 = (string)InvoiceLineRet.ORSerialLotNumber.SerialNumber.GetValue();
                                                logger.Debug(string.Format("SerialNumber153={0}", SerialNumber153));
                                            }
                                        }
                                        if (InvoiceLineRet.ORSerialLotNumber.LotNumber != null)
                                        {
                                            //Get value of LotNumber
                                            if (InvoiceLineRet.ORSerialLotNumber.LotNumber != null)
                                            {
                                                string LotNumber154 = (string)InvoiceLineRet.ORSerialLotNumber.LotNumber.GetValue();
                                                logger.Debug(string.Format("LotNumber154={0}", LotNumber154));
                                            }
                                        }
                                    }
                                    //Get value of ServiceDate
                                    if (InvoiceLineRet.ServiceDate != null)
                                    {
                                        DateTime ServiceDate155 = (DateTime)InvoiceLineRet.ServiceDate.GetValue();
                                        logger.Debug(string.Format("ServiceDate155={0}", ServiceDate155));
                                    }
                                    if (InvoiceLineRet.SalesTaxCodeRef != null)
                                    {
                                        //Get value of ListID
                                        if (InvoiceLineRet.SalesTaxCodeRef.ListID != null)
                                        {
                                            string ListID156 = (string)InvoiceLineRet.SalesTaxCodeRef.ListID.GetValue();
                                            logger.Debug(string.Format("ListID156={0}", ListID156));
                                        }
                                        //Get value of FullName
                                        if (InvoiceLineRet.SalesTaxCodeRef.FullName != null)
                                        {
                                            string FullName157 = (string)InvoiceLineRet.SalesTaxCodeRef.FullName.GetValue();
                                            logger.Debug(string.Format("FullName157={0}", FullName157));
                                        }
                                    }
                                    //Get value of Other1
                                    if (InvoiceLineRet.Other1 != null)
                                    {
                                        string Other1158 = (string)InvoiceLineRet.Other1.GetValue();
                                        logger.Debug(string.Format("Other1158={0}", Other1158));
                                    }
                                    //Get value of Other2
                                    if (InvoiceLineRet.Other2 != null)
                                    {
                                        string Other2159 = (string)InvoiceLineRet.Other2.GetValue();
                                        logger.Debug(string.Format("Other2159={0}", Other2159));
                                    }
                                    if (InvoiceLineRet.DataExtRetList != null)
                                    {
                                        for (int i160 = 0; i160 < InvoiceLineRet.DataExtRetList.Count; i160++)
                                        {
                                            logger.Debug(string.Format("--- InvoiceLineRet.DataExtRetList {0} ----", i160));
                                            IDataExtRet DataExtRet = InvoiceLineRet.DataExtRetList.GetAt(i160);
                                            //Get value of OwnerID
                                            if (DataExtRet.OwnerID != null)
                                            {
                                                string OwnerID161 = (string)DataExtRet.OwnerID.GetValue();
                                                logger.Debug(string.Format("OwnerID161={0}", OwnerID161));
                                            }
                                            //Get value of DataExtName
                                            string DataExtName162 = (string)DataExtRet.DataExtName.GetValue();
                                            logger.Debug(string.Format("DataExtName162={0}", DataExtName162));
                                            //Get value of DataExtType
                                            ENDataExtType DataExtType163 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                                            //Get value of DataExtValue
                                            string DataExtValue164 = (string)DataExtRet.DataExtValue.GetValue();
                                            logger.Debug(string.Format("DataExtValue164={0}", DataExtValue164));
                                        }
                                    }
                                }
                            }

                            if (ORInvoiceLineRet.InvoiceLineGroupRet.DataExtRetList != null)
                            {
                                for (int i165 = 0; i165 < ORInvoiceLineRet.InvoiceLineGroupRet.DataExtRetList.Count; i165++)
                                {
                                    logger.Debug(string.Format("----  ORInvoiceLineRet.InvoiceLineGroupRet.DataExtRetList {0} ----", i165));
                                    IDataExtRet DataExtRet = ORInvoiceLineRet.InvoiceLineGroupRet.DataExtRetList.GetAt(i165);
                                    //Get value of OwnerID
                                    if (DataExtRet.OwnerID != null)
                                    {
                                        string OwnerID166 = (string)DataExtRet.OwnerID.GetValue();
                                        logger.Debug(string.Format("OwnerID166={0}", OwnerID166));
                                    }
                                    //Get value of DataExtName
                                    string DataExtName167 = (string)DataExtRet.DataExtName.GetValue();
                                    logger.Debug(string.Format("DataExtName167={0}", DataExtName167));
                                    //Get value of DataExtType
                                    ENDataExtType DataExtType168 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                                    //Get value of DataExtValue
                                    string DataExtValue169 = (string)DataExtRet.DataExtValue.GetValue();
                                    logger.Debug(string.Format("DataExtValue169={0}", DataExtValue169));
                                }
                            }
                        }
                    }
                }
            }
            string OwnerID171, DataExtName172, DataExtValue174;
            if (InvoiceRet.DataExtRetList != null)
            {
                for (int i170 = 0; i170 < InvoiceRet.DataExtRetList.Count; i170++)
                {
                    logger.Debug(string.Format("---- InvoiceRet.DataExtRetList.Count {0} ----", i170));

                    IDataExtRet DataExtRet = InvoiceRet.DataExtRetList.GetAt(i170);
                    //Get value of OwnerID
                    if (DataExtRet.OwnerID != null)
                    {
                        OwnerID171 = (string)DataExtRet.OwnerID.GetValue();
                        logger.Debug(string.Format("OwnerID171={0}", OwnerID171));
                    }
                    //Get value of DataExtName
                    DataExtName172 = (string)DataExtRet.DataExtName.GetValue();
                    logger.Debug(string.Format("DataExtName172={0}", DataExtName172));
                    //Get value of DataExtType
                    ENDataExtType DataExtType173 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                    //Get value of DataExtValue
                    DataExtValue174 = (string)DataExtRet.DataExtValue.GetValue();
                    logger.Debug(string.Format("DataExtValue174={0}", DataExtValue174));
                }
            }
        }

    }
}
