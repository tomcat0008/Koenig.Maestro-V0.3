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
                {
                    mlist.Add(GetMaestroCustomer(qbc));
                    /*
                    if(qbc.ShipToAddressList != null)
                    {
                        Console.WriteLine("count:"+qbc.ShipToAddressList.Count);
                    }*/
                }
                    //WalkCustomerRet(qbc);
                    //
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
            result.CustomerGroup = qbc.CustomerTypeRef == null ? string.Empty : ReadString(qbc.CustomerTypeRef.FullName);
            if (qbc.ShipToAddressList != null)
                for (int i = 0; i < qbc.ShipToAddressList.Count; i++)
                {
                    StringBuilder bld = new StringBuilder();
                    IShipToAddress shipAddress = qbc.ShipToAddressList.GetAt(i);
                    bld.AppendLine(shipAddress.Name.GetValue());
                    bld.AppendLine(shipAddress.Addr1.GetValue());
                    bld.AppendLine(shipAddress.Addr2.GetValue());
                    bld.AppendLine(shipAddress.Addr3.GetValue());
                    bld.AppendLine(shipAddress.Addr4.GetValue());
                    bld.AppendLine(shipAddress.Addr5.GetValue());
                    bld.AppendLine(shipAddress.City.GetValue());
                    bld.AppendLine(shipAddress.PostalCode.GetValue());
                    bld.AppendLine(shipAddress.State.GetValue());
                    Console.WriteLine(bld.ToString());
                }

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

        void WalkCustomerRet(ICustomerRet CustomerRet)
        {
            if (CustomerRet == null) return;
            if (CustomerRet.ShipToAddressList == null)
                return;
            //Go through all the elements of ICustomerRetList
            //Get value of ListID
            string ListID5 = (string)CustomerRet.ListID.GetValue();
            //Get value of TimeCreated
            DateTime TimeCreated6 = (DateTime)CustomerRet.TimeCreated.GetValue();
            //Get value of TimeModified
            DateTime TimeModified7 = (DateTime)CustomerRet.TimeModified.GetValue();
            //Get value of EditSequence
            string EditSequence8 = (string)CustomerRet.EditSequence.GetValue();
            //Get value of Name
            string Name9 = (string)CustomerRet.Name.GetValue();
            //Get value of FullName
            string FullName10 = (string)CustomerRet.FullName.GetValue();
            //Get value of IsActive

            if (CustomerRet.IsActive != null)
            {
                bool IsActive11 = (bool)CustomerRet.IsActive.GetValue();
            }
            if (CustomerRet.ClassRef != null)
            {
                //Get value of ListID
                if (CustomerRet.ClassRef.ListID != null)
                {
                    string ListID12 = (string)CustomerRet.ClassRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.ClassRef.FullName != null)
                {
                    string FullName13 = (string)CustomerRet.ClassRef.FullName.GetValue();
                }
            }
            if (CustomerRet.ParentRef != null)
            {
                //Get value of ListID
                if (CustomerRet.ParentRef.ListID != null)
                {
                    string ListID14 = (string)CustomerRet.ParentRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.ParentRef.FullName != null)
                {
                    string FullName15 = (string)CustomerRet.ParentRef.FullName.GetValue();
                }
            }
            //Get value of Sublevel
            int Sublevel16 = (int)CustomerRet.Sublevel.GetValue();
            //Get value of CompanyName
            if (CustomerRet.CompanyName != null)
            {
                string CompanyName17 = (string)CustomerRet.CompanyName.GetValue();
            }
            //Get value of Salutation
            if (CustomerRet.Salutation != null)
            {
                string Salutation18 = (string)CustomerRet.Salutation.GetValue();
            }
            //Get value of FirstName
            if (CustomerRet.FirstName != null)
            {
                string FirstName19 = (string)CustomerRet.FirstName.GetValue();
            }
            //Get value of MiddleName
            if (CustomerRet.MiddleName != null)
            {
                string MiddleName20 = (string)CustomerRet.MiddleName.GetValue();
            }
            //Get value of LastName
            if (CustomerRet.LastName != null)
            {
                string LastName21 = (string)CustomerRet.LastName.GetValue();
            }
            //Get value of JobTitle
            if (CustomerRet.JobTitle != null)
            {
                string JobTitle22 = (string)CustomerRet.JobTitle.GetValue();
            }
            if (CustomerRet.BillAddress != null)
            {
                //Get value of Addr1
                if (CustomerRet.BillAddress.Addr1 != null)
                {
                    string Addr123 = (string)CustomerRet.BillAddress.Addr1.GetValue();
                }
                //Get value of Addr2
                if (CustomerRet.BillAddress.Addr2 != null)
                {
                    string Addr224 = (string)CustomerRet.BillAddress.Addr2.GetValue();
                }
                //Get value of Addr3
                if (CustomerRet.BillAddress.Addr3 != null)
                {
                    string Addr325 = (string)CustomerRet.BillAddress.Addr3.GetValue();
                }
                //Get value of Addr4
                if (CustomerRet.BillAddress.Addr4 != null)
                {
                    string Addr426 = (string)CustomerRet.BillAddress.Addr4.GetValue();
                }
                //Get value of Addr5
                if (CustomerRet.BillAddress.Addr5 != null)
                {
                    string Addr527 = (string)CustomerRet.BillAddress.Addr5.GetValue();
                }
                //Get value of City
                if (CustomerRet.BillAddress.City != null)
                {
                    string City28 = (string)CustomerRet.BillAddress.City.GetValue();
                }
                //Get value of State
                if (CustomerRet.BillAddress.State != null)
                {
                    string State29 = (string)CustomerRet.BillAddress.State.GetValue();
                }
                //Get value of PostalCode
                if (CustomerRet.BillAddress.PostalCode != null)
                {
                    string PostalCode30 = (string)CustomerRet.BillAddress.PostalCode.GetValue();
                }
                //Get value of Country
                if (CustomerRet.BillAddress.Country != null)
                {
                    string Country31 = (string)CustomerRet.BillAddress.Country.GetValue();
                }
                //Get value of Note
                if (CustomerRet.BillAddress.Note != null)
                {
                    string Note32 = (string)CustomerRet.BillAddress.Note.GetValue();
                }
            }
            if (CustomerRet.BillAddressBlock != null)
            {
                //Get value of Addr1
                if (CustomerRet.BillAddressBlock.Addr1 != null)
                {
                    string Addr133 = (string)CustomerRet.BillAddressBlock.Addr1.GetValue();
                }
                //Get value of Addr2
                if (CustomerRet.BillAddressBlock.Addr2 != null)
                {
                    string Addr234 = (string)CustomerRet.BillAddressBlock.Addr2.GetValue();
                }
                //Get value of Addr3
                if (CustomerRet.BillAddressBlock.Addr3 != null)
                {
                    string Addr335 = (string)CustomerRet.BillAddressBlock.Addr3.GetValue();
                }
                //Get value of Addr4
                if (CustomerRet.BillAddressBlock.Addr4 != null)
                {
                    string Addr436 = (string)CustomerRet.BillAddressBlock.Addr4.GetValue();
                }
                //Get value of Addr5
                if (CustomerRet.BillAddressBlock.Addr5 != null)
                {
                    string Addr537 = (string)CustomerRet.BillAddressBlock.Addr5.GetValue();
                }
            }
            if (CustomerRet.ShipAddress != null)
            {
                //Get value of Addr1
                if (CustomerRet.ShipAddress.Addr1 != null)
                {
                    string Addr138 = (string)CustomerRet.ShipAddress.Addr1.GetValue();
                }
                //Get value of Addr2
                if (CustomerRet.ShipAddress.Addr2 != null)
                {
                    string Addr239 = (string)CustomerRet.ShipAddress.Addr2.GetValue();
                }
                //Get value of Addr3
                if (CustomerRet.ShipAddress.Addr3 != null)
                {
                    string Addr340 = (string)CustomerRet.ShipAddress.Addr3.GetValue();
                }
                //Get value of Addr4
                if (CustomerRet.ShipAddress.Addr4 != null)
                {
                    string Addr441 = (string)CustomerRet.ShipAddress.Addr4.GetValue();
                }
                //Get value of Addr5
                if (CustomerRet.ShipAddress.Addr5 != null)
                {
                    string Addr542 = (string)CustomerRet.ShipAddress.Addr5.GetValue();
                }
                //Get value of City
                if (CustomerRet.ShipAddress.City != null)
                {
                    string City43 = (string)CustomerRet.ShipAddress.City.GetValue();
                }
                //Get value of State
                if (CustomerRet.ShipAddress.State != null)
                {
                    string State44 = (string)CustomerRet.ShipAddress.State.GetValue();
                }
                //Get value of PostalCode
                if (CustomerRet.ShipAddress.PostalCode != null)
                {
                    string PostalCode45 = (string)CustomerRet.ShipAddress.PostalCode.GetValue();
                }
                //Get value of Country
                if (CustomerRet.ShipAddress.Country != null)
                {
                    string Country46 = (string)CustomerRet.ShipAddress.Country.GetValue();
                }
                //Get value of Note
                if (CustomerRet.ShipAddress.Note != null)
                {
                    string Note47 = (string)CustomerRet.ShipAddress.Note.GetValue();
                }
            }
            
            if (CustomerRet.ShipAddressBlock != null)
            {
                //Get value of Addr1
                if (CustomerRet.ShipAddressBlock.Addr1 != null)
                {
                    string Addr148 = (string)CustomerRet.ShipAddressBlock.Addr1.GetValue();
                }
                //Get value of Addr2
                if (CustomerRet.ShipAddressBlock.Addr2 != null)
                {
                    string Addr249 = (string)CustomerRet.ShipAddressBlock.Addr2.GetValue();
                }
                //Get value of Addr3
                if (CustomerRet.ShipAddressBlock.Addr3 != null)
                {
                    string Addr350 = (string)CustomerRet.ShipAddressBlock.Addr3.GetValue();
                }
                //Get value of Addr4
                if (CustomerRet.ShipAddressBlock.Addr4 != null)
                {
                    string Addr451 = (string)CustomerRet.ShipAddressBlock.Addr4.GetValue();
                }
                //Get value of Addr5
                if (CustomerRet.ShipAddressBlock.Addr5 != null)
                {
                    string Addr552 = (string)CustomerRet.ShipAddressBlock.Addr5.GetValue();
                }
            }
            if (CustomerRet.ShipToAddressList != null)
            {
                for (int i53 = 0; i53 < CustomerRet.ShipToAddressList.Count; i53++)
                {
                    IShipToAddress ShipToAddress = CustomerRet.ShipToAddressList.GetAt(i53);
                    //Get value of Name
                    string Name54 = (string)ShipToAddress.Name.GetValue();
                    //Get value of Addr1
                    if (ShipToAddress.Addr1 != null)
                    {
                        string Addr155 = (string)ShipToAddress.Addr1.GetValue();
                    }
                    //Get value of Addr2
                    if (ShipToAddress.Addr2 != null)
                    {
                        string Addr256 = (string)ShipToAddress.Addr2.GetValue();
                    }
                    //Get value of Addr3
                    if (ShipToAddress.Addr3 != null)
                    {
                        string Addr357 = (string)ShipToAddress.Addr3.GetValue();
                    }
                    //Get value of Addr4
                    if (ShipToAddress.Addr4 != null)
                    {
                        string Addr458 = (string)ShipToAddress.Addr4.GetValue();
                    }
                    //Get value of Addr5
                    if (ShipToAddress.Addr5 != null)
                    {
                        string Addr559 = (string)ShipToAddress.Addr5.GetValue();
                    }
                    //Get value of City
                    if (ShipToAddress.City != null)
                    {
                        string City60 = (string)ShipToAddress.City.GetValue();
                    }
                    //Get value of State
                    if (ShipToAddress.State != null)
                    {
                        string State61 = (string)ShipToAddress.State.GetValue();
                    }
                    //Get value of PostalCode
                    if (ShipToAddress.PostalCode != null)
                    {
                        string PostalCode62 = (string)ShipToAddress.PostalCode.GetValue();
                    }
                    //Get value of Country
                    if (ShipToAddress.Country != null)
                    {
                        string Country63 = (string)ShipToAddress.Country.GetValue();
                    }
                    //Get value of Note
                    if (ShipToAddress.Note != null)
                    {
                        string Note64 = (string)ShipToAddress.Note.GetValue();
                    }
                    //Get value of DefaultShipTo
                    if (ShipToAddress.DefaultShipTo != null)
                    {
                        bool DefaultShipTo65 = (bool)ShipToAddress.DefaultShipTo.GetValue();
                    }
                }
            }
            //Get value of Phone
            if (CustomerRet.Phone != null)
            {
                string Phone66 = (string)CustomerRet.Phone.GetValue();
            }
            //Get value of AltPhone
            if (CustomerRet.AltPhone != null)
            {
                string AltPhone67 = (string)CustomerRet.AltPhone.GetValue();
            }
            //Get value of Fax
            if (CustomerRet.Fax != null)
            {
                string Fax68 = (string)CustomerRet.Fax.GetValue();
            }
            //Get value of Email
            if (CustomerRet.Email != null)
            {
                string Email69 = (string)CustomerRet.Email.GetValue();
            }
            //Get value of Cc
            if (CustomerRet.Cc != null)
            {
                string Cc70 = (string)CustomerRet.Cc.GetValue();
            }
            //Get value of Contact
            if (CustomerRet.Contact != null)
            {
                string Contact71 = (string)CustomerRet.Contact.GetValue();
            }
            //Get value of AltContact
            if (CustomerRet.AltContact != null)
            {
                string AltContact72 = (string)CustomerRet.AltContact.GetValue();
            }

            if (CustomerRet.ContactsRetList != null)
            {
                for (int i76 = 0; i76 < CustomerRet.ContactsRetList.Count; i76++)
                {
                    IContactsRet ContactsRet = CustomerRet.ContactsRetList.GetAt(i76);
                    //Get value of ListID
                    string ListID77 = (string)ContactsRet.ListID.GetValue();
                    //Get value of TimeCreated
                    DateTime TimeCreated78 = (DateTime)ContactsRet.TimeCreated.GetValue();
                    //Get value of TimeModified
                    DateTime TimeModified79 = (DateTime)ContactsRet.TimeModified.GetValue();
                    //Get value of EditSequence
                    string EditSequence80 = (string)ContactsRet.EditSequence.GetValue();
                    //Get value of Contact
                    if (ContactsRet.Contact != null)
                    {
                        string Contact81 = (string)ContactsRet.Contact.GetValue();
                    }
                    //Get value of Salutation
                    if (ContactsRet.Salutation != null)
                    {
                        string Salutation82 = (string)ContactsRet.Salutation.GetValue();
                    }
                    //Get value of FirstName
                    string FirstName83 = (string)ContactsRet.FirstName.GetValue();
                    //Get value of MiddleName
                    if (ContactsRet.MiddleName != null)
                    {
                        string MiddleName84 = (string)ContactsRet.MiddleName.GetValue();
                    }
                    //Get value of LastName
                    if (ContactsRet.LastName != null)
                    {
                        string LastName85 = (string)ContactsRet.LastName.GetValue();
                    }
                    //Get value of JobTitle
                    if (ContactsRet.JobTitle != null)
                    {
                        string JobTitle86 = (string)ContactsRet.JobTitle.GetValue();
                    }
                    if (ContactsRet.AdditionalContactRefList != null)
                    {
                        for (int i87 = 0; i87 < ContactsRet.AdditionalContactRefList.Count; i87++)
                        {
                            IQBBaseRef QBBaseRef = ContactsRet.AdditionalContactRefList.GetAt(i87);
                            //Get value of ContactName
                            string ContactName88 = (string)QBBaseRef.FullName.GetValue();
                            //Get value of ContactValue
                            string ContactValue89 = (string)QBBaseRef.ListID.GetValue();
                        }
                    }
                }
            }
            if (CustomerRet.CustomerTypeRef != null)
            {
                //Get value of ListID
                if (CustomerRet.CustomerTypeRef.ListID != null)
                {
                    string ListID90 = (string)CustomerRet.CustomerTypeRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.CustomerTypeRef.FullName != null)
                {
                    string FullName91 = (string)CustomerRet.CustomerTypeRef.FullName.GetValue();
                }
            }
            if (CustomerRet.TermsRef != null)
            {
                //Get value of ListID
                if (CustomerRet.TermsRef.ListID != null)
                {
                    string ListID92 = (string)CustomerRet.TermsRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.TermsRef.FullName != null)
                {
                    string FullName93 = (string)CustomerRet.TermsRef.FullName.GetValue();
                }
            }
            if (CustomerRet.SalesRepRef != null)
            {
                //Get value of ListID
                if (CustomerRet.SalesRepRef.ListID != null)
                {
                    string ListID94 = (string)CustomerRet.SalesRepRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.SalesRepRef.FullName != null)
                {
                    string FullName95 = (string)CustomerRet.SalesRepRef.FullName.GetValue();
                }
            }
            //Get value of Balance
            if (CustomerRet.Balance != null)
            {
                double Balance96 = (double)CustomerRet.Balance.GetValue();
            }
            //Get value of TotalBalance
            if (CustomerRet.TotalBalance != null)
            {
                double TotalBalance97 = (double)CustomerRet.TotalBalance.GetValue();
            }
            if (CustomerRet.SalesTaxCodeRef != null)
            {
                //Get value of ListID
                if (CustomerRet.SalesTaxCodeRef.ListID != null)
                {
                    string ListID98 = (string)CustomerRet.SalesTaxCodeRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.SalesTaxCodeRef.FullName != null)
                {
                    string FullName99 = (string)CustomerRet.SalesTaxCodeRef.FullName.GetValue();
                }
            }
            if (CustomerRet.ItemSalesTaxRef != null)
            {
                //Get value of ListID
                if (CustomerRet.ItemSalesTaxRef.ListID != null)
                {
                    string ListID100 = (string)CustomerRet.ItemSalesTaxRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.ItemSalesTaxRef.FullName != null)
                {
                    string FullName101 = (string)CustomerRet.ItemSalesTaxRef.FullName.GetValue();
                }
            }
            //Get value of ResaleNumber
            if (CustomerRet.ResaleNumber != null)
            {
                string ResaleNumber102 = (string)CustomerRet.ResaleNumber.GetValue();
            }
            //Get value of AccountNumber
            if (CustomerRet.AccountNumber != null)
            {
                string AccountNumber103 = (string)CustomerRet.AccountNumber.GetValue();
            }
            //Get value of CreditLimit
            if (CustomerRet.CreditLimit != null)
            {
                double CreditLimit104 = (double)CustomerRet.CreditLimit.GetValue();
            }
            if (CustomerRet.PreferredPaymentMethodRef != null)
            {
                //Get value of ListID
                if (CustomerRet.PreferredPaymentMethodRef.ListID != null)
                {
                    string ListID105 = (string)CustomerRet.PreferredPaymentMethodRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.PreferredPaymentMethodRef.FullName != null)
                {
                    string FullName106 = (string)CustomerRet.PreferredPaymentMethodRef.FullName.GetValue();
                }
            }
            if (CustomerRet.CreditCardInfo != null)
            {
                //Get value of CreditCardNumber
                if (CustomerRet.CreditCardInfo.CreditCardNumber != null)
                {
                    string CreditCardNumber107 = (string)CustomerRet.CreditCardInfo.CreditCardNumber.GetValue();
                }
                //Get value of ExpirationMonth
                if (CustomerRet.CreditCardInfo.ExpirationMonth != null)
                {
                    int ExpirationMonth108 = (int)CustomerRet.CreditCardInfo.ExpirationMonth.GetValue();
                }
                //Get value of ExpirationYear
                if (CustomerRet.CreditCardInfo.ExpirationYear != null)
                {
                    int ExpirationYear109 = (int)CustomerRet.CreditCardInfo.ExpirationYear.GetValue();
                }
                //Get value of NameOnCard
                if (CustomerRet.CreditCardInfo.NameOnCard != null)
                {
                    string NameOnCard110 = (string)CustomerRet.CreditCardInfo.NameOnCard.GetValue();
                }
                //Get value of CreditCardAddress
                if (CustomerRet.CreditCardInfo.CreditCardAddress != null)
                {
                    string CreditCardAddress111 = (string)CustomerRet.CreditCardInfo.CreditCardAddress.GetValue();
                }
                //Get value of CreditCardPostalCode
                if (CustomerRet.CreditCardInfo.CreditCardPostalCode != null)
                {
                    string CreditCardPostalCode112 = (string)CustomerRet.CreditCardInfo.CreditCardPostalCode.GetValue();
                }
            }
            //Get value of JobStatus
            if (CustomerRet.JobStatus != null)
            {
                ENJobStatus JobStatus113 = (ENJobStatus)CustomerRet.JobStatus.GetValue();
            }
            //Get value of JobStartDate
            if (CustomerRet.JobStartDate != null)
            {
                DateTime JobStartDate114 = (DateTime)CustomerRet.JobStartDate.GetValue();
            }
            //Get value of JobProjectedEndDate
            if (CustomerRet.JobProjectedEndDate != null)
            {
                DateTime JobProjectedEndDate115 = (DateTime)CustomerRet.JobProjectedEndDate.GetValue();
            }
            //Get value of JobEndDate
            if (CustomerRet.JobEndDate != null)
            {
                DateTime JobEndDate116 = (DateTime)CustomerRet.JobEndDate.GetValue();
            }
            //Get value of JobDesc
            if (CustomerRet.JobDesc != null)
            {
                string JobDesc117 = (string)CustomerRet.JobDesc.GetValue();
            }
            if (CustomerRet.JobTypeRef != null)
            {
                //Get value of ListID
                if (CustomerRet.JobTypeRef.ListID != null)
                {
                    string ListID118 = (string)CustomerRet.JobTypeRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.JobTypeRef.FullName != null)
                {
                    string FullName119 = (string)CustomerRet.JobTypeRef.FullName.GetValue();
                }
            }
            //Get value of Notes
            if (CustomerRet.Notes != null)
            {
                string Notes120 = (string)CustomerRet.Notes.GetValue();
            }
            if (CustomerRet.AdditionalNotesRetList != null)
            {
                for (int i121 = 0; i121 < CustomerRet.AdditionalNotesRetList.Count; i121++)
                {
                    IAdditionalNotesRet AdditionalNotesRet = CustomerRet.AdditionalNotesRetList.GetAt(i121);
                    //Get value of NoteID
                    int NoteID122 = (int)AdditionalNotesRet.NoteID.GetValue();
                    //Get value of Date
                    DateTime Date123 = (DateTime)AdditionalNotesRet.Date.GetValue();
                    //Get value of Note
                    string Note124 = (string)AdditionalNotesRet.Note.GetValue();
                }
            }
            //Get value of PreferredDeliveryMethod
            if (CustomerRet.PreferredDeliveryMethod != null)
            {
                ENPreferredDeliveryMethod PreferredDeliveryMethod125 = (ENPreferredDeliveryMethod)CustomerRet.PreferredDeliveryMethod.GetValue();
            }
            if (CustomerRet.PriceLevelRef != null)
            {
                //Get value of ListID
                if (CustomerRet.PriceLevelRef.ListID != null)
                {
                    string ListID126 = (string)CustomerRet.PriceLevelRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.PriceLevelRef.FullName != null)
                {
                    string FullName127 = (string)CustomerRet.PriceLevelRef.FullName.GetValue();
                }
            }
            //Get value of ExternalGUID
            if (CustomerRet.ExternalGUID != null)
            {
                string ExternalGUID128 = (string)CustomerRet.ExternalGUID.GetValue();
            }
            if (CustomerRet.CurrencyRef != null)
            {
                //Get value of ListID
                if (CustomerRet.CurrencyRef.ListID != null)
                {
                    string ListID129 = (string)CustomerRet.CurrencyRef.ListID.GetValue();
                }
                //Get value of FullName
                if (CustomerRet.CurrencyRef.FullName != null)
                {
                    string FullName130 = (string)CustomerRet.CurrencyRef.FullName.GetValue();
                }
            }
            if (CustomerRet.DataExtRetList != null)
            {
                for (int i131 = 0; i131 < CustomerRet.DataExtRetList.Count; i131++)
                {
                    IDataExtRet DataExtRet = CustomerRet.DataExtRetList.GetAt(i131);
                    //Get value of OwnerID
                    if (DataExtRet.OwnerID != null)
                    {
                        string OwnerID132 = (string)DataExtRet.OwnerID.GetValue();
                    }
                    //Get value of DataExtName
                    string DataExtName133 = (string)DataExtRet.DataExtName.GetValue();
                    //Get value of DataExtType
                    ENDataExtType DataExtType134 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                    //Get value of DataExtValue
                    string DataExtValue135 = (string)DataExtRet.DataExtValue.GetValue();
                }
            }
        }



    }
}
