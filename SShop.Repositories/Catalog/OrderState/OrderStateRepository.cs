using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.ViewModels.Catalog.OrderState;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Addresses;

namespace SShop.Repositories.Catalog.OrderState
{
    public class OrderStateRepository : IOrderStateRepository
    {
        private readonly AppDbContext _context;

        public OrderStateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(OrderStateCreateRequest request)
        {
            try
            {
                var orderState = new Domain.Entities.OrderState()
                {
                    OrderStateName = request.OrderStateName,
                };
                _context.OrderStates.Add(orderState);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> Delete(int id)
        {
            try
            {
                var orderState = await _context.OrderStates.FindAsync(id);
                if (orderState == null)
                {
                    return -1;
                }
                _context.OrderStates.Remove(orderState);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private OrderStateViewModel GetOrderStateViewModel(Domain.Entities.OrderState orderState)
        {
            return new OrderStateViewModel()
            {
                OrderStateId = orderState.OrderStateId,
                OrderStateName = orderState.OrderStateName,
            };
        }

        public async Task<PagedResult<OrderStateViewModel>> RetrieveAll(OrderStateGetPagingRequest request)
        {
            try
            {
                var query = await _context.OrderStates
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.OrderStateName.Contains(request.Keyword))
                        .ToList();
                }
                var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetOrderStateViewModel(x)).ToList();

                return new PagedResult<OrderStateViewModel>
                {
                    Items = data,
                    TotalItem = query.Count
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<OrderStateViewModel> RetrieveById(int orderStateId)
        {
            try
            {
                var orderState = await _context.OrderStates.FindAsync(orderStateId);
                if (orderState == null)
                {
                    return null;
                }
                return GetOrderStateViewModel(orderState);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> Update(OrderStateUpdateRequest request)
        {
            try
            {
                var orderState = await _context.OrderStates.FindAsync(request.OrderStateId);
                if (orderState == null)
                {
                    return -1;
                }
                orderState.OrderStateName = request.OrderStateName;
                _context.OrderStates.Update(orderState);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}