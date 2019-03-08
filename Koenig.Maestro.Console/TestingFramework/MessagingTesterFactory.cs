using Koenig.Maestro.Console.TestRepository.TransactionTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console
{
    internal class MessagingTesterFactory
    {
        public static IMaestroTester GetTester(string transaction)
        {
            IMaestroTester result = null;

            switch(transaction)
            {
                case "ORDER":
                    result = new OrderTest();
                    break;
                case "CUSTOMER":
                    result = new CustomerTest();
                    break;
                case "PRODUCT":
                    result = new ProductTest();
                    break;
                case "UNIT":
                    result = new UnitTest();
                    break;
                case "UNIT_TYPE":
                    result = new UnitTypeTest();
                    break;
                case "TRAN_DEFINITION":
                    result = new TranDefTest();
                    break;
                case "REGION":
                    result = new RegionTest();
                    break;
                case "CUSTOMER_PRODUCT_UNIT":
                    result = new CustomerProductUnitTest();
                    break;
                case "QUICKBOOKS_INVOICE":
                    result = new QbInvoiceTest();
                    break;
            }

            return result;
        }
    }
}
