using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Messaging;
using Koenig.Maestro.Operation.UserManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console
{
    internal class MessagingTest : BaseTest
    {
        static MessagingTest instance;

        public static MessagingTest Instance
        {
            get {
                if (instance == null)
                    instance = new MessagingTest();
                return instance;
            }
        }


        void Execute(string transactionType, Dictionary<string, object> testData)
        {

            try
            {
                IMaestroTester tester = MessagingTesterFactory.GetTester(transactionType);
                ResponseMessage result = tester.TriggerTest(testData);
                System.Console.WriteLine("RESULTS RETRIEVED:");
                System.Console.WriteLine("****************************************");
                System.Console.WriteLine(result.ToString());
                Context.TestResult = result;
                System.Console.WriteLine("****************************************");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("!!!!!!!!! EXCEPTION !!!!!!!!!!!!!");
                System.Console.WriteLine(ex.ToString());
            }
            System.Console.WriteLine();
            System.Console.WriteLine("############# PRES ANY KEY #############");
            System.Console.ReadKey();

        }

        #region TransactionDefinition

        public struct Transactions
        {
            static string transactionCode = "TRAN_DEFINITION";

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Add(string codeBase, string description, string tcode)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("CODE_BASE", codeBase);
                testData.Add("GROUP", "DATA");
                testData.Add("DESCRIPTION", description);
                testData.Add("MENU_ID", 0);
                testData.Add("TCODE", tcode);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, string codeBase, string description, string tcode)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("CODE_BASE", codeBase);
                testData.Add("TCODE", tcode);
                testData.Add("MENU_ID", 0);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }

        }

        #endregion TransactionDefinition

        #region Regions

        public struct Regions
        {
            static string transactionCode = "REGION";
            public static void Add(string name, string postalCode)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("NAME", name);
                testData.Add("PK", postalCode);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, string name, string postalCode)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("NAME", name);
                testData.Add("PK", postalCode);
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }

        }



        #endregion Regions

        #region Customers

        public struct Customers
        {
            static string transactionCode = "CUSTOMER";

            public static void Import()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "ImportQb");
                Instance.Execute(transactionCode, testData);
            }

            public static void Add(long regionId, string name, string title)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("REGION_ID", regionId);
                testData.Add("NAME", name);
                testData.Add("TITLE", title);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, long regionId, string name, string title, string phone)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("PHONE", phone);
                testData.Add("REGION_ID", regionId);
                testData.Add("NAME", name);
                testData.Add("TITLE", title);
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion Customers

        #region QbInvoice

        public struct QbInvoice
        {
            static string transactionCode = "QUICKBOOKS_INVOICE";
            public static void Import()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "ImportQb");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion

        #region Products

        public struct Products
        {
            static string transactionCode = "PRODUCT";

            public static void Import()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "ImportQb");
                Instance.Execute(transactionCode, testData);
            }


            public static void Add(int minOrder, decimal price, long unitTypeID, string name, long groupId)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("MINIMUM_ORDER", minOrder);
                testData.Add("PRICE", price);
                testData.Add("UNIT_TYPE_ID", unitTypeID);
                testData.Add("NAME", name);
                testData.Add("GROUP_ID", groupId);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, int minOrder, decimal price, long unitTypeID, long groupId, string name)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("MINIMUM_ORDER", minOrder);
                testData.Add("PRICE", price);
                testData.Add("UNIT_TYPE_ID", unitTypeID);
                testData.Add("NAME", name);
                testData.Add("GROUP_ID", groupId);
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion Product

        #region UnitTypes

        public struct UnitTypes
        {
            static string transactionCode = "UNIT_TYPE";

            public static void Add(string name, string description, bool canHaveUnits)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("NAME", name);
                testData.Add("DESCRIPTION", description);
                testData.Add("CAN_HAVE_UNITS", canHaveUnits);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, string name, string description, bool canHaveUnits)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("NAME", name);
                testData.Add("DESCRIPTION", description);
                testData.Add("CAN_HAVE_UNITS", canHaveUnits);
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion UnitTypes

        #region Units

        public struct Units
        {
            static string transactionCode = "UNIT";

            public static void Add(string name, long unitTypeId, string qbUnits)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("QB_UNIT", qbUnits);
                testData.Add("UNIT_TYPE_ID", unitTypeId);
                testData.Add("NAME", name);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, string name, long unitTypeId,string qbUnits)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("QB_UNIT", qbUnits);
                testData.Add("UNIT_TYPE_ID", unitTypeId);
                testData.Add("NAME", name);
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion Units

        #region CustomerProductUnitTest

        public struct CustomerProductUnits
        {
            static string transactionCode = "CUSTOMER_PRODUCT_UNIT";

            public static void Add(long customerId, long productId, long unitId)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("CUSTOMER_ID", customerId);
                testData.Add("PRODUCT_ID", productId);
                testData.Add("UNIT_ID", unitId);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id, long customerId, long productId, long unitId)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("CUSTOMER_ID", customerId);
                testData.Add("PRODUCT_ID", productId);
                testData.Add("UNIT_ID", unitId);
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion CustomerProductUnitTest

        #region Orders

        public struct Orders
        {
            static string transactionCode = "ORDER";

            public static void GetNewId()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("REQUEST_TYPE", "RequestNewId");
                Instance.Execute(transactionCode, testData);


            }

            public static void Add()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "New");
                testData.Add("REQUEST_TYPE", "InsertNewOrder");
                testData.Add("ID", 1L);
                Instance.Execute(transactionCode, testData);
            }

            public static void Delete(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Delete");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void Update(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Update");
                testData.Add("ID", id);
                testData.Add("QUANTITY", 43);
                testData.Add("NOTES", "Notes about an order");
                Instance.Execute(transactionCode, testData);
            }

            public static void Get(long id)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "Get");
                testData.Add("ID", id);
                Instance.Execute(transactionCode, testData);
            }

            public static void List()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "List");
                Instance.Execute(transactionCode, testData);
            }
        }

        #endregion Orders

    }
}
