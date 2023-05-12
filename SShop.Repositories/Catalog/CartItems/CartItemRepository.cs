using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Utilities.Constants.Products;
using SShop.ViewModels.Catalog.CartItems;
using SShop.ViewModels.Common;
using System.Net.WebSockets;

namespace SShop.Repositories.Catalog.CartItems
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> AddProductToCart(CartItemCreateRequest request)
        {
            try
            {
                var product = await _context.Products.FindAsync(request.ProductId);
                var cartItem = await _context.CartItems
                    .Where(x => x.ProductId == request.ProductId && x.UserId == request.UserId)
                    .FirstOrDefaultAsync();
                var currentCart = 0;
                var isUpdateQuantity = false;
                if (product.Status == PRODUCT_STATUS.OUT_STOCK)
                {
                    throw new Exception("Product is out of stock");
                }
                if (product.Status == PRODUCT_STATUS.SUSPENDED)
                {
                    throw new Exception("Product is suspended");
                }
                if (product.Quantity > 0)
                {
                    if (cartItem != null)
                    {
                        CartItemUpdateRequest req = new CartItemUpdateRequest()
                        {
                            CartItemId = cartItem.CartItemId,
                            Quantity = cartItem.Quantity + request.Quantity,
                            Status = request.Status,
                            UserId = request.UserId
                        };

                        var res = await UpdateCartItem(req);
                        isUpdateQuantity = true;
                    }
                    else
                    {
                        var res = await Create(request);
                        if (res < 1)
                        {
                            throw new Exception("Cannot add product to cart");
                        }
                    }
                    currentCart = (await _context.Users.Where(x => x.Id == request.UserId).Include(x => x.CartItems).FirstOrDefaultAsync()).CartItems.Count;
                }
                else
                {
                    throw new Exception("Product quantity must larger than 0");
                }
                return new
                {
                    CurrentCartAmount = currentCart,
                    IsUpdateQuantity = isUpdateQuantity,
                };
            }
            catch (Exception ex)
            {
                throw ex;
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
                var cartItem = await _context.CartItems.FindAsync(cartItemId) ?? throw new KeyNotFoundException("Cannot find this item");

                var userId = cartItem.UserId;
                _context.CartItems.Remove(cartItem);
                var res = await _context.SaveChangesAsync();
                if (res < 1)
                    throw new Exception("Failed to delete this item");

                var currentCart = (await _context.Users.Where(x => x.Id == userId).Include(x => x.CartItems).FirstOrDefaultAsync()).CartItems.Count;

                return currentCart;
            }
            catch (Exception e)
            {
                throw e;
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

        public CartItemViewModel GetCartItemViewModel(CartItem cartItem)
        {
            return new CartItemViewModel()
            {
                CartItemId = cartItem.CartItemId,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                ImageProduct = cartItem.Product.ProductImages
                            .Where(c => c.ProductId == cartItem.ProductId && c.IsDefault == true)
                            .FirstOrDefault()?.Path,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.Quantity * cartItem.Product.Price,
                UnitPrice = cartItem.Product.Price,
                DateAdded = DateTime.Now,
                Status = cartItem.Status,
                ProductStatus = PRODUCT_STATUS.ProductStatus[cartItem.Product.Status]
            };
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
                    .Select(x => GetCartItemViewModel(x)).ToList();

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
                return GetCartItemViewModel(cartItem);
            }
            catch
            {
                return null;
            }
        }

        public async Task<PagedResult<CartItemViewModel>> RetrieveCartByUserId(string userId, int status)
        {
            try
            {
                var query = await _context.CartItems
                    .Where(x => x.UserId == userId)
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                if (status != -1)
                    query = query.Where(x => x.Status == status).ToList();
                var data = query
                    .Select(x => GetCartItemViewModel(x)).ToList();

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

        public async Task<object> UpdateCartItem(CartItemUpdateRequest request)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Where(c => c.CartItemId == request.CartItemId)
                    .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Cannot find cart item");

                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product.Quantity < request.Quantity)
                {
                    throw new Exception("Product quantity is not enough");
                }
                cartItem.Quantity = request.Quantity;

                cartItem.Status = request.Status;
                var res = await _context.SaveChangesAsync();
                if (res < 0)
                    throw new Exception("Cannot update cart");

                var query = await _context.CartItems
                    .Where(x => x.UserId == request.UserId && x.Status == 1)
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                var totalPrice = query.Select(x => x.Product.Price * x.Quantity)?.Sum();

                return new
                {
                    TotalSelectedItem = query.Count,
                    TotalPaymentPrice = totalPrice,
                    cartItem = GetCartItemViewModel(cartItem)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(CartItemUpdateRequest request)
        {
            throw new NotImplementedException();
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

        public async Task<object> UpdateAllStatus(string userId, bool selectAll)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Where(c => c.UserId == userId)
                    .ToListAsync() ?? throw new KeyNotFoundException("Cannot find cart item");

                cartItems.ForEach(c => c.Status = selectAll ? 1 : 0);

                var res = await _context.SaveChangesAsync();
                if (res < 0)
                    throw new Exception("Cannot update cart");

                var totalPrice = cartItems.Select(x => x.Product.Price * x.Quantity)?.Sum();

                return new
                {
                    TotalSelectedItem = selectAll ? cartItems.Count : 0,
                    TotalPaymentPrice = selectAll ? totalPrice : 0,
                    CartItems = cartItems.Select(x => GetCartItemViewModel(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> DeleteSelectedCartItem(string userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Where(c => c.UserId == userId)
                    .ToListAsync() ?? throw new KeyNotFoundException("Cannot find cart item");

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Status == 1)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                }

                var res = await _context.SaveChangesAsync();
                if (res < 0)
                    throw new Exception("Cannot delete cart items");

                cartItems = await _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Where(c => c.UserId == userId)
                    .ToListAsync() ?? throw new KeyNotFoundException("Cannot find cart item");
                return new
                {
                    TotalSelectedItem = 0,
                    TotalPaymentPrice = 0,
                    CartItems = cartItems.Select(x => GetCartItemViewModel(x)).ToList(),
                    CurrentCartAmount = cartItems.Count
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}