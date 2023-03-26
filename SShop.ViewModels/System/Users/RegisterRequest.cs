using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.System.Users
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public IFormFile Avatar { get; set; }

        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string DisplayName { get; set; }

        [Required]
        public string Host { get; set; }
    }
}