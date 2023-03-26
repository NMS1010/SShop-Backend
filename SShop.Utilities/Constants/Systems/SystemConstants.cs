using System.Collections.Generic;

namespace SShop.Utilities.Constants.Systems
{
    public class SystemConstants
    {
        public class AppSettings
        {
            public static string BearerTokenSession = "BearerToken";
        }

        public class UserRoles
        {
            public static readonly string CUSTOMER_ROLE = "Customer";
            public static readonly string ADMIN_ROLE = "Admin";

            public static readonly List<string> Roles = new()
            {
                ADMIN_ROLE,CUSTOMER_ROLE
            };
        }
    }
}