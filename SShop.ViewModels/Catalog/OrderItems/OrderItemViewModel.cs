namespace SShop.ViewModels.Catalog.OrderItems
{
    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductBrand { get; set; }
        public string ProductCategory { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int ReviewItemId { get; set; }
    }
}