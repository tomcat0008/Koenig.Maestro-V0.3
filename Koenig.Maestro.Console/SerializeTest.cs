using Koenig.Maestro.Console.TestingFramework;
using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console
{
    internal class SerializeTest : BaseTest
    {
        static SerializeTest instance;
        public static SerializeTest Instance
        {
            get
            {
                if (instance == null)
                    instance = new SerializeTest();
                return instance;
            }
        }

        public void Execute(Dictionary<string, object> testData)
        {

            try
            {
                IMaestroTester tester = SerializeTesterFactory.GetTester();
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

        public struct Serializer
        {
            public static void Serialize()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "SERIALIZE");
                testData.Add("OBJECT", GetOrder());
                Instance.Execute(testData);
            }
        }

        public struct Deserializer
        {
            public static void Deserialize()
            {
                Dictionary<string, object> testData = new Dictionary<string, object>();
                testData.Add("ACTION_TYPE", "DESERIALIZE");
                testData.Add("OBJECT", GetJson());
                Instance.Execute(testData);
            }
        }

        static string GetJson()
        {
            string result = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json.json"), System.Text.Encoding.UTF8);
            return result;
        }

        static OrderMaster GetOrder()
        {
            OrderMaster item = new OrderMaster()
            {
                CreateDate = DateTime.Now.AddDays(-1),
                CreatedUser = "TEST_USER",
                Id = 201902190007,
                Customer = new MaestroCustomer() { Id = 88 },
                Notes = "Some notes about order",
                DeliveryDate = DateTime.Now.AddDays(3),
                OrderDate = DateTime.Now,
                OrderStatus = "ON",
                PaymentType = "COD",
                OrderItems = new List<OrderItem>(
                    new OrderItem[]
                    {
                        new OrderItem()
                        {
                            OrderId = 201902190007,
                            Product = new MaestroProduct(){Id = 2},
                            QbProductMap = new QuickBooksProductMapDef(){Id = 10},
                            Quantity = 12,
                            Unit = new MaestroUnit{Id = 12}
                        },
                        new OrderItem()
                        {
                            OrderId = 201902190007,
                            Product = new MaestroProduct(){Id = 24},
                            Quantity = 11,
                            QbProductMap = new QuickBooksProductMapDef(){Id = 74},
                            Unit = new MaestroUnit{Id = 12}
                        }
                    }
                    )

            };
            return item;
        }
    }

}

