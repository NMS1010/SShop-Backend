using SShop.ViewModels.Catalog.OrderItems;
using SShop.ViewModels.Common;
using SShop.Repositories.Common.Interfaces;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.OrderItems
{
    public interface IOrderItemRepository : IModifyEntity<OrderItemCreateRequest, OrderItemUpdateRequest, int>,
        IRetrieveEntity<OrderItemViewModel, OrderItemGetPagingRequest, int>
    {
        Task<PagedResult<OrderItemViewModel>> RetrieveByOrderId(int orderId);
    }
}