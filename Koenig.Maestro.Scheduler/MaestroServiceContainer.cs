using Koenig.Maestro.Entity;
using Koenig.Maestro.Entity.Enums;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Koenig.Maestro.Scheduler
{
    internal class MaestroServiceContainer : IDisposable
    {
        Timer timer;
        string taskName, codeBase, methodName, className;
        int intervall;
        IntervallUnits intervallUnit;

        DateTime lastExecution;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void ServiceExecutedEventHandler(string taskName, DateTime executionTime);

        public event ServiceExecutedEventHandler ServiceExecuted;

        void OnServiceExecutedEventHandler()
        {
            if (this.ServiceExecuted != null)
                ServiceExecuted(taskName, lastExecution);
        }

        public MaestroServiceContainer(MaestroTaskSchedule mtask, DateTime lastExecution)
        {
            taskName = mtask.Name;
            codeBase = mtask.CodeBase;
            methodName = mtask.MethodName;
            intervall = mtask.Intervall;
            className = mtask.ClassName;
            if (!Enum.TryParse<IntervallUnits>(mtask.IntervallUnit, out intervallUnit))
                intervallUnit = IntervallUnits.MINUTE;

            this.lastExecution = lastExecution;

            
        }

        public void Initialize()
        {
            PrepareTimer();
            CheckExecutionLog();
            timer.Start();
        }

        void CheckExecutionLog()
        {

            if (DateTime.MinValue == lastExecution)
            {
                logger.Debug(string.Format("Task `{0}` never executed before, executing for the first time", taskName));
                ExecuteTask();
            }
            else
            {
                TimeSpan span = DateTime.Now.Subtract(this.lastExecution);
                if (span.TotalMilliseconds >= timer.Interval)
                {
                    double delay = span.TotalMilliseconds - intervall;
                    logger.Debug(string.Format("Task `{0}` delayed for {1} miliseconds, executing immediately", taskName, delay));
                    ExecuteTask();
                }
            }

            
        }

        void PrepareTimer()
        {
            logger.Debug(string.Format("Preparing timer for Task `{0}`", taskName));

            timer = new Timer();

            switch(intervallUnit)
            {
                case IntervallUnits.MILISECOND:
                    timer.Interval = intervall;
                    break;
                case IntervallUnits.SECOND:
                    timer.Interval = intervall * 1000;
                    break;
                case IntervallUnits.MINUTE:
                    timer.Interval = intervall * 60000;
                    break;
                case IntervallUnits.HOUR:
                    timer.Interval = intervall * 3600000;
                    break;
                case IntervallUnits.DAY:
                    timer.Interval = intervall * 86400000;
                    break;
                case IntervallUnits.MONTH:
                    timer.Interval = intervall * 2592000000;
                    break;
                case IntervallUnits.YEAR:
                    timer.Interval = intervall * 31104000000;
                    break;
            }


            timer.Elapsed += Timer_Elapsed;
            

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ExecuteTask();
                OnServiceExecutedEventHandler();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, string.Format("Exception occured while executing task `{0}`, class:`{1}`, method:`{2}`", taskName, className, methodName));
            }
        }

        void ExecuteTask()
        {
            logger.Debug(string.Format("Execution starts for Task `{0}`", taskName));
            DateTime start = DateTime.Now;

            Type executableType = GetExecutableType();

            logger.Debug(string.Format("Creating executable instance for Task `{0}`, type:{1}", taskName, className));
            object executableInstance = Activator.CreateInstance(executableType);

            logger.Debug(string.Format("Executing method `{0}` of `{1}` for Task `{2}`", methodName, className, taskName));
            executableType.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, executableInstance, null);

            InsertExecutionLog(start);
        }

        Type GetExecutableType()
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(a=>a.GetName().Name.Equals(codeBase));
            if (assembly == null)
            {
                string error = string.Format("Assembly `{0}` could not be loaded in the current domain", codeBase);
                logger.Fatal(error);
                throw new Exception(error);
            }

            Type result = assembly.GetType(className, true, false);
            return result;

        }

        void InsertExecutionLog(DateTime startDate)
        {
            

            DateTime endDate = DateTime.Now;
            double duration = endDate.Subtract(endDate).TotalMilliseconds;

            logger.Debug(string.Format("Inserting execution log for Task `{0}`, duration:{1} miliseconds", taskName, duration));

            using (Database db = new Database())
            {
                SpCall call = new SpCall("COR.TASK_SCHEDULER_LOG_INSERT");
                call.SetVarchar("@TASK_NAME", taskName);
                call.SetDateTime("@START_DATE", startDate);
                call.SetDateTime("@END_DATE", endDate);
                call.SetInt("@INTERVALL", intervall);
                call.SetVarchar("@INTERVALL_UNIT", intervallUnit.ToString());
                call.SetDecimal("@DURATION", Convert.ToDecimal(duration));
                call.SetDateTime("@CREATE_DATE", DateTime.Now);
                db.ExecuteNonQuery(call);
                
            }

            this.lastExecution = endDate;
        }

        public void Dispose()
        {
            logger.Debug(string.Format("Disposing Task `{0}`", taskName));
            this.timer.Stop();
            this.timer.Dispose();
        }
    }
}
