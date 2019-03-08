using System;
using System.Collections.Generic;
using System.Text;

namespace Koenig.Maestro.Entity.Enums
{
    [Serializable]
    public enum OrderRequestType
    {
        RequestNewId,
        InsertNewOrder,
        UpdateQbOrder,
        RecreateOrderItems,
        ListByOrderDate,
        ListByDeliveryDate
    }
}
