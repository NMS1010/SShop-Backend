using System.Collections.Generic;

namespace SShop.ViewModels.Common
{
    public class PagedResult<T>
    {
        public int TotalItem { get; set; }
        public List<T> Items { get; set; }
    }
}