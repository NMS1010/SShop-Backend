using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.ReviewItems
{
    public class ReviewItemCreateRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public int OrderItemId { get; set; }
    }
}