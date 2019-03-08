using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koenig.Maestro.Operation.Utility
{
    internal class EnumUtils
    {
        public static T GetEnum<T>(string value) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new Exception(string.Format("Unknown enum string `{0}` for type `{1}`", value, typeof(T).FullName));

            return (T)Enum.Parse(typeof(T), value);

        }
    }
}
