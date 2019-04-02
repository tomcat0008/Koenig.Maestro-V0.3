﻿using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Koenig.Maestro.Operation.Cache.CacheRepository;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal sealed class OrderManager : ManagerBase
    {
        QuickBooksInvoiceManager qm;
        public OrderManager(TransactionContext context):base(context)
        {
            qm = new QuickBooksInvoiceManager(context);
        }

        public OrderMaster GetOrder(long id)
        {

            SpCall spCall = new SpCall("DAT.ORDER_MASTER_SELECT");
            spCall.SetBigInt("@ID", id);
            DataSet ds = db.ExecuteDataSet(spCall);
            if (ds.Tables[0].Rows.Count == 0)
                throw new Exception(string.Format("Order id {0} not found", id));

            OrderMaster result = ReadOrderMaster(ds.Tables[0].Rows[0]);
            result.OrderItems = ReadOrderItems(ds.Tables[1].AsEnumerable().ToList());
            if(ds.Tables[2].Rows.Count > 0)
                result.InvoiceLog = ReadInvoice(ds.Tables[2].Rows[0]);
            return result;
        }

        public void UpdateOrderStatus(List<long> ids, string status)
        {
            SpCall spCall = new SpCall("DAT.ORDER_MASTER_UPDATE_STATUS");
            spCall.SetStructured<long>("@ID_LIST", "COR.ID_LIST", "ID", ids);
            spCall.SetVarchar("@ORDER_STATUS", status);
            db.ExecuteNonQuery(spCall);
        }

        public List<OrderMaster> GetOrders(List<long> ids)
        {
            SpCall spCall = new SpCall("DAT.ORDER_MASTER_SELECT_MULTI_BY_ID");
            spCall.SetStructured<long>("@ID_LIST", "COR.ID_LIST", "ID", ids);

            DataSet ds = db.ExecuteDataSet(spCall);

            if (ds.Tables[0].Rows.Count == 0)
                throw new Exception("No orders could be found not found");

            List<OrderMaster> result = new List<OrderMaster>();

            ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    long orderId = row.Field<long>("ID");
                    OrderMaster orderMaster = ReadOrderMaster(row);
                    orderMaster.OrderItems = ReadOrderItems(ds.Tables[1].AsEnumerable().Where(r => r.Field<long>("ORDER_ID") == orderId).ToList());
                    orderMaster.InvoiceLog = ReadInvoice(ds.Tables[2].AsEnumerable().First(r => r.Field<long>("ORDER_ID") == orderId));
                    result.Add(orderMaster);
                });

            return result;
        }

        public long GetNewOrderId()
        {
            long result = 0;
            SpCall spCall = new SpCall("DAT.GET_ORDER_ID");
            result = db.ExecuteScalar<long>(spCall);
            return result;
        }


        public void InsertOrder(OrderMaster orderMaster)
        {
            orderMaster.CreateDate = DateTime.Now;

            SpCall spCall = new SpCall("DAT.ORDER_MASTER_INSERT");
            spCall.SetBigInt("@ID", orderMaster.Id);
            spCall.SetBigInt("@CUSTOMER_ID", orderMaster.Customer.Id);
            spCall.SetDateTime("@ORDER_DATE", orderMaster.OrderDate);
            spCall.SetDateTime("@DELIVERY_DATE", orderMaster.DeliveryDate);
            spCall.SetVarchar("@PAYMENT_TYPE", orderMaster.PaymentType);
            spCall.SetVarchar("@NOTES", orderMaster.Notes);
            spCall.SetDateTime("@CREATE_DATE", orderMaster.CreateDate);
            spCall.SetVarchar("@CREATE_USER", orderMaster.CreatedUser);
            orderMaster.UpdatedUser = orderMaster.CreatedUser;
            orderMaster.UpdateDate = orderMaster.CreateDate;
            db.ExecuteNonQuery(spCall);

            DataTable dt = PrepareOrderItemsTable(orderMaster.OrderItems);
            db.ExecuteBulkInsert(dt);

        }

        public void UpdateOrder(OrderMaster orderMaster, bool cleanItems)
        {
            orderMaster.UpdateDate = DateTime.Now;

            SpCall spCall = new SpCall("DAT.ORDER_MASTER_UPDATE");
            spCall.SetBigInt("@ID", orderMaster.Id);
            spCall.SetBigInt("@CUSTOMER_ID", orderMaster.Customer.Id);
            spCall.SetDateTime("@ORDER_DATE", orderMaster.OrderDate);
            spCall.SetDateTime("@DELIVERY_DATE", orderMaster.DeliveryDate);
            spCall.SetVarchar("@PAYMENT_TYPE", orderMaster.PaymentType);
            spCall.SetVarchar("@NOTES", orderMaster.Notes);
            spCall.SetVarchar("@ORDER_STATUS", orderMaster.OrderStatus);
            spCall.SetDateTime("@UPDATE_DATE", orderMaster.UpdateDate);
            spCall.SetVarchar("@UPDATE_USER", orderMaster.UpdatedUser);
            spCall.SetBit("@CLEAN_ORDERITEMS", cleanItems);
            db.ExecuteNonQuery(spCall);

            if(cleanItems)
            {
                DataTable dt = PrepareOrderItemsTable(orderMaster.OrderItems);
                db.ExecuteBulkInsert(dt);
            }
            else
            {
                ExecuteItemCrud(orderMaster);
            }

        }

        void ExecuteItemCrud(OrderMaster newOrderContent)
        {
            //get existing db state
            OrderMaster om = GetOrder(newOrderContent.Id);

            //existing items update or delete
            foreach (OrderItem item in om.OrderItems)
            {
                OrderItem newItem = newOrderContent.OrderItems.Find(itm => itm.QbProductMap.Id == item.QbProductMap.Id);
                if (newItem != null)
                {
                    if (item.Quantity != newItem.Quantity)
                    {
                        item.Quantity = newItem.Quantity;
                        item.Unit = newItem.Unit;
                        UpdateOrderItem(item);
                    }
                }
                else
                    DeleteOrderItem(item);

            }
            //new items can be bulk inserted
            List<long> existingProducts = om.OrderItems.Select(oi => oi.Product.Id).ToList();
            List<OrderItem> newItems = newOrderContent.OrderItems.Where(oi => !existingProducts.Contains(oi.Product.Id)).ToList();
            if (newItems.Count > 0)
            {
                if (newItems.Count == 1)
                    InsertOrderItem(newItems[0]);
                else
                {
                    DataTable dt = PrepareOrderItemsTable(newItems);
                    db.ExecuteBulkInsert(dt);
                }
            }
        }

        void DeleteOrderItem(OrderItem item)
        {
            item.UpdateDate = DateTime.Now;
            item.UpdatedUser = context.UserName;

            SpCall spCall = new SpCall("DAT.ORDER_ITEM_DELETE");
            spCall.SetBigInt("@ID", item.Id);
            spCall.SetDateTime("@UPDATE_DATE", item.UpdateDate);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(spCall);
        }

        void UpdateOrderItem(OrderItem item)
        {
            item.UpdateDate = DateTime.Now;
            item.UpdatedUser = context.UserName;

            SpCall spCall = new SpCall("DAT.ORDER_ITEM_UPDATE");
            spCall.SetBigInt("@ID", item.Id);
            spCall.SetBigInt("@ORDER_ID", item.OrderId);
            spCall.SetBigInt("@PRODUCT_ID", item.Product.Id);
            spCall.SetInt("@QUANTITY", item.Quantity);
            spCall.SetBigInt("@UNIT_ID", item.Unit.Id);
            spCall.SetBigInt("@QB_PRODUCT_MAP_ID", item.QbProductMap.Id);
            spCall.SetDecimal("@PRICE", item.Price);
            spCall.SetDateTime("@UPDATE_DATE", item.UpdateDate);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            spCall.SetDecimal("@AMOUNT", item.Amount);
            db.ExecuteNonQuery(spCall);
        }

        void InsertOrderItem(OrderItem item)
        {
            item.CreateDate = DateTime.Now;
            item.UpdateDate = item.CreateDate;
            item.CreatedUser = context.UserName;
            item.UpdatedUser = context.UserName;

            SpCall spCall = new SpCall("DAT.ORDER_ITEM_INSERT");
            spCall.SetBigInt("@ORDER_ID", item.OrderId);
            spCall.SetBigInt("@PRODUCT_ID", item.Product.Id);
            spCall.SetInt("@QUANTITY", item.Quantity);
            spCall.SetBigInt("@UNIT_ID", item.Unit.Id);
            spCall.SetBigInt("@QB_PRODUCT_MAP_ID", item.QbProductMap.Id);
            spCall.SetDecimal("@PRICE", item.Price);
            spCall.SetVarchar("@CREATE_USER", context.UserName);
            spCall.SetDateTime("@CREATE_DATE", item.UpdateDate);
            spCall.SetDecimal("@AMOUNT", item.Amount);
            db.ExecuteNonQuery(spCall);
        }

        public List<OrderMaster> List(DateTime begin, DateTime end, long customerId, string status, string dateField)
        {
            List<OrderMaster> result = new List<OrderMaster>();

            SpCall spCall = new SpCall("DAT.ORDER_MASTER_LIST");
            spCall.SetDateTime("@BEGIN_DATE", begin);
            spCall.SetDateTime("@END_DATE", end);
            spCall.SetBigInt("@CUSTOMER_ID", customerId);
            spCall.SetVarchar("@STATUS", status);
            spCall.SetVarchar("@DATE_FIELD", dateField);

            DataSet ds = db.ExecuteDataSet(spCall);

            if(ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(omRow =>
                {

                    OrderMaster om = ReadOrderMaster(omRow);
                    om.OrderItems = new List<OrderItem>();
                    ds.Tables[1].AsEnumerable().Where(itemRow =>
                        itemRow.Field<long>("ORDER_ID") == om.Id).ToList().ForEach(itemRow =>
                        om.OrderItems.Add(InitOrderItem(itemRow)));

                    ds.Tables[2].AsEnumerable().Where(logRow => logRow.Field<long>("ORDER_ID") == om.Id).ToList().ForEach(
                        logRow => om.InvoiceLog = qm.InitLog(logRow));
                    result.Add(om);

                });


            }


            return result;
        }

        DataTable PrepareOrderItemsTable(List<OrderItem> orderItems)
        {
            //buraya update/insert/delete karasilastirmasi konabilir.
            
            SpCall spCall = new SpCall("DAT.GET_ORDER_ITEM_SCHEMA");
            DataTable dt = db.ExecuteDataTable(spCall);
            DateTime now = DateTime.Now;
            orderItems.ForEach(o =>
            {
                DataRow dr = dt.NewRow();
                dr["ORDER_ID"] = o.OrderId;
                dr["PRODUCT_ID"] = o.Product.Id;
                dr["QUANTITY"] = o.Quantity;
                dr["UNIT_ID"] = o.Unit.Id;
                dr["QB_PRODUCT_MAP_ID"] = o.QbProductMap.Id;
                dr["PRICE"] = o.Price;
                dr["CREATE_DATE"] = now;
                dr["CREATE_USER"] = context.UserName;
                dr["UPDATE_DATE"] = now;
                dr["UPDATE_USER"] = context.UserName;
                dr["RECORD_STATUS"] = "A";
                dr["AMOUNT"] = o.Amount;
                dt.Rows.Add(dr);
            });
            dt.TableName = "DAT.ORDER_ITEM";
            return dt;
        }

        OrderMaster ReadOrderMaster(DataRow row)
        {
            OrderMaster result = new OrderMaster()
            {
                Id = row.Field<long>("ID"),
                CreateDate = row.Field<DateTime>("CREATE_DATE"),
                CreatedUser = row.Field<string>("CREATE_USER"),
                Customer = CustomerCache.Instance[row.Field<long>("CUSTOMER_ID")],
                DeliveryDate = row.Field<DateTime>("DELIVERY_DATE"),
                Notes = row.Field<string>("NOTES"),
                OrderDate = row.Field<DateTime>("ORDER_DATE"),
                OrderStatus = row.Field<string>("ORDER_STATUS"),
                //IntegrationStatus = row.Field<string>("INTEGRATION_STATUS"),
                PaymentType = row.Field<string>("PAYMENT_TYPE"),
                RecordStatus = row.Field<string>("RECORD_STATUS"),
                UpdateDate = row.Field<DateTime>("UPDATE_DATE"),
                UpdatedUser = row.Field<string>("UPDATE_USER")
            };

            
            return result;
        }

        OrderItem InitOrderItem(DataRow row)
        {
            OrderItem result = new OrderItem()
            {
                Id = row.Field<long>("ID"),
                CreateDate = row.Field<DateTime>("CREATE_DATE"),
                CreatedUser = row.Field<string>("CREATE_USER"),
                RecordStatus = row.Field<string>("RECORD_STATUS"),
                UpdateDate = row.Field<DateTime>("UPDATE_DATE"),
                UpdatedUser = row.Field<string>("UPDATE_USER"),
                OrderId = row.Field<long>("ORDER_ID"),
                Product = ProductCache.Instance[row.Field<long>("PRODUCT_ID")],
                Quantity = row.Field<int>("QUANTITY"),
                QbProductMap = QuickBooksProductMapCache.Instance[row.Field<long>("QB_PRODUCT_MAP_ID")],
                Price = QuickBooksProductMapCache.Instance[row.Field<long>("QB_PRODUCT_MAP_ID")].Price,
                Unit = UnitCache.Instance[row.Field<long>("UNIT_ID")],
                Amount = row.Field<decimal>("AMOUNT")
            };
            return result;
        }

        QuickBooksInvoiceLog ReadInvoice(DataRow row)
        {
            QuickBooksInvoiceLog result = null;
            if(row != null)
            {
                

                result = new QuickBooksInvoiceLog()
                {
                    BatchId = row.Field<long>("BATCH_ID"),
                    CreateDate = row.Field<DateTime>("CREATE_DATE"),
                    CreatedUser = row.Field<string>("CREATE_USER"),
                    Customer = CustomerCache.Instance[row.Field<long>("MAESTRO_CUSTOMER_ID")],
                    ErrorLog = row.Field<string>("ERROR_LOG"),
                    Id = row.Field<long>("ID"),
                    IntegrationDate = row.Field<DateTime>("INTEGRATION_DATE"),
                    IntegrationStatus = row.Field<string>("INTEGRATION_STATUS"),
                    OrderId = row.Field<long>("ORDER_ID"),
                    QuickBooksInvoiceId = row.Field<string>("QB_INVOICE_NO"),
                    QuickBooksTxnId = row.Field<string>("QB_TXN_ID"),
                    QuickBooksCustomerId = row.Field<string>("QB_CUSTOMER_ID"),
                    RecordStatus = row.Field<string>("RECORD_STATUS"),
                    UpdateDate = row.Field<DateTime>("UPDATE_DATE"),
                    UpdatedUser = row.Field<string>("UPDATE_USER")
                };
            }

            return result;
        }

        List<OrderItem> ReadOrderItems(List<DataRow> rows)
        {
            List<OrderItem> result = new List<OrderItem>();

            rows.ToList().ForEach(row =>
                {
                    result.Add(InitOrderItem(row));
                }
            );

            return result;
        }

        protected override DataTable PrepareTable(List<ITransactionEntity> itemList)
        {
            throw new NotImplementedException();
        }
    }
}
