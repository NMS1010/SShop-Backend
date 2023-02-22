namespace SShop.ViewModels.System.Users
{
    public class UserCheckEditRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool IsChangePassword { get; set; }
    }
}