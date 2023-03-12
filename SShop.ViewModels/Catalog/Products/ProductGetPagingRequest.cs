using SShop.ViewModels.Common;

namespace SShop.ViewModels.Catalog.Products
{
    public class ProductGetPagingRequest : PagingRequest
    {
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = decimal.MaxValue;
        public int[] CategoryIds { get; set; }
        public int[] BrandIds { get; set; }
    }
}