using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Categories
{
    public class CategoryUpdateRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public string Content { get; set; }

        public IFormFile Image { get; set; }
    }
}