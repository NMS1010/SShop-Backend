using System;

namespace SShop.ViewModels.Catalog.CartItems
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageProduct { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime DateAdded { get; set; }
        public int Status { get; set; }
        public string ProductStatus { get; set; }
    }
}