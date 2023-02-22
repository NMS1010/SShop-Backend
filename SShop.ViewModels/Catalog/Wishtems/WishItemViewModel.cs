using System;

namespace SShop.ViewModels.Catalog.Wishtems
{
    public class WishItemViewModel
    {
        public int WishItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime DateAdded { get; set; }
        public string ProductStatus { get; set; }
    }
}