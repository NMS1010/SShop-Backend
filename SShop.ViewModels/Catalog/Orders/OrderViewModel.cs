using SShop.ViewModels.Catalog.DeliveryMethod;
using SShop.ViewModels.Catalog.OrderItems;
using SShop.ViewModels.Catalog.OrderState;
using SShop.ViewModels.Catalog.PaymentMethod;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Addresses;
using System;

namespace SShop.ViewModels.Catalog.Orders
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int AddressId { get; set; }
        public string UserFullName { get; set; }
        public string UserPhone { get; set; }
        public int? DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal TotalItemPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDone { get; set; }
        public DateTime? DatePaid { get; set; }
        public int TotalItem { get; set; }

        public DeliveryMethodViewModel DeliveryMethod { get; set; }
        public PaymentMethodViewModel PaymentMethod { get; set; }
        public int OrderStateId { get; set; }
        public string OrderStateName { get; set; }
        public AddressViewModel Address { get; set; }
        public PagedResult<OrderItemViewModel> OrderItems { get; set; }
    }
}