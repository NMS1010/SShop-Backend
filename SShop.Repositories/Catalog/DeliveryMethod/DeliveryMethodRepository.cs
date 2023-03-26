using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Repositories.Catalog.DeliveryMethod;
using SShop.ViewModels.Catalog.DeliveryMethod;
using SShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.DeliveryMethod
{
    public class DeliveryMethodRepository : IDeliveryMethodRepository
    {
        private readonly AppDbContext _context;

        public DeliveryMethodRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(DeliveryMethodCreateRequest request)
        {
            try
            {
                var deliveryMethod = new Domain.Entities.DeliveryMethod()
                {
                    DeliveryMethodName = request.DeliveryMethodName,
                    Price = request.DeliveryMethodPrice
                };
                _context.DeliveryMethods.Add(deliveryMethod);
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
                var deliveryMethod = await _context.DeliveryMethods.FindAsync(id);
                if (deliveryMethod == null)
                {
                    return -1;
                }
                _context.DeliveryMethods.Remove(deliveryMethod);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private DeliveryMethodViewModel GetDeliveryMethodViewModel(Domain.Entities.DeliveryMethod deliveryMethod)
        {
            return new DeliveryMethodViewModel()
            {
                DeliveryMethodId = deliveryMethod.DeliveryMethodId,
                DeliveryMethodName = deliveryMethod.DeliveryMethodName,
                DeliveryMethodPrice = deliveryMethod.Price
            };
        }

        public async Task<PagedResult<DeliveryMethodViewModel>> RetrieveAll(DeliveryMethodGetPagingRequest request)
        {
            try
            {
                var query = await _context.DeliveryMethods.ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.DeliveryMethodName.Contains(request.Keyword))
                        .ToList();
                }
                var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetDeliveryMethodViewModel(x)).ToList();

                return new PagedResult<DeliveryMethodViewModel>
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

        public async Task<DeliveryMethodViewModel> RetrieveById(int deliveryMethodId)
        {
            try
            {
                var deliveryMethod = await _context.DeliveryMethods.FindAsync(deliveryMethodId);
                if (deliveryMethod == null)
                {
                    return null;
                }
                return GetDeliveryMethodViewModel(deliveryMethod);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> Update(DeliveryMethodUpdateRequest request)
        {
            try
            {
                var deliveryMethod = await _context.DeliveryMethods.FindAsync(request.DeliveryMethodId);
                if (deliveryMethod == null)
                {
                    return -1;
                }
                deliveryMethod.DeliveryMethodName = request.DeliveryMethodName;
                deliveryMethod.Price = request.DeliveryMethodPrice;
                _context.DeliveryMethods.Update(deliveryMethod);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}