using System.Collections.Generic;

namespace SShop.Utilities.Constants.Orders
{
    public class ORDER_STATUS
    {
        public const int PENDING = 0;
        public const int READY_TO_SHIP = 1;
        public const int ON_THE_WAY = 2;
        public const int DELIVERED = 3;
        public const int CANCELED = 4;

        public static Dictionary<int, string> OrderStatus = new Dictionary<int, string>()
        {
            {PENDING, "Pending" },
            {READY_TO_SHIP, "Ready to ship" },
            {ON_THE_WAY, "On the way" },
            {DELIVERED, "Delivered" },
            {CANCELED, "Cancelled" },
        };
    }
}