using System.Collections.Generic;

namespace SShop.Utilities.Constants.Users
{
    public class USER_STATUS
    {
        public static int IN_ACTIVE = 0;
        public static int ACTIVE = 1;

        public static Dictionary<int, string> UserStatus = new Dictionary<int, string>()
        {
            {ACTIVE, "Đang hoạt động" },
            {IN_ACTIVE, "Ngưng hoạt động" }
        };
    }
}