using System;

namespace SShop.Domain.Entities
{
    public class WishItem
    {
        public int WishItemId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public DateTime DateAdded { get; set; }

        public Product Product { get; set; }
        public AppUser User { get; set; }
    }
}