using SShop.ViewModels.Catalog.Orders;
using SShop.Repositories.Common.Interfaces;
using System.Threading.Tasks;
using SShop.ViewModels.Common;

namespace SShop.Repositories.Catalog.Orders
{
    public interface IOrderRepository : IModifyEntity<OrderCreateRequest, OrderUpdateRequest, int>,
        IRetrieveEntity<OrderViewModel, OrderGetPagingRequest, int>
    {
        Task<OrderOverviewViewModel> GetOverviewStatictis();

        Task<PagedResult<OrderViewModel>> RetrieveByUserId(OrderGetPagingRequest request);
    }
}