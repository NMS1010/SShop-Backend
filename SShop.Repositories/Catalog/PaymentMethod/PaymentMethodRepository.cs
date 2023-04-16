using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Services.FileStorage;
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
        private readonly IFileStorageService _fileStorage;

        public PaymentMethodRepository(AppDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<int> Create(PaymentMethodCreateRequest request)
        {
            try
            {
                var paymentMethod = new Domain.Entities.PaymentMethod()
                {
                    PaymentMethodName = request.PaymentMethodName,
                    Image = await _fileStorage.SaveFile(request.PaymentImage),
                };
                _context.PaymentMethods.Add(paymentMethod);
                var count = await _context.SaveChangesAsync();
                if (count <= 0)
                    throw new Exception("Cannot handle add");
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete(int id)
        {
            try
            {
                var paymentMethod = await _context.PaymentMethods.FindAsync(id) ?? throw new KeyNotFoundException("Cannot find this object");

                _context.PaymentMethods.Remove(paymentMethod);
                int count = await _context.SaveChangesAsync();
                if (count <= 0)
                    throw new Exception("Cannot handle delete");
                await _fileStorage.DeleteFile(Path.GetFileName(paymentMethod.Image));
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PaymentMethodViewModel GetPaymentMethodViewModel(Domain.Entities.PaymentMethod paymentMethod)
        {
            return new PaymentMethodViewModel()
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                PaymentMethodName = paymentMethod.PaymentMethodName,
                Image = paymentMethod.Image,
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
                throw ex;
            }
        }

        public async Task<PaymentMethodViewModel> RetrieveById(int paymentMethodId)
        {
            try
            {
                var paymentMethod = await _context.PaymentMethods.FindAsync(paymentMethodId) ?? throw new KeyNotFoundException("Cannot find this object");
                return GetPaymentMethodViewModel(paymentMethod);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(PaymentMethodUpdateRequest request)
        {
            try
            {
                var paymentMethod = await _context.PaymentMethods.FindAsync(request.PaymentMethodId) ?? throw new KeyNotFoundException("Cannot find this object");
                paymentMethod.PaymentMethodName = request.PaymentMethodName;
                if (request.PaymentImage != null)
                {
                    await _fileStorage.DeleteFile(Path.GetFileName(paymentMethod.Image));
                    paymentMethod.Image = await _fileStorage.SaveFile(request.PaymentImage);
                }
                _context.PaymentMethods.Update(paymentMethod);
                var count = await _context.SaveChangesAsync();
                if (count <= 0)
                    throw new Exception("Cannot handle update");
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}