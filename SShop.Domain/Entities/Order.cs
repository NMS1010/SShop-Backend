namespace SShop.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int? DiscountId { get; set; }
        public int AddressId { get; set; }
        public int DeliveryMethodId { get; set; }
        public int OrderStateId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal TotalItemPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDone { get; set; }
        public Address Address { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public HashSet<OrderItem> OrderItems { get; set; }
        public Discount Discount { get; set; }
        public AppUser User { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public OrderState OrderState { get; set; }
    }
}