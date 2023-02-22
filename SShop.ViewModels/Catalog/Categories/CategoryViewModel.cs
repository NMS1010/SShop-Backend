using System.Collections.Generic;

namespace SShop.ViewModels.Catalog.Categories
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }

        public string Image { get; set; }
        public int TotalProduct { get; set; }
        public List<CategoryViewModel> SubCategories { get; set; }
    }
}