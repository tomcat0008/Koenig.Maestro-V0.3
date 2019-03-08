using Koenig.Maestro.Operation.Framework;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console.TestRepository
{
    internal class IntegrityTest : IMaestroTester
    {
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            ResponseMessage result = new ResponseMessage();


            string action = testData["ACTION_TYPE"].ToString();

            switch (action)
            {
                case "ORDER":
                    result.TransactionResult = DoOrder(testData);
                    break;
                case "IMPORT_CUSTOMER":
                    result.TransactionResult = DoImportCustomer(testData);
                    break;
                case "IMPORT_PRODUCT":
                    result.TransactionResult = DoImportProduct(testData);
                    break;
                
                    

            }

            return result;
        }

        ResponseMessage DoOrder(Dictionary<string, object> testData)
        {
            string json = testData["JSON"].ToString();
            if(testData.ContainsKey("ORDER_ID"))
            {
                string orderId = testData["ORDER_ID"].ToString();
                int idx1 = json.IndexOf("\"Id\":", 0);
                int idx2 = json.IndexOf(",", idx1);
                json = json.Replace(json.Substring(idx1 + 6, idx2 - idx1 - 6), orderId);

            }

            ResponseMessage result = MaestroReceiver.ProcessRequest(json);
            return result;
        }

        ResponseMessage DoImportCustomer(Dictionary<string, object> testData)
        {
            string json = testData["JSON"].ToString();
            ResponseMessage result = MaestroReceiver.ProcessRequest(json);
            return result;
        }

        ResponseMessage DoImportProduct(Dictionary<string, object> testData)
        {
            string json = testData["JSON"].ToString();
            ResponseMessage result = MaestroReceiver.ProcessRequest(json);
            return result;
        }

    }
}
