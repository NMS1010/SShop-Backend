namespace SShop.ViewModels.Catalog.ProductImages
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }

        public string Image { get; set; }

        public bool IsDefault { get; set; }
        public int ProductId { get; set; }
    }
}