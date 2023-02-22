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
        public const int RETURNED = 5;

        public static Dictionary<int, string> OrderStatus = new Dictionary<int, string>()
        {
            {PENDING, "Đang đợi" },
            {READY_TO_SHIP, "Sẵn sàng chuyển đi" },
            {ON_THE_WAY, "Đang chuyển" },
            {DELIVERED, "Đã hoàn thành" },
            {CANCELED, "Đã huỷ" },
            {RETURNED, "Hoàn trả" },
        };

        public static string IsCompleted(int orderStatus, int status)
        {
            if (orderStatus >= status)
                return "completed";
            return "";
        }
    }
}