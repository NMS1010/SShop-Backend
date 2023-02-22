using System.Collections.Generic;

namespace SShop.Utilities.Constants.Sort
{
    public class SORT_BY
    {
        public const int BY_NAME_AZ = 0;
        public const int BY_NAME_ZA = 1;
        public const int BY_PRICE_AZ = 2;
        public const int BY_PRICE_ZA = 3;

        public static Dictionary<int, string> SortBy = new Dictionary<int, string>(){
            {BY_NAME_AZ, "Tên A-Z" },
            {BY_NAME_ZA, "Tên Z-A" },
            {BY_PRICE_AZ, "Giá tăng dần" },
            {BY_PRICE_ZA, "Giá giảm dần" },
        };
    }
}