using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Operation.Framework
{
    [Serializable]
    public enum ActionType
    {
        New,
        Update,
        Delete,
        Get,
        List,
        ExportQb,
        ImportQb,
        Undefined,
        Undelete,
        Erase
    }
}
