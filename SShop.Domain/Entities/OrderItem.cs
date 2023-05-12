﻿namespace SShop.Domain.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int? ReviewItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public ReviewItem ReviewItem { get; set; }
    }
}