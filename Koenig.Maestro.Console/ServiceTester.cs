using Koenig.Maestro.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Console
{
    internal class ServiceTester
    {
        public static void Execute()
        {
            try
            {
                MaestroTaskScheduler s = new MaestroTaskScheduler();
                s.TestRun(null);
            }
            catch(Exception ex)
            {

                System.Console.WriteLine("*********************************");
                System.Console.WriteLine("!!!!!!!!!! EXCEPTION !!!!!!!!!!!!");
                System.Console.WriteLine(ex.ToString());
                System.Console.WriteLine("*********************************");
            }

            System.Console.ReadKey();
        }
    }
}
