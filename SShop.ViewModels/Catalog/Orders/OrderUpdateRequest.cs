using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Orders
{
    public class OrderUpdateRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int OrderStateId { get; set; }
    }
}