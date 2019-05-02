using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    public class TaskScheduleCache : DbLoadCache<long, MaestroTaskSchedule>
    {
        TaskScheduleCache() : base("TASK_SCHEDULE_CACHE", "COR.TASK_SCHEDULE_SELECT_ALL")
        {
        }

        static TaskScheduleCache instance = null;

        public static TaskScheduleCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new TaskScheduleCache();
                return instance;
            }
        }

        public MaestroTaskSchedule Get(string taskName)
        {
            return Values.ToList().Find(t => t.Name.Equals(taskName));
        }

        protected override Tuple<long, MaestroTaskSchedule> GetItem(SqlReader reader)
        {
            MaestroTaskSchedule t = new MaestroTaskSchedule();
            t.Id = reader.GetInt64("ID");
            t.Name = reader.GetString("TASK_NAME");
            t.CodeBase = reader.GetString("CODE_BASE");
            t.Intervall = reader.GetInt32("INTERVALL");
            t.IntervallUnit = reader.GetString("INTERVALL_UNIT");
            t.MethodName = reader.GetString("METHOD_NAME");
            t.ClassName = reader.GetString("CLASS_NAME");
            t.CodeBase = reader.GetString("CODE_BASE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            return new Tuple<long, MaestroTaskSchedule>(t.Id, t);

        }

    }
}
