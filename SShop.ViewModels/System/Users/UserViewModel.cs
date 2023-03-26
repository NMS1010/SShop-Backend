using SShop.ViewModels.Catalog.Orders;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Addresses;
using SShop.ViewModels.System.Roles;
using System.Collections.Generic;

namespace SShop.ViewModels.System.Users
{
    public class UserViewModel
    {
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Dob { get; set; }
        public string Gender { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdated { get; set; }
        public int Status { get; set; }
        public string StatusCode { get; set; }
        public int TotalBought { get; set; }
        public int TotalWishItem { get; set; }
        public int TotalCartItem { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalCost { get; set; }
        public List<string> RoleIds { get; set; }
        public List<RoleViewModel> Roles { get; set; }
        public PagedResult<OrderViewModel> Orders { get; set; }
    }
}