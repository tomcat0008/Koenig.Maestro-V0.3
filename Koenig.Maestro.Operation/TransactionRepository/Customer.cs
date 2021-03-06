﻿using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Koenig.Maestro.Operation.QuickBooks;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using Koenig.Maestro.Operation.Utility;
using Newtonsoft.Json.Linq;

namespace Koenig.Maestro.Operation.TransactionRepository
{
    internal class Customer : TransactionBase
    {
        CustomerManager cm = null;
        public Customer(TransactionContext context) : base("CUSTOMER", context)
        {
            this.IsProgressing = false;
            this.MainEntitySample = new MaestroCustomer();
            cm = new CustomerManager(context);
        }

        protected override void DeserializeLog(byte[] logData)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(JToken token)
        {
            MaestroCustomer resultObj = new MaestroCustomer();

            JObject entityObj = JObject.Parse(token.ToString());

            resultObj.Id = entityObj["Id"].ToObject<long>();
            resultObj.Address = entityObj["Address"].ToString();
            resultObj.DefaultPaymentType = entityObj["DefaultPaymentType"].ToString();
            resultObj.Email = entityObj["Email"].ToString();
            resultObj.Name = entityObj["Name"].ToString();
            resultObj.Phone = entityObj["Phone"].ToString();
            resultObj.Title = entityObj["Title"].ToString();
            resultObj.QuickBooksId = entityObj["QuickBooksId"].ToString();
            resultObj.QuickBoosCompany = entityObj["QuickBoosCompany"].ToString();
            resultObj.CustomerGroup = entityObj["CustomerGroup"].ToString();
            resultObj.ReportGroup = entityObj["ReportGroup"].ToString();
            resultObj.InvoiceGroup = entityObj["InvoiceGroup"].ToString();
            MaestroRegion region = null;
            if(entityObj.ContainsKey("MaestroRegion"))
            {
                JObject regionObj = JObject.Parse(entityObj["MaestroRegion"].ToString());
                if(regionObj.ContainsKey("Id"))
                {
                    long regionId = 0;
                    long.TryParse(regionObj["Id"].ToString(), out regionId);
                    if (regionId > 0)
                        region = RegionCache.Instance[regionId];
                    if (region == null)
                        region = RegionCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);
                }
            }
            resultObj.Region = region;

            Context.TransactionObject = resultObj;
        }

        protected override void Get()
        {
            long id = ValidateEntityIdFromDataExtension();
            response.TransactionResult = CustomerCache.Instance[id];
        }

        protected override void Delete()
        {
            long id = ValidateEntityIdFromDataExtension();
            Context.TransactionObject = CustomerCache.Instance[id];
            cm.Delete(id);
        }

        protected override void List()
        {
            ExtractTransactionCriteria();
            string listCode = Context.Bag[MessageDataExtensionKeys.LIST_CODE].ToString();

            List<MaestroCustomer> result = CustomerCache.Instance.Values.Where(c => c.RecordStatus == "A").ToList();

            if (listCode.Equals(MessageDataExtensionValues.LIST_CODE_MERGE_INVOICE))
                result = result.Where(c => !c.InvoiceGroup.Equals(string.Empty)).ToList();

            response.TransactionResult = result.Cast<ITransactionEntity>().ToList();
        }

        protected override void New()
        {
            MaestroCustomer customer = (MaestroCustomer)request.TransactionEntityList[0];
            cm.InsertNewItem(customer);
            response.TransactionResult = customer;
            //Context.TransactionObject = customer;

        }

        protected override void Update()
        {
            MaestroCustomer customer = (MaestroCustomer)request.TransactionEntityList[0];
            cm.Update(customer);
            Context.TransactionObject = customer;
            
        }

        protected override void ImportQb()
        {
            /*
            ImportRequestType importRequestType = EnumUtils.GetEnum<ImportRequestType>(extendedData[MessageDataExtensionKeys.IMPORT_TYPE]);
            string qbId = string.Empty;
            if (importRequestType == ImportRequestType.Single)
                qbId = extendedData[MessageDataExtensionKeys.ID];
            */
            List<MaestroCustomer> qbCustomers = null;

            BackUp();

            using (qbAgent = new QuickBooksCustomerAgent(Context))
            {
                qbCustomers = qbAgent.Import().Cast<MaestroCustomer>().ToList();
            }

            if (qbCustomers.Count > 0)
            {
                List<MaestroCustomer> updatedCustomers = new List<MaestroCustomer>();
                foreach(MaestroCustomer customer in qbCustomers)
                {
                    MaestroCustomer existing = CustomerCache.Instance.GetByQbId(customer.QuickBooksId);
                    if (existing != null)
                    {
                        customer.Id = existing.Id;
                        customer.DefaultPaymentType = existing.DefaultPaymentType;
                        cm.Update(customer);
                        updatedCustomers.Add(existing);
                    }
                }
                List<ITransactionEntity> clist = qbCustomers.Where(q => !updatedCustomers.Select(u => u.QuickBooksId).Contains(q.QuickBooksId)).Cast<ITransactionEntity>().ToList();
                if(clist.Count > 0)
                    cm.BulkInsert(clist);
                responseMessage = string.Format("{0} customers have been imported/updated", qbCustomers.Count);
                response.TransactionResult = qbCustomers;
            }
            else
                responseMessage = "No customers have been imported";

                
        }

        protected override void ExportQb()
        {
            throw new NotImplementedException();
            /*
            using (qbAgent = new QuickBooksCustomerAgent(Context))
            {
                qbAgent.Export();
            }             
             */
        }

        public override void RefreshCache(ActionType at)
        {
            CustomerCache.Instance.Reload(true);
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
            string guid = Guid.NewGuid().ToString();
            SpCall call = new SpCall("BCK.BACK_UP_CUSTOMER");
            call.SetVarchar("@BATCH_ID", guid);
            call.SetDateTime("@BATCH_DATE", DateTime.Now);
            db.ExecuteNonQuery(call);
            response.TransactionResult = guid;
            response.ResultMessage = string.Format("Backup created with batch id `{0}`", guid) + Environment.NewLine;
        }


    }
}
