using System.Collections.Generic;

namespace SShop.Utilities.Constants.Users
{
    public class USER_STATUS
    {
        public static readonly int IN_ACTIVE = 0;
        public static readonly int ACTIVE = 1;

        public static readonly Dictionary<int, string> UserStatus = new()
        {
            {ACTIVE, "Đang hoạt động" },
            {IN_ACTIVE, "Ngưng hoạt động" }
        };
    }
}