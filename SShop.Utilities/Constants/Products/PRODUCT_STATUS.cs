using System.Collections.Generic;

namespace SShop.Utilities.Constants.Products
{
    public class PRODUCT_STATUS
    {
        public const int IN_STOCK = 0;
        public const int OUT_STOCK = 1;
        public const int SUSPENDED = 2;

        public static Dictionary<int, string> ProductStatus = new Dictionary<int, string>()
        {
            {IN_STOCK, "Còn hàng" },
            {OUT_STOCK, "Hết hàng" },
            {SUSPENDED, "Ngừng kinh doanh" }
        };
    }
}