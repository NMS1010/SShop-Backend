using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Wishtems
{
    public class WishItemCreateRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        public int Status { get; set; } = 0;
    }
}