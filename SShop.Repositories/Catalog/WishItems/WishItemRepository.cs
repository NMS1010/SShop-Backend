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

        public async Task<string> AddProductToWish(WishItemCreateRequest request)
        {
            var wishItem = await _context.WishItems.Where(x => x.ProductId == request.ProductId && x.UserId == request.UserId).FirstOrDefaultAsync();
            if (wishItem != null)
            {
                return "error";
            }
            return await Create(request) > 0 ? ((await _context.Users.Where(x => x.Id == request.UserId).Include(x => x.WishItems).FirstOrDefaultAsync()).WishItems.Count) + "success" : "error";
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
                var wishItem = await _context.WishItems.FindAsync(wishItemId);
                if (wishItem == null)
                    return -1;
                _context.WishItems.Remove(wishItem);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
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
                    .Select(x => new WishItemViewModel()
                    {
                        WishItemId = x.WishItemId,
                        UserId = x.UserId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductImage = x.Product.ProductImages
                            .Where(c => c.IsDefault == true && c.ProductId == x.ProductId)
                            .FirstOrDefault()?.Path,
                        DateAdded = x.DateAdded,
                        Status = x.Status,
                        UnitPrice = x.Product.Price,
                        UserName = x.User.UserName,
                        ProductStatus = PRODUCT_STATUS.ProductStatus[x.Product.Status]
                    }).ToList();

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
                    .Select(x => new WishItemViewModel()
                    {
                        WishItemId = x.WishItemId,
                        UserId = x.UserId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductImage = x.Product.ProductImages
                            .Where(c => c.IsDefault == true && c.ProductId == x.ProductId)
                            .FirstOrDefault()?.Path,
                        DateAdded = x.DateAdded,
                        Status = x.Status,
                        UnitPrice = x.Product.Price,
                        UserName = x.User.UserName,
                        ProductStatus = PRODUCT_STATUS.ProductStatus[x.Product.Status]
                    }).ToList();

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