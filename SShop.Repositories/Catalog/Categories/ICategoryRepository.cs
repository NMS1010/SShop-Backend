using SShop.ViewModels.Catalog.Categories;
using SShop.ViewModels.Common;
using SShop.Repositories.Common.Interfaces;

namespace SShop.Repositories.Catalog.Categories
{
    public interface ICategoryRepository : IModifyEntity<CategoryCreateRequest, CategoryUpdateRequest, int>,
        IRetrieveEntity<CategoryViewModel, CategoryGetPagingRequest, int>
    {
        List<CategoryViewModel> GetSubCategory(int categoryId);

        Task<PagedResult<CategoryViewModel>> GetParentCategory();
    }
}