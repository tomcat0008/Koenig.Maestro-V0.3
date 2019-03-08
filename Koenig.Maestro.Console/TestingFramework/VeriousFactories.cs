using Koenig.Maestro.Console.TestRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console.TestingFramework
{
    internal class SerializeTesterFactory
    {
        public static IMaestroTester GetTester()
        {
            IMaestroTester result = new SerializationTest();
            return result;
        }

    }

    internal class IntegrityTesterFactory
    {
        public static IMaestroTester GetTester()
        {
            IMaestroTester result = new IntegrityTest();
            return result;
        }
    }
}
