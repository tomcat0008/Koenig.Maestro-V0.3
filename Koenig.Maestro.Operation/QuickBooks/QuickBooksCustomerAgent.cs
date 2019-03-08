using Interop.QBFC13;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Framework.ManagerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.QuickBooks
{
    internal class QuickBooksCustomerAgent : QuickBooksAgent
    {

        public QuickBooksCustomerAgent(TransactionContext context) : base(context)
        {

        }

        public override void Export()
        {
            //StartSession();
            
            //FinishSession(true);
        }

        public override List<ITransactionEntity> Import()
        {
            StartSession();

            IMsgSetRequest request = GetLatestMsgSetRequest();
            ICustomerQuery query = request.AppendCustomerQueryRq();
            IResponse res = GetResponse(request);

            ICustomerRetList returnList = res.Detail as ICustomerRetList;
            
            List<MaestroCustomer> mlist = new List<MaestroCustomer>();

            for (int i = 0; i<returnList.Count; i++)
            {
                ICustomerRet qbc = returnList.GetAt(i);
                if (ReadBool(qbc.IsActive))
                    mlist.Add(GetMaestroCustomer(qbc));
            }
            
            return mlist.Cast<ITransactionEntity>().ToList();

        }

        MaestroCustomer GetMaestroCustomer(ICustomerRet qbc)
        {
            MaestroCustomer result = new MaestroCustomer();
            
            result.Address = ReadAddress(qbc.BillAddress);
            result.Email = ReadString(qbc.Email);
            result.Name = ReadString(qbc.Name);
            result.Phone = ReadString(qbc.Phone);
            result.Region = GetRegion(qbc.BillAddress);
            result.Title = ReadString(qbc.JobTitle);
            result.QuickBooksId = ReadQbId(qbc.ListID);
            result.QuickBoosCompany = ReadString(qbc.CompanyName);
            result.CreateDate = DateTime.Now;
            result.UpdateDate = DateTime.Now;
            result.UpdatedUser = context.UserName;
            result.CreatedUser = context.UserName;
            return result;
        }

        MaestroRegion GetRegion(IAddress addressData)
        {
            MaestroRegion region = null;

            if (addressData != null)
            {
                string pk = ReadString(addressData.PostalCode).Replace(" ", "");

                if (!string.IsNullOrWhiteSpace(pk))
                    region = RegionCache.Instance.GetByPostalCode(pk);
            }

            if (region == null)
                region = new RegionManager(context).GetUnknownItem();

            return region;


        }


        string ReadAddress(IAddress addressData)
        {
            string result = string.Empty;
            if (addressData != null)
            {
                string addressChunk = ReadString(addressData.Addr1);
                result = string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
                addressChunk = ReadString(addressData.Addr2);
                result += string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
                addressChunk = ReadString(addressData.Addr3);
                result += string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
                addressChunk = ReadString(addressData.Addr4);
                result += string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
                addressChunk = ReadString(addressData.City);
                result += string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
                addressChunk = ReadString(addressData.PostalCode);
                result += string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
                addressChunk = ReadString(addressData.State);
                result += string.IsNullOrWhiteSpace(result) ? addressChunk : " " + addressChunk;
            }
            return result;
        }



    }
}
