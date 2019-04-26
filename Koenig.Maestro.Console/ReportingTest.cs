using Koenig.Maestro.Console.TestingFramework;
using Koenig.Maestro.Console.TestRepository;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console
{
    internal class ReportingTest : BaseTest
    {
        static ReportingTest instance;
        public static ReportingTest Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReportingTest();
                return instance;
            }
        }

        public void Execute(Dictionary<string, object> testData)
        {

            try
            {
                IMaestroTester tester = new ReportTest();
                ResponseMessage result = tester.TriggerTest(testData);
                Context.TestResult = result;
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

        public struct OrderSummary
        {
            public static void GenerateReport(DateTime begin, DateTime end)
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "ORDER_SUMMARY");
                testData.Add("REPORT_CODE", "ORDER_SUMMARY");
                testData.Add("BEGIN_DATE", begin);
                testData.Add("END_DATE", end);



                Instance.Execute(testData);
            }
        }


    }
}
