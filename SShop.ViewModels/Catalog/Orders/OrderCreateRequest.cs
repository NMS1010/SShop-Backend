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
        public decimal TotalItemPrice { get; set; }

        [Required]
        public decimal Shipping { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public int Payment { get; set; }
    }
}