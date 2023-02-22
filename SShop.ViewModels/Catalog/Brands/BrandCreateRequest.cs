using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Brands
{
    public class BrandCreateRequest
    {
        [Required]
        [MaxLength(255)]
        public string BrandName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Origin { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}