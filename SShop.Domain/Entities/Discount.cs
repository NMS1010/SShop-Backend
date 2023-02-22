using System;
using System.Collections.Generic;

namespace SShop.Domain.Entities
{
    public class Discount
    {
        public int DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public int Quantity { get; set; }
        public HashSet<Order> Orders { get; set; }
    }
}