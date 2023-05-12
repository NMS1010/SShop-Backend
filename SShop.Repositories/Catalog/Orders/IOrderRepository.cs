using SShop.ViewModels.Catalog.Orders;
using SShop.Repositories.Common.Interfaces;
using System.Threading.Tasks;
using SShop.ViewModels.Common;
using SShop.ViewModels.Catalog.Statistics;

namespace SShop.Repositories.Catalog.Orders
{
    public interface IOrderRepository : IModifyEntity<OrderCreateRequest, OrderUpdateRequest, int>,
        IRetrieveEntity<OrderViewModel, OrderGetPagingRequest, int>
    {
        Task<StatisticViewModel> GetOverviewStatictis();

        Task<YearlyRevenueViewModel> GetYearlyRevenue(int year);

        Task<WeeklyRevenueViewModel> GetWeeklyRevenue(int year, int month, int day);

        Task<PagedResult<OrderViewModel>> RetrieveByUserId(OrderGetPagingRequest request);
    }
}