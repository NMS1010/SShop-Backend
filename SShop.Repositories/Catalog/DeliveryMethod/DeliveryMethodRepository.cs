using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Repositories.Catalog.DeliveryMethod;
using SShop.Services.FileStorage;
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
        private readonly IFileStorageService _fileStorage;

        public DeliveryMethodRepository(AppDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<int> Create(DeliveryMethodCreateRequest request)
        {
            try
            {
                var deliveryMethod = new Domain.Entities.DeliveryMethod()
                {
                    DeliveryMethodName = request.DeliveryMethodName,
                    Price = request.DeliveryMethodPrice,
                    Image = await _fileStorage.SaveFile(request.DeliveryImage),
                };
                _context.DeliveryMethods.Add(deliveryMethod);
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
                var deliveryMethod = await _context.DeliveryMethods.FindAsync(id) ?? throw new KeyNotFoundException("Cannot find this object");
                _context.DeliveryMethods.Remove(deliveryMethod);
                int count = await _context.SaveChangesAsync();
                if (count <= 0)
                    throw new Exception("Cannot handle delete");
                await _fileStorage.DeleteFile(Path.GetFileName(deliveryMethod.Image));
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DeliveryMethodViewModel GetDeliveryMethodViewModel(Domain.Entities.DeliveryMethod deliveryMethod)
        {
            return new DeliveryMethodViewModel()
            {
                DeliveryMethodId = deliveryMethod.DeliveryMethodId,
                DeliveryMethodName = deliveryMethod.DeliveryMethodName,
                DeliveryMethodPrice = deliveryMethod.Price,
                Image = deliveryMethod.Image,
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
                throw ex;
            }
        }

        public async Task<DeliveryMethodViewModel> RetrieveById(int deliveryMethodId)
        {
            try
            {
                var deliveryMethod = await _context.DeliveryMethods.FindAsync(deliveryMethodId) ?? throw new KeyNotFoundException("Cannot find this object");

                return GetDeliveryMethodViewModel(deliveryMethod);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(DeliveryMethodUpdateRequest request)
        {
            try
            {
                var deliveryMethod = await _context.DeliveryMethods.FindAsync(request.DeliveryMethodId) ?? throw new KeyNotFoundException("Cannot find this object");
                deliveryMethod.DeliveryMethodName = request.DeliveryMethodName;
                deliveryMethod.Price = request.DeliveryMethodPrice;
                if (request.DeliveryImage != null)
                {
                    await _fileStorage.DeleteFile(Path.GetFileName(deliveryMethod.Image));
                    deliveryMethod.Image = await _fileStorage.SaveFile(request.DeliveryImage);
                }
                _context.DeliveryMethods.Update(deliveryMethod);
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