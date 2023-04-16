using SShop.ViewModels.Catalog.Wishtems;
using SShop.ViewModels.Common;
using SShop.Repositories.Common.Interfaces;

namespace SShop.Repositories.Catalog.WishItems
{
    public interface IWishItemRepository : IModifyEntity<WishItemCreateRequest, WishItemUpdateRequest, int>,
        IRetrieveEntity<WishItemViewModel, WishItemGetPagingRequest, int>
    {
        Task<PagedResult<WishItemViewModel>> RetrieveWishByUserId(string userId);

        Task<object> AddProductToWish(WishItemCreateRequest request);

        Task<int> DeleteAll(string userId);
    }
}