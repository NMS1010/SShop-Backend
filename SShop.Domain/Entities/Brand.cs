using System.Collections.Generic;

namespace SShop.Domain.Entities
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Origin { get; set; }
        public string Image { get; set; }

        public HashSet<Product> Products { get; set; }
    }
}