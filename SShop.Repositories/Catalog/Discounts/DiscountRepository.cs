using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Utilities.Constants.Discounts;
using SShop.ViewModels.Catalog.Discounts;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace SShop.Repositories.Catalog.Discounts
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;

        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> ApllyDiscount(string discountCode)
        {
            try
            {
                var discount = await _context.Discounts
                .Where(p => p.DiscountCode == discountCode)
                .FirstOrDefaultAsync();

                if (discount == null)
                    return "error";
                if (discount.Quantity <= 0)
                    return "out";
                if (discount.StartDate > DateTime.Now || discount.EndDate < DateTime.Now)
                    return "expired";
                if (discount.Status == DISCOUNT_STATUS.IN_ACTIVE)
                    return "suspended";
                discount.Quantity -= 1;
                if (discount.Quantity <= 0)
                {
                    discount.Status = DISCOUNT_STATUS.IN_ACTIVE;
                    discount.Quantity = 0;
                }
                _context.Discounts.Update(discount);
                await _context.SaveChangesAsync();
                return Newtonsoft.Json.JsonConvert.SerializeObject(discount);
            }
            catch
            {
                return "error";
            }
        }

        public async Task<int> Create(DiscountCreateRequest request)
        {
            try
            {
                var discount = new Discount()
                {
                    DiscountCode = request.DiscountCode,
                    DiscountValue = request.DiscountValue,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = request.Status,
                    Quantity = request.Quantity,
                };

                _context.Discounts.Add(discount);

                await _context.SaveChangesAsync();
                return discount.DiscountId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(int discountId)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(discountId);

                if (discount == null)
                    return -1;
                _context.Discounts.Remove(discount);
                return await _context.SaveChangesAsync();
            }
            catch { return -1; }
        }

        public DiscountViewModel GetDiscountViewModel(Discount discount)
        {
            return new DiscountViewModel()
            {
                DiscountId = discount.DiscountId,
                DiscountCode = discount.DiscountCode,
                DiscountValue = discount.DiscountValue,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                Status = discount.Status,
                Quantity = discount.Quantity,
                StatusCode = DISCOUNT_STATUS.DiscountStatus[discount.Status]
            };
        }

        public async Task<PagedResult<DiscountViewModel>> RetrieveAll(DiscountGetPagingRequest request)
        {
            try
            {
                var query = await _context.Discounts
                .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.DiscountCode.Contains(request.Keyword))
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetDiscountViewModel(x)).ToList();

                return new PagedResult<DiscountViewModel>
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

        public async Task<DiscountViewModel> RetrieveById(int discountId)
        {
            try
            {
                var discount = await _context.Discounts
                .Where(p => p.DiscountId == discountId)
                .FirstOrDefaultAsync();
                if (discount == null)
                    return null;
                return GetDiscountViewModel(discount);
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> Update(DiscountUpdateRequest request)
        {
            try
            {
                var discount = await _context.Discounts
                .Where(c => c.DiscountId == request.DiscountId)
                .FirstOrDefaultAsync();
                if (discount == null)
                    return -1;
                discount.DiscountCode = request.DiscountCode;
                discount.DiscountValue = request.DiscountValue;
                discount.StartDate = request.StartDate;
                discount.EndDate = request.EndDate;
                discount.Quantity = request.Quantity;
                if (request.Status == DISCOUNT_STATUS.IN_ACTIVE || request.Status == DISCOUNT_STATUS.EXPIRED)
                {
                    discount.Quantity = 0;
                }

                if (request.Quantity == 0)
                    discount.Status = DISCOUNT_STATUS.IN_ACTIVE;
                else
                    discount.Status = request.Status;
                return await _context.SaveChangesAsync();
            }
            catch { return -1; }
        }
    }
}