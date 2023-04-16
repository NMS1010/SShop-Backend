using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.CartItems
{
    public class CartItemCreateRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        public int Quantity { get; set; } = 1;

        public int Status { get; set; } = 0;
    }
}