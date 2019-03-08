using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console
{
    internal class TestContext
    {
        public object TestResult { get; set; }
        public Dictionary<string, object> ExtendedTestData = new Dictionary<string, object>();
        public TestContext()
        {

        }
    }
}
