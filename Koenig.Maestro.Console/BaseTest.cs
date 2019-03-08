using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console
{
    internal class BaseTest
    {
        public TestContext Context { get; }

        public BaseTest()
        {
            Context = new TestContext();
        }



    }
}
