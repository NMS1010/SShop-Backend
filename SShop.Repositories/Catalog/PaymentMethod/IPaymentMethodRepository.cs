using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Catalog.PaymentMethod;

namespace SShop.Repositories.Catalog.PaymentMethod
{
    public interface IPaymentMethodRepository : IModifyEntity<PaymentMethodCreateRequest, PaymentMethodUpdateRequest, int>,
        IRetrieveEntity<PaymentMethodViewModel, PaymentMethodGetPagingRequest, int>
    {
    }
}