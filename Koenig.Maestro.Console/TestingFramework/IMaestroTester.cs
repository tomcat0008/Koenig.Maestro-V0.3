using Koenig.Maestro.Operation.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Console
{
    internal interface IMaestroTester
    {
        ResponseMessage TriggerTest(Dictionary<string, object> testData);
    }
}
