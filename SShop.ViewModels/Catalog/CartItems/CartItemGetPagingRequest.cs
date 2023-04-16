using SShop.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.Catalog.CartItems
{
    public class CartItemGetPagingRequest : PagingRequest
    {
        [Required]
        public string UserId { get; set; }

        public int Status { get; set; } = 0;
    }
}