using SShop.ViewModels.Catalog.ReviewItems;
using SShop.ViewModels.Common;
using SShop.Repositories.Common.Interfaces;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.ReviewItems
{
    public interface IReviewItemRepository : IModifyEntity<ReviewItemCreateRequest, ReviewItemUpdateRequest, int>,
        IRetrieveEntity<ReviewItemViewModel, ReviewItemGetPagingRequest, int>
    {
        Task<int> ChangeReviewStatus(int reviewItemId);

        Task<PagedResult<ReviewItemViewModel>> RetrieveReviewsByUser(string userId);

        Task<PagedResult<ReviewItemViewModel>> RetrieveReviewsByProduct(int productId);

        Task<ReviewItemViewModel> RetrieveReviewsByOrderItem(int orderItemId);
    }
}