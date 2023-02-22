using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Catalog.CartItems;
using SShop.ViewModels.Common;

namespace SShop.Repositories.Catalog.CartItems
{
    public interface ICartItemRepository : IModifyEntity<CartItemCreateRequest, CartItemUpdateRequest, int>,
        IRetrieveEntity<CartItemViewModel, CartItemGetPagingRequest, int>
    {
        Task<PagedResult<CartItemViewModel>> RetrieveCartByUserId(string userId);

        Task<int> UpdateQuantityByProductId(int productId, int quantity);

        Task<string> AddProductToCart(CartItemCreateRequest request);

        Task<bool> DeleteCartByUserId(string userId);

        Task<int> CanUpdateCartItemQuantity(int cartItemId, int quantity);
    }
}