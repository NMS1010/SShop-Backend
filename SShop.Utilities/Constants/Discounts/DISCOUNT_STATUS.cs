using System.Collections.Generic;

namespace SShop.Utilities.Constants.Discounts
{
    public class DISCOUNT_STATUS
    {
        public const int EXPIRED = 0;
        public const int ACTIVE = 1;
        public const int IN_ACTIVE = 2;

        public static Dictionary<int, string> DiscountStatus = new Dictionary<int, string>()
        {
            {EXPIRED, "Hết hạn"},

            {ACTIVE, "Còn mã" },

            {IN_ACTIVE, "Hết mã"}
        };
    }
}