using System;
using System.Collections.Generic;

namespace SShop.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public string Origin { get; set; }
        public int Status { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public HashSet<ReviewItem> ReviewItems { get; set; }
        public HashSet<ProductImage> ProductImages { get; set; }
        public HashSet<CartItem> CartItems { get; set; }
        public HashSet<OrderItem> OrderItems { get; set; }
        public HashSet<WishItem> WishItems { get; set; }
    }
}