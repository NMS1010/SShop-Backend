using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.ViewModels.Catalog.ReviewItems;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.ReviewItems
{
    public class ReviewItemRepository : IReviewItemRepository
    {
        private readonly AppDbContext _context;

        public ReviewItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(ReviewItemCreateRequest request)
        {
            try
            {
                var review = new ReviewItem()
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    Content = request.Content,
                    Rating = request.Rating,
                    Status = request.Status,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                };

                _context.ReviewItems.Add(review);
                await _context.SaveChangesAsync();

                return review.ReviewItemId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(int reviewId)
        {
            try
            {
                var review = await _context.ReviewItems.FindAsync(reviewId);
                if (review == null)
                    return -1;
                _context.ReviewItems.Remove(review);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<PagedResult<ReviewItemViewModel>> RetrieveAll(ReviewItemGetPagingRequest request)
        {
            try
            {
                var query = await _context.ReviewItems
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.Content.Contains(request.Keyword))
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new ReviewItemViewModel()
                    {
                        ReviewItemId = x.ReviewItemId,
                        UserId = x.UserId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductImage = x.Product.ProductImages
                            .Where(c => c.IsDefault == true && c.ProductId == x.ProductId)
                            .FirstOrDefault()?.Path,
                        Rating = x.Rating,
                        Content = x.Content,
                        DateCreated = x.DateCreated,
                        DateUpdated = x.DateUpdated,
                        Status = x.Status,
                        UserName = x.User.UserName,
                        UserAvatar = x.User.Avatar
                    }).ToList();

                return new PagedResult<ReviewItemViewModel>
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

        public async Task<ReviewItemViewModel> RetrieveById(int reviewItemId)
        {
            try
            {
                var review = await _context.ReviewItems
                    .Where(x => x.ReviewItemId == reviewItemId)
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .FirstOrDefaultAsync();
                if (review == null)
                    return null;
                return new ReviewItemViewModel()
                {
                    ReviewItemId = review.ReviewItemId,
                    ProductId = review.ProductId,
                    ProductName = review.Product.Name,
                    ProductImage = review.Product.ProductImages
                        .Where(c => c.IsDefault == true && c.ProductId == review.ProductId)
                        .FirstOrDefault()?.Path,
                    UserId = review.UserId,
                    Content = review.Content,
                    Rating = review.Rating,
                    DateCreated = review.DateCreated,
                    DateUpdated = review.DateUpdated,
                    Status = review.Status,
                    UserName = review.User.UserName,
                    UserAvatar = review.User.Avatar
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> Update(ReviewItemUpdateRequest request)
        {
            try
            {
                var review = await _context.ReviewItems.FindAsync(request.ReviewItemId);
                if (review == null)
                    return -1;
                review.Content = request.Content;
                review.Rating = request.Rating;
                review.DateUpdated = DateTime.Now;
                review.Status = request.Status;
                _context.ReviewItems.Update(review);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> ChangeReviewStatus(int reviewItemId)
        {
            try
            {
                var reviewItem = await _context.ReviewItems.FindAsync(reviewItemId);
                if (reviewItem == null)
                    return -1;
                reviewItem.Status = reviewItem.Status == 1 ? 0 : 1;
                _context.ReviewItems.Update(reviewItem);
                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<PagedResult<ReviewItemViewModel>> RetrieveReviewsByUser(string userId, int productId)
        {
            try
            {
                var query = await _context.ReviewItems
                    .Where(x => x.ProductId == productId && x.UserId == userId)
                    .Include(x => x.User)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .ToListAsync();
                var data = query
                    .Select(x => new ReviewItemViewModel()
                    {
                        ReviewItemId = x.ReviewItemId,
                        UserId = x.UserId,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductImage = x.Product.ProductImages
                            .Where(c => c.IsDefault == true && c.ProductId == x.ProductId)
                            .FirstOrDefault()?.Path,
                        Rating = x.Rating,
                        Content = x.Content,
                        DateCreated = x.DateCreated,
                        DateUpdated = x.DateUpdated,
                        Status = x.Status,
                        UserName = x.User.UserName,
                        UserAvatar = x.User.Avatar
                    }).ToList();

                return new PagedResult<ReviewItemViewModel>
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
    }
}