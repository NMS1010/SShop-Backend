namespace SShop.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int? DiscountId { get; set; }
        public decimal TotalItemPrice { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalPrice { get; set; }

        public string Address { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDone { get; set; }
        public int Status { get; set; }
        public int Payment { get; set; }
        public HashSet<OrderItem> OrderItems { get; set; }
        public Discount Discount { get; set; }
        public AppUser User { get; set; }
    }
}