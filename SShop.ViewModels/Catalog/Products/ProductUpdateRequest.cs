using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.Products
{
    public class ProductUpdateRequest
    {
        public int ProductId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public string Origin { get; set; }

        public IFormFile Image { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }
    }
}