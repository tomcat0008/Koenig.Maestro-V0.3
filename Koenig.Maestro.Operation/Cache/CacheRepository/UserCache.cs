using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Operation.Cache.CacheRepository
{
    internal class UserCache : DbLoadCache<long, MaestroUser>
    {
        UserCache() : base("USER_CACHE", "USR.USERS_SELECT_ALL")
        {
        }

        static UserCache instance = null;
        public static UserCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new UserCache();
                return instance;
            }
        }

        public MaestroUser GetUser(string userName)
        {
            return Values.ToList().Find(u => u.UserName.Equals(userName));
        }

        protected override Tuple<long, MaestroUser> GetItem(SqlReader reader)
        {
            MaestroUser t = new MaestroUser();
            t.Id = reader.GetInt64("ID");

            t.FirstName = reader.GetString("FIRST_NAME");
            t.MidName = reader.GetString("MID_NAME");
            t.LastName = reader.GetString("LAST_NAME");
            t.UserName = reader.GetString("LOGIN_NAME");
            t.Email = reader.GetString("EMAIL");
            t.Phone = reader.GetString("GSM_PHONE");
            t.Language = reader.GetString("USER_LANGUAGE");
            t.CreateDate = reader.GetDateTime("CREATE_DATE");
            t.RecordStatus = reader.GetString("RECORD_STATUS");
            t.UpdateDate = reader.GetDateTime("UPDATE_DATE");
            t.UpdatedUser = reader.GetString("UPDATE_USER");
            t.CreatedUser = reader.GetString("CREATE_USER");

            return new Tuple<long, MaestroUser>(t.Id, t);

        }
    }
}
