using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace SShop.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int Status { get; set; }
        public string Avatar { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredTime { get; set; }
        public HashSet<ReviewItem> ReviewItems { get; set; }
        public HashSet<Order> Orders { get; set; }
        public HashSet<CartItem> CartItems { get; set; }
        public HashSet<WishItem> WishItems { get; set; }
        public HashSet<Address> Addresses { get; set; }
    }
}