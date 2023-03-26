using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Orders
{
    public class OrderCreateRequest
    {
        [Required]
        public string UserId { get; set; }

        public int? DiscountId { get; set; }
        public decimal DiscountValue { get; set; }

        [Required]
        public int DeliveryMethodId { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }

        [Required]
        public decimal TotalItemPrice { get; set; }

        [Required]
        public decimal Shipping { get; set; }

        [Required]
        public int AddressId { get; set; }
    }
}