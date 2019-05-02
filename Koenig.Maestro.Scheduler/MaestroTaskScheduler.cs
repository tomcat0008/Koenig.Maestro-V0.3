using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.IO;

namespace Koenig.Maestro.Scheduler
{
    public partial class MaestroTaskScheduler : ServiceBase
    {
        List<MaestroTaskSchedule> taskList;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        List<MaestroServiceContainer> serviceContainers = new List<MaestroServiceContainer>();
        Dictionary<string, DateTime> executionRegister = new Dictionary<string, DateTime>();

        public MaestroTaskScheduler()
        {
            InitializeComponent();
            try
            {
                IConfigurationRoot configRoot = new ConfigurationBuilder().SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();
                Maestro.Operation.MaestroApplication.ConfigRoot = configRoot;
                
            }
            catch(Exception ex)
            {
                logger.Fatal(ex, "Could not start service");
            }

        }

        public void TestRun(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {

            logger.Debug("Maestro Task Scheduler is starting...");

            taskList = Maestro.Operation.Cache.CacheRepository.TaskScheduleCache.Instance.Values.ToList();
            if(taskList.Count == 0)
            {
                logger.Debug("No scheduled task definition found, stopping");
                OnStop();
                throw new Exception("No scheduled task definition found");
            }
            GetLastExecutionLogs();

            logger.Debug("Creating Maestro Service Containers");

            taskList.ForEach(delegate (MaestroTaskSchedule task)
            {

                DateTime lastExecution = executionRegister.ContainsKey(task.Name) ? executionRegister[task.Name] : DateTime.MinValue;
                MaestroServiceContainer schedule = new MaestroServiceContainer(task, lastExecution);
                schedule.ServiceExecuted += Schedule_ServiceExecuted;
                schedule.Initialize();
                serviceContainers.Add(schedule);
            });

            logger.Debug("Service Containers initialized, waiting execution times to come");

        }

        private void Schedule_ServiceExecuted(string taskName, DateTime executionTime)
        {
            if (executionRegister.ContainsKey(taskName))
                executionRegister[taskName] = executionTime;
            else
                executionRegister.Add(taskName, executionTime);
        }

        protected override void OnStop()
        {
            logger.Debug("Maestro Task Scheduler is stopping...");
            serviceContainers.ForEach(c => c.Dispose());
        }

        void GetLastExecutionLogs()
        {
            logger.Debug("Getting last execution logs from db");

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
