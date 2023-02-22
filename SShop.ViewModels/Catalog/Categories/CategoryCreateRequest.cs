using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Categories
{
    public class CategoryCreateRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        [MaxLength(255)]
        public string Content { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}