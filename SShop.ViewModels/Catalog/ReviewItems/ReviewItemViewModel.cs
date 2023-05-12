using System;

namespace SShop.ViewModels.Catalog.ReviewItems
{
    public class ReviewItemViewModel
    {
        public int ReviewItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public int Status { get; set; }
        public string State { get; set; }
    }
}