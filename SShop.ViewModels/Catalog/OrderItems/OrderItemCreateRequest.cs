using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.OrderItems
{
    public class OrderItemCreateRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }
}