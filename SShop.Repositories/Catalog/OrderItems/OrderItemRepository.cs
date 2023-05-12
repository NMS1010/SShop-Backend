using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.ViewModels.Catalog.OrderItems;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace SShop.Repositories.Catalog.OrderItems
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(OrderItemCreateRequest request)
        {
            try
            {
                var orderItem = new OrderItem()
                {
                    OrderId = request.OrderId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    TotalPrice = request.UnitPrice * request.Quantity,
                };

                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

                return orderItem.OrderItemId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(int orderItemId)
        {
            try
            {
                var orderItem = await _context.OrderItems.FindAsync(orderItemId);
                if (orderItem == null)
                    return -1;
                _context.OrderItems.Remove(orderItem);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public OrderItemViewModel GetOrderItemViewModel(OrderItem orderItem)
        {
            return new OrderItemViewModel()
            {
                OrderItemId = orderItem.OrderItemId,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                ProductName = orderItem.Product.Name,
                ProductImage = orderItem.Product.ProductImages
                            .Where(c => c.IsDefault == true && c.ProductId == orderItem.ProductId)
                            .FirstOrDefault()?.Path,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice,
                ProductBrand = orderItem.Product.Brand.BrandName,
                ProductCategory = orderItem.Product.Category.Name,
                ReviewItemId = orderItem.ReviewItemId ?? -1,
            };
        }

        public async Task<PagedResult<OrderItemViewModel>> RetrieveAll(OrderItemGetPagingRequest request)
        {
            try
            {
                var query = await _context.OrderItems
                        .Include(x => x.Product)
                        .ThenInclude(x => x.ProductImages)
                        .Include(x => x.Product)
                        .ThenInclude(x => x.Category)
                        .Include(x => x.Product)
                        .ThenInclude(x => x.Brand)
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetOrderItemViewModel(x)).ToList();

                return new PagedResult<OrderItemViewModel>
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

        public async Task<OrderItemViewModel> RetrieveById(int orderItemId)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .Where(x => x.OrderItemId == orderItemId)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.Brand)
                    .FirstOrDefaultAsync();
                if (orderItem == null)
                    return null;
                return GetOrderItemViewModel(orderItem);
            }
            catch
            {
                return null;
            }
        }

        public async Task<PagedResult<OrderItemViewModel>> RetrieveByOrderId(int orderId)
        {
            try
            {
                var query = await _context.OrderItems
                    .Where(x => x.OrderId == orderId)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.Brand)
                    .ToListAsync();
                var data = query
                .Select(x => GetOrderItemViewModel(x)).ToList();

                return new PagedResult<OrderItemViewModel>
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

        public async Task<int> Update(OrderItemUpdateRequest request)
        {
            try
            {
                var orderItem = await _context.OrderItems.FindAsync(request.OrderItemId);
                if (orderItem == null)
                    return -1;

                _context.OrderItems.Update(orderItem);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}