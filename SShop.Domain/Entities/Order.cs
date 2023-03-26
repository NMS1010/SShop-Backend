namespace SShop.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int? DiscountId { get; set; }
        public int AddressId { get; set; }
        public decimal TotalItemPrice { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalPrice { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateDone { get; set; }
        public int Status { get; set; }
        public int Payment { get; set; }
        public Address Address { get; set; }
        public HashSet<OrderItem> OrderItems { get; set; }
        public Discount Discount { get; set; }
        public AppUser User { get; set; }
    }
}