using System.Collections.Generic;

namespace SShop.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public HashSet<Product> Products { get; set; }
    }
}