using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.ReviewItems
{
    public class ReviewItemUpdateRequest
    {
        [Required]
        public int ReviewItemId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public int Status { get; set; } = 1;
    }
}