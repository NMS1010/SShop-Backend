using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.ProductImages
{
    public class ProductImageCreateRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public List<IFormFile> Images { get; set; }
    }
}