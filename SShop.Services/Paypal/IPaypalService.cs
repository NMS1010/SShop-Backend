using SShop.ViewModels.Catalog.CartItems;
using SShop.ViewModels.Catalog.Orders;
using SShop.ViewModels.Common;

namespace SShop.Services.Paypal
{
    public interface IPaypalService
    {
        string PaypalCheckout(PagedResult<CartItemViewModel> products, OrderCreateRequest orderCreateRequest, string hostname);

        bool ExecutePayment(string payerID, string paymentID);
    }
}