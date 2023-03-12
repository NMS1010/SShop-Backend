using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.ViewModels.Catalog.ProductImages;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SShop.Services.FileStorage;

namespace SShop.Repositories.Catalog.ProductImages
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public ProductImageRepository(AppDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<int> Create(ProductImageCreateRequest request)
        {
            try
            {
                var product = await _context.Products
                    .Where(c => c.ProductId == request.ProductId)
                    .Include(c => c.ProductImages)
                    .FirstOrDefaultAsync();

                if (product == null)
                    return -1;

                foreach (var item in request.Images)
                {
                    _context.ProductImages.Add(new ProductImage()
                    {
                        ProductId = product.ProductId,
                        IsDefault = false,
                        Path = await _fileStorageService.SaveFile(item)
                    });
                }

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(int productImageId)
        {
            try
            {
                var productImage = await _context.ProductImages.FindAsync(productImageId);
                if (productImage == null)
                    return -1;
                _context.ProductImages.Remove(productImage);
                await _fileStorageService.DeleteFile(Path.GetFileName(productImage.Path));
                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public ProductImageViewModel GetProductImageViewModel(ProductImage productImage)
        {
            return new ProductImageViewModel()
            {
                Id = productImage.Id,
                IsDefault = productImage.IsDefault,
                Image = productImage.Path,
                ProductId = productImage.ProductId,
            };
        }

        public async Task<ProductImageViewModel> RetrieveById(int productImageId)
        {
            try
            {
                var productImage = await _context.ProductImages.FindAsync(productImageId);
                if (productImage == null)
                    return null;
                return GetProductImageViewModel(productImage);
            }
            catch { return null; }
        }

        public async Task<int> Update(ProductImageUpdateRequest request)
        {
            try
            {
                var productImg = await _context.ProductImages.FindAsync(request.ProductImageId);
                if (productImg == null)
                    return -1;
                if (request.Image == null)
                    return -1;

                await _fileStorageService.DeleteFile(Path.GetFileName(productImg.Path));
                productImg.Path = await _fileStorageService.SaveFile(request.Image);

                _context.ProductImages.Update(productImg);

                return await _context.SaveChangesAsync();
            }
            catch { return -1; }
        }

        public async Task<PagedResult<ProductImageViewModel>> RetrieveAll(ProductImageGetPagingRequest request)
        {
            try
            {
                var query = await _context.ProductImages
                    .Where(c => c.ProductId == request.ProductId)
                    .ToListAsync();

                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetProductImageViewModel(x))
                    .ToList();

                return new PagedResult<ProductImageViewModel>
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

        public async Task<int> CreateSingleImage(ProductImageCreateSingleRequest request)
        {
            try
            {
                var product = await _context.Products
                    .Where(c => c.ProductId == request.ProductId)
                    .Include(c => c.ProductImages)
                    .FirstOrDefaultAsync();

                if (product == null)
                    return -1;
                var productImg = new ProductImage()
                {
                    ProductId = product.ProductId,
                    IsDefault = false,
                    Path = await _fileStorageService.SaveFile(request.Image)
                };
                _context.ProductImages.Add(productImg);

                await _context.SaveChangesAsync();
                return productImg.Id;
            }
            catch
            {
                return -1;
            }
        }
    }
}