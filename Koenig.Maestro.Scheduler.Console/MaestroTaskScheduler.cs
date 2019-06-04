using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.IO;


namespace Koenig.Maestro.Scheduler.Console
{
    public class MaestroTaskScheduler
    {

        public delegate void ConsoleLogEventHandler(object sender, ConsoleLogEventArgs e);

        public event ConsoleLogEventHandler ConsoleLogEvent;


       List<MaestroTaskSchedule> taskList;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        List<MaestroServiceContainer> serviceContainers = new List<MaestroServiceContainer>();
        Dictionary<string, DateTime> executionRegister = new Dictionary<string, DateTime>();

        public MaestroTaskScheduler()
        {
            try
            {
                IConfigurationRoot configRoot = new ConfigurationBuilder().SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
                Maestro.Operation.MaestroApplication.ConfigRoot = configRoot;

            }
            catch (Exception ex)
            {
                OnConsoleLogEvent("Could not start service", ex);
            }

        }

        public void OnConsoleLogEvent(string message, Exception ex)
        {
            if (ex == null)
                logger.Debug(message);
            else
                logger.Fatal(ex, message);

            if (this.ConsoleLogEvent != null)
                this.ConsoleLogEvent(this, new ConsoleLogEventArgs(message, ex));
        }


        public void OnStart(string[] args)
        {
            string msg = "Maestro Task Scheduler is starting...";
            OnConsoleLogEvent(msg, null);

            taskList = Maestro.Operation.Cache.CacheRepository.TaskScheduleCache.Instance.Values.ToList();
            if (taskList.Count == 0)
            {
                msg = "No scheduled task definition found, stopping";
                OnConsoleLogEvent(msg, null);
                OnStop();
                throw new Exception(msg);
            }
            GetLastExecutionLogs();

            msg = "Creating Maestro Service Containers";
            OnConsoleLogEvent(msg, null);


            taskList.ForEach(delegate (MaestroTaskSchedule task)
            {

                DateTime lastExecution = executionRegister.ContainsKey(task.Name) ? executionRegister[task.Name] : DateTime.MinValue;
                MaestroServiceContainer schedule = new MaestroServiceContainer(task, lastExecution);
                schedule.ServiceExecuted += Schedule_ServiceExecuted;
                schedule.LogEvent += Schedule_LogEvent;
                schedule.Initialize();
                serviceContainers.Add(schedule);
            });

            msg = "Service Containers initialized, waiting execution times to come";
            OnConsoleLogEvent(msg, null);

        }

        private void Schedule_LogEvent(string message, Exception ex)
        {
            this.OnConsoleLogEvent(message, ex);
        }

        private void Schedule_ServiceExecuted(string taskName, DateTime executionTime)
        {
            if (executionRegister.ContainsKey(taskName))
                executionRegister[taskName] = executionTime;
            else
                executionRegister.Add(taskName, executionTime);
        }

        public void OnStop()
        {

            string msg = "Maestro Task Scheduler is stopping...";
            OnConsoleLogEvent(msg, null);

            serviceContainers.ForEach(c => c.Dispose());
        }

        void GetLastExecutionLogs()
        {

            string msg = "Getting last execution logs from db";
            OnConsoleLogEvent(msg, null);

            SpCall call = new SpCall("COR.TASK_SCHEDULER_LOG_GET_LATEST");
            using (Database db = new Database())
            {
                using (SqlReader reader = db.ExecuteReader(call))
                {
                    while (reader.Read())
                    {

                        string taskName = reader.GetString("TASK_NAME");
                        DateTime logDate = reader.GetDateTime("LAST_EXECUTED");
                        executionRegister.Add(taskName, logDate);
                    }
                }
            }
        }

    }
}
