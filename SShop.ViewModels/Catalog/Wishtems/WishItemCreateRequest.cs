namespace SShop.ViewModels.Catalog.Wishtems
{
    public class WishItemCreateRequest
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
    }
}