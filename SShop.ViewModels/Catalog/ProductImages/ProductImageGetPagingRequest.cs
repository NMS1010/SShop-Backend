using SShop.ViewModels.Common;

namespace SShop.ViewModels.Catalog.ProductImages
{
    public class ProductImageGetPagingRequest : PagingRequest
    {
        public int ProductId { get; set; }
    }
}