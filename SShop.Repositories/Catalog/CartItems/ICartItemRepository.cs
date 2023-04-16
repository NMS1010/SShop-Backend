using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Catalog.CartItems;
using SShop.ViewModels.Common;

namespace SShop.Repositories.Catalog.CartItems
{
    public interface ICartItemRepository : IModifyEntity<CartItemCreateRequest, CartItemUpdateRequest, int>,
        IRetrieveEntity<CartItemViewModel, CartItemGetPagingRequest, int>
    {
        Task<PagedResult<CartItemViewModel>> RetrieveCartByUserId(string userId, int status);

        Task<int> UpdateQuantityByProductId(int productId, int quantity);

        Task<object> AddProductToCart(CartItemCreateRequest request);

        Task<bool> DeleteCartByUserId(string userId);

        Task<int> CanUpdateCartItemQuantity(int cartItemId, int quantity);

        Task<object> UpdateCartItem(CartItemUpdateRequest request);

        Task<object> UpdateAllStatus(string userId, bool selectAll);

        Task<object> DeleteSelectedCartItem(string userId);
    }
}