using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class CustomerManager : ManagerBase
    {
        public CustomerManager(TransactionContext context) : base(context)
        {
        }


        public void Delete(long id)
        {
            //MaestroCustomer td = (MaestroCustomer)request.TransactionEntityList[0];
            SpCall spCall = new SpCall("DAT.CUSTOMER_DELETE");
            spCall.SetBigInt("@ID", id);
            spCall.SetDateTime("@UPDATE_DATE", DateTime.Now);
            spCall.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(spCall);

        }

        public void Erase(long id)
        {
            SpCall spCall = new SpCall("DAT.CUSTOMER_ERASE");
            spCall.SetBigInt("@ID", id);
            db.ExecuteNonQuery(spCall);
        }

        public void Update(MaestroCustomer customer)
        {
            SpCall call = new SpCall("DAT.CUSTOMER_UPDATE");
            call.SetBigInt("@ID", customer.Id);
            call.SetVarchar("@CUSTOMER_NAME", customer.QuickBoosCompany);
            call.SetVarchar("@CUSTOMER_TITLE", customer.Title);
            call.SetVarchar("@CUSTOMER_ADDRESS", customer.Address);
            call.SetVarchar("@CUSTOMER_PHONE", customer.Phone);
            call.SetVarchar("@CUSTOMER_EMAIL", customer.Email);
            call.SetVarchar("@QB_CUSTOMER_ID", customer.QuickBooksId);
            call.SetVarchar("@QB_COMPANY", customer.Name);
            call.SetBigInt("@REGION_ID", customer.Region.Id);
            call.SetVarchar("@DEFAULT_PAYMENT_TYPE", customer.DefaultPaymentType);
            call.SetVarchar("@CUSTOMER_GROUP", customer.CustomerGroup);
            call.SetVarchar("@REPORT_GROUP", customer.ReportGroup);
            
            call.SetDateTime("@UPDATE_DATE", DateTime.Now);
            call.SetVarchar("@UPDATE_USER", context.UserName);
            db.ExecuteNonQuery(call);
        }

        protected override DataTable PrepareTable(List<ITransactionEntity> customers)
        {
            SpCall spCall = new SpCall("DAT.GET_CUSTOMER_SCHEMA");
            DataTable dt = db.ExecuteDataTable(spCall);
            customers.Cast<MaestroCustomer>().ToList().ForEach(cus =>
            {
                MaestroCustomer existing = CustomerCache.Instance.GetByQbId(cus.QuickBooksId);
                if (existing != null)
                    Delete(existing.Id);

                DataRow dr = dt.NewRow();
                dr["CUSTOMER_NAME"] = cus.Name;
                dr["CUSTOMER_TITLE"] = cus.Title;
                dr["CUSTOMER_ADDRESS"] = cus.Address;
                dr["CUSTOMER_PHONE"] = cus.Phone;
                dr["CUSTOMER_EMAIL"] = cus.Email;
                dr["QB_CUSTOMER_ID"] = cus.QuickBooksId;
                dr["QB_COMPANY"] = cus.QuickBoosCompany;
                dr["REGION_ID"] = cus.Region.Id;
                dr["DEFAULT_PAYMENT_TYPE"] = cus.DefaultPaymentType;
                dr["CREATE_DATE"] = cus.CreateDate;
                dr["CREATE_USER"] = cus.CreatedUser;
                dr["UPDATE_DATE"] = cus.UpdateDate;
                dr["UPDATE_USER"] = cus.UpdatedUser;
                dr["RECORD_STATUS"] = "A";
                dr["CUSTOMER_GROUP"] = cus.CustomerGroup;
                dr["REPORT_GROUP"] = cus.ReportGroup;
                
                dt.Rows.Add(dr);
            });
            dt.TableName = "DAT.CUSTOMER";
            return dt;
        }

        public MaestroCustomer GetUnknownItem()
        {
            MaestroCustomer unknowCustomer = CustomerCache.Instance.GetByName(MaestroApplication.Instance.UNKNOWN_ITEM_NAME);

            if (unknowCustomer == null)
            {
                unknowCustomer = new MaestroCustomer()
                {
                    Address = string.Empty,
                    DefaultPaymentType = string.Empty,
                    Email    = string.Empty,
                    Phone = string.Empty,
                    QuickBooksId     = string.Empty,
                    QuickBoosCompany     = string.Empty,
                    Region = new RegionManager(context).GetUnknownItem(),
                    Title = string.Empty,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreatedUser = "MAESTRO",
                    UpdatedUser = "MAESTRO",
                    RecordStatus = "A",
                    CustomerGroup = string.Empty,
                    ReportGroup = string.Empty

                };
                InsertNewItem(unknowCustomer);
                CustomerCache.Instance.Reload(true);
            }

            return unknowCustomer;
        }

        public void InsertNewItem(MaestroCustomer customer)
        {
            SpCall call = new SpCall("DAT.CUSTOMER_INSERT");
            call.SetVarchar("@CUSTOMER_NAME", customer.Name);
            call.SetVarchar("@CUSTOMER_TITLE", customer.Title);
            call.SetVarchar("@CUSTOMER_ADDRESS", customer.Address);
            call.SetVarchar("@CUSTOMER_PHONE", customer.Phone);
            call.SetVarchar("@CUSTOMER_EMAIL", customer.Email);
            call.SetVarchar("@QB_CUSTOMER_ID", customer.QuickBooksId);
            call.SetVarchar("@QB_COMPANY", customer.QuickBoosCompany);
            call.SetBigInt("@REGION_ID", customer.Region.Id);
            call.SetVarchar("@DEFAULT_PAYMENT_TYPE", customer.DefaultPaymentType);
            call.SetVarchar("@CUSTOMER_GROUP", customer.CustomerGroup);
            call.SetVarchar("@REPORT_GROUP", customer.ReportGroup);
            call.SetDateTime("@CREATE_DATE", DateTime.Now);
            call.SetVarchar("@CREATE_USER", context.UserName);
            customer.Id = db.ExecuteScalar<long>(call);
            
        }

    }
}
