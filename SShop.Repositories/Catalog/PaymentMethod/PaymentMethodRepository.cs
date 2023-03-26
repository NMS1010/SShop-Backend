using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.ViewModels.Catalog.PaymentMethod;
using SShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.PaymentMethod
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly AppDbContext _context;

        public PaymentMethodRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(PaymentMethodCreateRequest request)
        {
            try
            {
                var paymentMethod = new Domain.Entities.PaymentMethod()
                {
                    PaymentMethodName = request.PaymentMethodName,
                };
                _context.PaymentMethods.Add(paymentMethod);
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
                var paymentMethod = await _context.PaymentMethods.FindAsync(id);
                if (paymentMethod == null)
                {
                    return -1;
                }
                _context.PaymentMethods.Remove(paymentMethod);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private PaymentMethodViewModel GetPaymentMethodViewModel(Domain.Entities.PaymentMethod paymentMethod)
        {
            return new PaymentMethodViewModel()
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                PaymentMethodName = paymentMethod.PaymentMethodName,
            };
        }

        public async Task<PagedResult<PaymentMethodViewModel>> RetrieveAll(PaymentMethodGetPagingRequest request)
        {
            try
            {
                var query = await _context.PaymentMethods.ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.PaymentMethodName.Contains(request.Keyword))
                        .ToList();
                }
                var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetPaymentMethodViewModel(x)).ToList();

                return new PagedResult<PaymentMethodViewModel>
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

        public async Task<PaymentMethodViewModel> RetrieveById(int paymentMethodId)
        {
            try
            {
                var paymentMethod = await _context.PaymentMethods.FindAsync(paymentMethodId);
                if (paymentMethod == null)
                {
                    return null;
                }
                return GetPaymentMethodViewModel(paymentMethod);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> Update(PaymentMethodUpdateRequest request)
        {
            try
            {
                var paymentMethod = await _context.PaymentMethods.FindAsync(request.PaymentMethodId);
                if (paymentMethod == null)
                {
                    return -1;
                }
                paymentMethod.PaymentMethodName = request.PaymentMethodName;
                _context.PaymentMethods.Update(paymentMethod);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}