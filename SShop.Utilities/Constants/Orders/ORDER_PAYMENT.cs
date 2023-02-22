using System.Collections.Generic;

namespace SShop.Utilities.Constants.Orders
{
    public class ORDER_PAYMENT
    {
        public static int PAYPAL = 0;
        public static int COD = 1;

        public static Dictionary<int, string> OrderPayment = new Dictionary<int, string>()
        {
            {PAYPAL, "Đã thanh toán" },
            {COD, "Thanh toán khi nhận hàng" }
        };
    }
}