using Koenig.Maestro.Entity;
using Koenig.Maestro.Operation.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.UserManagement
{
    public abstract class UserBase : DbEntityBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string MidName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Language { get; set; }
    }
}
