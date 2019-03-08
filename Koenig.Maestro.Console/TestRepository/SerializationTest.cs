using Koenig.Maestro.Operation.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console.TestRepository
{
    public class SerializationTest : IMaestroTester
    {
        public ResponseMessage TriggerTest(Dictionary<string, object> testData)
        {
            ResponseMessage result = new ResponseMessage();
            

            string action = testData["ACTION_TYPE"].ToString();
            
            switch (action)
            {
                case "SERIALIZE":
                    result.TransactionResult = Serialize(testData["OBJECT"]);
                    break;
                case "DESERIALIZE":
                    result.TransactionResult = Deserialize(testData["OBJECT"].ToString());
                    break;
            }

            return result;
        }

        string Serialize(object obj)
        {
            string result = string.Empty;

            result = JsonConvert.SerializeObject(obj, Formatting.Indented);

            return result;

        }

        object Deserialize(string json)
        {
           
            RequestMessage rm = JsonConvert.DeserializeObject<RequestMessage>(json, new MessageJsonConverter());
            return rm;
        }


    }
}
