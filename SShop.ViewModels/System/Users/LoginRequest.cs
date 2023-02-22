using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.System.Users
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}