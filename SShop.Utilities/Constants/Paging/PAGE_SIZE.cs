using System.Collections.Generic;

namespace SShop.Utilities.Constants.Paging
{
    public class PAGE_SIZE
    {
        public static int DefaultPageSize = 2;
        public static int MaxPageSize = 1000;

        public static List<int> PageSize = new List<int>() {
            DefaultPageSize,5,7,10
        };
    }
}