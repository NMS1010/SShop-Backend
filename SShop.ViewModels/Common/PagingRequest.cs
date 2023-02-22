using SShop.Utilities.Constants.Paging;

namespace SShop.ViewModels.Common
{
    public class PagingRequest : RequestBase
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = PAGE_SIZE.MaxPageSize;

        public string ColumnName { get; set; }
        public string TypeSort { get; set; } = "ASC";
        public int SortBy { get; set; }
    }
}