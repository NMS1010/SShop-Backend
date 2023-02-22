using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.ReviewItems
{
    public class ReviewItemCreateRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string UserId { get; set; }

        [MaxLength(255)]
        public string Content { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public int Status { get; set; }
    }
}