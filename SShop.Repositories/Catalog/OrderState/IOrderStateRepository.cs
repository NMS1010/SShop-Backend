using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Catalog.OrderState;

namespace SShop.Repositories.Catalog.OrderState
{
    public interface IOrderStateRepository : IModifyEntity<OrderStateCreateRequest, OrderStateUpdateRequest, int>,
        IRetrieveEntity<OrderStateViewModel, OrderStateGetPagingRequest, int>
    {
    }
}