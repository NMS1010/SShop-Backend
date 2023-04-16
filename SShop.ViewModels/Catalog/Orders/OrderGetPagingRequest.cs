using SShop.ViewModels.Common;

namespace SShop.ViewModels.Catalog.Orders
{
    public class OrderGetPagingRequest : PagingRequest
    {
        public int OrderStateId { get; set; } = 0;
        public string UserId { get; set; }
    }
}