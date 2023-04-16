using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Utilities.Constants.Products;
using SShop.ViewModels.Catalog.Wishtems;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace SShop.Repositories.Catalog.WishItems
{
    public class WishItemRepository : IWishItemRepository
    {
        private readonly AppDbContext _context;

        public WishItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> AddProductToWish(WishItemCreateRequest request)
        {
            try
            {
                var wishItem = await _context.WishItems
                    .Where(x => x.ProductId == request.ProductId && x.UserId == request.UserId)
                    .FirstOrDefaultAsync();
                if (wishItem != null)
                {
                    throw new Exception("Product has already been in your wish list");
                }

                var currentWishAmount = 0;
                var res = await Create(request);
                if (res < 1)
                {
                    throw new Exception("Cannot add product to your wish list");
                }
                currentWishAmount = (await _context.Users
                    .Where(x => x.Id == request.UserId)
                    .Include(x => x.WishItems)
                    .FirstOrDefaultAsync()).WishItems.Count;
                return new
                {
                    CurrentWishAmount = currentWishAmount
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Create(WishItemCreateRequest request)
        {
            try
            {
                var wishItem = new WishItem()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    DateAdded = DateTime.Now,
                    Status = request.Status,
                };

                _context.WishItems.Add(wishItem);
                await _context.SaveChangesAsync();

                return wishItem.WishItemId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(int wishItemId)
        {
            try
            {
                var wishItem = await _context.WishItems.FindAsync(wishItemId)
                    ?? throw new KeyNotFoundException("Wish item cannot be found");
                _context.WishItems.Remove(wishItem);

                var res = await _context.SaveChangesAsync();
                if (res < 1)
                {
                    throw new Exception("Cannot delete product from your wish list");
                }
                var currentWishAmount = (await _context.Users
                    .Where(x => x.Id == wishItem.UserId)
                    .Include(x => x.WishItems)
                    .FirstOrDefaultAsync()).WishItems.Count;
                return currentWishAmount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteAll(string userId)
        {
            try
            {
                var wishItems = await _context.WishItems
                    .Where(x => x.UserId == userId)
                    .ToListAsync() ?? throw new KeyNotFoundException("Cannot find wish item");
                foreach (var wishItem in wishItems)
                {
                    _context.WishItems.Remove(wishItem);
                }
                var res = await _context.SaveChangesAsync();
                if (res < 1)
                {
                    throw new Exception("Cannot delete all item in your wish");
                }
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public WishItemViewModel GetWishItemViewModel(WishItem wishItem)
        {
            return new WishItemViewModel()
            {
                WishItemId = wishItem.WishItemId,
                ProductId = wishItem.ProductId,
                ProductName = wishItem.Product.Name,
                ProductImage = wishItem.Product.ProductImages
                        .Where(c => c.IsDefault == true && c.ProductId == wishItem.ProductId)
                        .FirstOrDefault()?.Path,
                UserId = wishItem.UserId,
                DateAdded = wishItem.DateAdded,
                Status = wishItem.Status,
                UnitPrice = wishItem.Product.Price,
                UserName = wishItem.User.UserName,
                ProductStatus = PRODUCT_STATUS.ProductStatus[wishItem.Product.Status]
            };
        }

        public async Task<PagedResult<WishItemViewModel>> RetrieveAll(WishItemGetPagingRequest request)
        {
            try
            {
                var query = await _context.WishItems
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
                    .Select(x => GetWishItemViewModel(x)).ToList();

                return new PagedResult<WishItemViewModel>
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

        public async Task<WishItemViewModel> RetrieveById(int wishItemId)
        {
            try
            {
                var wishItem = await _context.WishItems
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Where(x => x.WishItemId == wishItemId)
                    .FirstOrDefaultAsync();
                if (wishItem == null)
                    return null;
                return GetWishItemViewModel(wishItem);
            }
            catch
            {
                return null;
            }
        }

        public async Task<PagedResult<WishItemViewModel>> RetrieveWishByUserId(string userId)
        {
            try
            {
                var query = await _context.WishItems
                    .Where(x => x.UserId == userId)
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();

                var data = query
                    .Select(x => GetWishItemViewModel(x)).ToList();

                return new PagedResult<WishItemViewModel>
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

        public async Task<int> Update(WishItemUpdateRequest request)
        {
            try
            {
                var wishItem = await _context.WishItems.FindAsync(request.WishItemId);
                if (wishItem == null)
                    return -1;
                _context.WishItems.Update(wishItem);
                wishItem.Status = request.Status;
                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}