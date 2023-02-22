using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Catalog.Brands;

namespace SShop.Repositories.Catalog.Brands
{
    public interface IBrandRepository : IModifyEntity<BrandCreateRequest, BrandUpdateRequest, int>,
        IRetrieveEntity<BrandViewModel, BrandGetPagingRequest, int>
    {
    }
}