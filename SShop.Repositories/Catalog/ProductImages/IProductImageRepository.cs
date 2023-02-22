using SShop.ViewModels.Catalog.ProductImages;
using SShop.Repositories.Common.Interfaces;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.ProductImages
{
    public interface IProductImageRepository : IModifyEntity<ProductImageCreateRequest, ProductImageUpdateRequest, int>,
        IRetrieveEntity<ProductImageViewModel, ProductImageGetPagingRequest, int>
    {
        Task<int> CreateSingleImage(ProductImageCreateSingleRequest request);
    }
}