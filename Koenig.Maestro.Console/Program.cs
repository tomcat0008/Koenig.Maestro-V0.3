using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;

namespace Koenig.Maestro.Console
{
    class Program
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Debug("starting....");

            IConfigurationRoot configRoot = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
            Maestro.Operation.MaestroApplication.ConfigRoot = configRoot;

            //MessagingTest.Transactions.Add("QuickbooksInvoice", "QB Invoice Integration", "QUICKBOOKS_INVOICE");

            //TestProduct();
            //TestCustomer();
            //TestCustomerProductUnit();
            //TestOrder();

            //MessagingTest.Customers.Import();
            //MessagingTest.Customers.List();
            MessagingTest.Products.Import();
            //MessagingTest.Products.List();

            //OrderAdd();

            //MessagingTest.QbInvoice.Import();
            //MessagingTest.Orders.Update(201902190007);
            //SerializeTest.Deserializer.Deserialize();
            //SerializeTest.Serializer.Serialize();
            //SerializeTest.Deserializer.Deserialize();
            //IntegrityTester.Integrity.OrderUpdateTest(0);
            //IntegrityTester.Integrity.ImportCustomerTest();
            //IntegrityTester.Integrity.ImportProductTest();
        }

        static void OrderAdd()
        {
            MessagingTest.Orders.GetNewId();
            long id = (long)((ResponseMessage)MessagingTest.Instance.Context.TestResult).TransactionResult;
            IntegrityTester.Integrity.OrderNewTest(id);
            //MessagingTest.Orders.Delete(id);
        }

        static void TestProduct()
        {
            MessagingTest.Products.Add(36, 2.56M, 1, "a new product", 1);
            MessagingTest.Products.Add(2, 11.40M, 1, "Daenish", 2);
            MessagingTest.Products.Update(1, 33, 1.1M, 1, 1, "update name");
            MessagingTest.Products.Delete(1);
            MessagingTest.Products.Get(2);
            MessagingTest.Products.List();
        }

        static void TestCustomer()
        {
            MessagingTest.Customers.Add(5, "a new customer", "new title");
            MessagingTest.Customers.Update(4, 5, "updated name", "updated name", "22233");
            MessagingTest.Customers.Delete(4);
            MessagingTest.Customers.Get(4);
            MessagingTest.Customers.List();
        }

        static void TestCustomerProductUnit()
        {
            MessagingTest.CustomerProductUnits.Add(1, 1, 1);
            //MessagingTest.CustomerProductUnits.Add(3, 2, 1);
            MessagingTest.CustomerProductUnits.Update(3, 1, 7, 4);
            MessagingTest.CustomerProductUnits.Delete(2);
            MessagingTest.CustomerProductUnits.Get(1);
            MessagingTest.CustomerProductUnits.List();
        }

        static void TestOrder()
        {
            //MessagingTest.Orders.GetNewId();
            //MessagingTest.Orders.Add();
            //MessagingTest.Orders.Get(1);
            //MessagingTest.Orders.List();
            //MessagingTest.Orders.Delete(1);
            //MessagingTest.Orders.Update(1);
        }

        static void TestUnitType()
        {
            MessagingTest.UnitTypes.Add("A test unit type", "test desc", true);
            MessagingTest.UnitTypes.Update(3, "updated name", "updated desc", false);
            MessagingTest.UnitTypes.Delete(3);
            MessagingTest.UnitTypes.Get(1);
            MessagingTest.UnitTypes.List();
        }

        static void TestUnit()
        {
            MessagingTest.Units.Add("a test unit", 1, null);
            MessagingTest.Units.Add("a test unit2", 1, null);
            MessagingTest.Units.Update(3, "updated name", 1, "A QB UNIT");
            MessagingTest.Units.Delete(3);
            MessagingTest.Units.Get(1);
            MessagingTest.Units.List();
        }

        static void TestRegion()
        {
            MessagingTest.Regions.Add("a new region", "POSTALCODE");
            MessagingTest.Regions.Update(1, "updated region", "updatedpk");
            MessagingTest.Regions.Delete(5);
            MessagingTest.Regions.Delete(1);
            MessagingTest.Regions.List();
        }


    }
}
