using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Cache.CacheRepository;
using Koenig.Maestro.Operation.Data;
using Koenig.Maestro.Operation.UserManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Koenig.Maestro.Operation.Framework.ManagerRepository
{
    internal class UserManager : ManagerBase
    {
        public UserManager(TransactionContext context) : base(context) { }

        public static MaestroUser GetUser(long id)
        {
            return UserCache.Instance[id];
        }

        public static MaestroUser GetUser(string userName)
        {
            return UserCache.Instance.GetUser(userName);
        }

    }
}
