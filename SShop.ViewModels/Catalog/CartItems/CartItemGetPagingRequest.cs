using SShop.ViewModels.Common;

namespace SShop.ViewModels.Catalog.CartItems
{
    public class CartItemGetPagingRequest : PagingRequest
    {
        public string UserId { get; set; }
    }
}