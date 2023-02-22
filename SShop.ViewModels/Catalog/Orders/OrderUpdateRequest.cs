using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Orders
{
    public class OrderUpdateRequest
    {
        [Required]
        public int OrderId { get; set; }

        public int Status { get; set; }
    }
}