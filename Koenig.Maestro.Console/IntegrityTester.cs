using Koenig.Maestro.Console.TestingFramework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console
{
    internal class IntegrityTester : BaseTest
    {
        static IntegrityTester instance;
        public static IntegrityTester Instance
        {
            get
            {
                if (instance == null)
                    instance = new IntegrityTester();
                return instance;
            }
        }


        public void Execute(Dictionary<string, object> testData)
        {

            try
            {
                IMaestroTester tester = IntegrityTesterFactory.GetTester();
                ResponseMessage result = tester.TriggerTest(testData);
                System.Console.WriteLine("RESULTS RETRIEVED:");
                System.Console.WriteLine("****************************************");
                System.Console.WriteLine(result.ToString());
                Context.TestResult = result;
                System.Console.WriteLine("****************************************");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("!!!!!!!!! EXCEPTION !!!!!!!!!!!!!");
                System.Console.WriteLine(ex.ToString());
            }
            System.Console.WriteLine();
            System.Console.WriteLine("############# PRES ANY KEY #############");
            System.Console.ReadKey();
        }

        public struct Integrity
        {
            public static void OrderNewTest(long id)
            {
                string json = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orderNew.json"), System.Text.Encoding.UTF8);
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "ORDER");
                testData.Add("JSON", json);
                if(id > 0)
                    testData.Add("ORDER_ID", id);
                Instance.Execute(testData);
            }

            public static void OrderUpdateTest(long id)
            {
                string json = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orderUpdate.json"), System.Text.Encoding.UTF8);
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "ORDER");
                testData.Add("JSON", json);
                if (id > 0)
                    testData.Add("ORDER_ID", id);
                Instance.Execute(testData);
            }


            public static void ImportCustomerTest()
            {
                string json = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customerImport.json"), System.Text.Encoding.UTF8);
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "IMPORT_CUSTOMER");
                testData.Add("JSON", json);
                Instance.Execute(testData);
            }

            public static void ImportProductTest()
            {
                string json = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "productImport.json"), System.Text.Encoding.UTF8);
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "IMPORT_PRODUCT");
                testData.Add("JSON", json);
                Instance.Execute(testData);
            }
        }
    }
}
