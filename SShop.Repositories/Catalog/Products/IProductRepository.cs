using SShop.ViewModels.Catalog.Products;
using SShop.Repositories.Common.Interfaces;

namespace SShop.Repositories.Catalog.Products
{
    public interface IProductRepository : IModifyEntity<ProductCreateRequest, ProductUpdateRequest, int>,
        IRetrieveEntity<ProductViewModel, ProductGetPagingRequest, int>
    {
    }
}