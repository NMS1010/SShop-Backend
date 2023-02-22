using SShop.ViewModels.Catalog.Discounts;
using SShop.Repositories.Common.Interfaces;
using System.Threading.Tasks;

namespace SShop.Repositories.Catalog.Discounts
{
    public interface IDiscountRepository : IModifyEntity<DiscountCreateRequest, DiscountUpdateRequest, int>,
        IRetrieveEntity<DiscountViewModel, DiscountGetPagingRequest, int>
    {
        Task<string> ApllyDiscount(string discountCode);
    }
}