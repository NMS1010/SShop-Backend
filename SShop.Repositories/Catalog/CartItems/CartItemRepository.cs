using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Utilities.Constants.Products;
using SShop.ViewModels.Catalog.CartItems;
using SShop.ViewModels.Common;

namespace SShop.Repositories.Catalog.CartItems
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddProductToCart(CartItemCreateRequest request)
        {
            try
            {
                string responseStatus;
                var product = await _context.Products.FindAsync(request.ProductId);
                var cartItem = await _context.CartItems
                    .Where(x => x.ProductId == request.ProductId && x.UserId == request.UserId)
                    .FirstOrDefaultAsync();
                if (product.Quantity > 0)
                {
                    if (cartItem != null)
                    {
                        CartItemUpdateRequest req = new CartItemUpdateRequest()
                        {
                            CartItemId = cartItem.CartItemId,
                            Quantity = cartItem.Quantity + 1,
                            Status = request.Status
                        };

                        responseStatus = await Update(req) > 0 ? "repeat" : "error";
                    }
                    else
                    {
                        responseStatus = await Create(request) > 0 ? (await _context.Users.Where(x => x.Id == request.UserId).Include(x => x.CartItems).FirstOrDefaultAsync()).CartItems.Count + "-success" : "error";
                    }
                }
                else
                {
                    responseStatus = "expired";
                }
                return responseStatus;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> CanUpdateCartItemQuantity(int cartItemId, int quantity)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(cartItemId);
                var product = await _context.Products.FindAsync(cartItem.ProductId);

                if (product.Quantity < quantity)
                    return product.Quantity;
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Create(CartItemCreateRequest request)
        {
            try
            {
                var cartItem = new CartItem()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Quantity = request.Quantity,
                    DateAdded = DateTime.Now,
                    Status = request.Status,
                };

                _context.CartItems.Add(cartItem);

                await _context.SaveChangesAsync();
                return cartItem.CartItemId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(int cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(cartItemId);

                if (cartItem == null)
                    return -1;
                _context.CartItems.Remove(cartItem);
                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> DeleteCartByUserId(string userId)
        {
            try
            {
                var list = _context.CartItems.Where(x => x.UserId == userId);
                foreach (var cartItem in list)
                {
                    int res = await Delete(cartItem.CartItemId);
                    if (res <= 0)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PagedResult<CartItemViewModel>> RetrieveAll(CartItemGetPagingRequest request)
        {
            try
            {
                var query = await _context.CartItems
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.Product.Name.Contains(request.Keyword))
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new CartItemViewModel()
                    {
                        CartItemId = x.CartItemId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ImageProduct = x.Product.ProductImages
                            .Where(c => c.ProductId == x.ProductId && c.IsDefault == true)
                            .FirstOrDefault()?.Path,
                        Quantity = x.Quantity,
                        TotalPrice = x.Quantity * x.Product.Price,
                        UnitPrice = x.Product.Price,
                        DateAdded = DateTime.Now,
                        Status = x.Status,
                        ProductStatus = PRODUCT_STATUS.ProductStatus[x.Product.Status]
                    }).ToList();

                return new PagedResult<CartItemViewModel>
                {
                    TotalItem = query.Count,
                    Items = data
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<CartItemViewModel> RetrieveById(int cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Where(p => p.CartItemId == cartItemId)
                    .FirstOrDefaultAsync();
                if (cartItem == null)
                    return null;
                return new CartItemViewModel()
                {
                    CartItemId = cartItem.CartItemId,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    ImageProduct = cartItem.Product.ProductImages
                        .Where(x => x.ProductId == cartItem.ProductId && x.IsDefault == true)
                        .FirstOrDefault()?.Path,
                    Quantity = cartItem.Quantity,
                    TotalPrice = cartItem.Quantity * cartItem.Product.Price,
                    UnitPrice = cartItem.Product.Price,
                    DateAdded = DateTime.Now,
                    Status = cartItem.Status,
                    ProductStatus = PRODUCT_STATUS.ProductStatus[cartItem.Product.Status]
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<PagedResult<CartItemViewModel>> RetrieveCartByUserId(string userId)
        {
            try
            {
                var query = await _context.CartItems
                    .Where(x => x.UserId == userId)
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                var data = query
                    .Select(x => new CartItemViewModel()
                    {
                        CartItemId = x.CartItemId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ImageProduct = x.Product.ProductImages
                            .Where(c => c.ProductId == x.ProductId && c.IsDefault == true)
                            .FirstOrDefault()?.Path,
                        Quantity = x.Quantity,
                        TotalPrice = x.Quantity * x.Product.Price,
                        UnitPrice = x.Product.Price,
                        DateAdded = DateTime.Now,
                        Status = x.Status,
                        ProductStatus = PRODUCT_STATUS.ProductStatus[x.Product.Status]
                    }).ToList();

                return new PagedResult<CartItemViewModel>()
                {
                    TotalItem = data.Count,
                    Items = data
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> Update(CartItemUpdateRequest request)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(x => x.Product)
                    .Where(c => c.CartItemId == request.CartItemId)
                    .FirstOrDefaultAsync();
                if (cartItem == null)
                    return -1;
                cartItem.Quantity = request.Quantity;

                cartItem.Status = request.Status;
                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> UpdateQuantityByProductId(int productId, int quantity)
        {
            try
            {
                var cartItem = await _context.CartItems.Where(x => x.ProductId == productId).ToListAsync();

                foreach (var item in cartItem)
                {
                    item.Quantity = quantity;
                    _context.Update(item);
                }

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}