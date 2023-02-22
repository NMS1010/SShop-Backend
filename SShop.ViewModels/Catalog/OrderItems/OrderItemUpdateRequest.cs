using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.OrderItems
{
    public class OrderItemUpdateRequest
    {
        [Required]
        public int OrderItemId { get; set; }
    }
}