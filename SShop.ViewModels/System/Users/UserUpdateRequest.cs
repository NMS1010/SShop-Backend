using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.System.Users
{
    public class UserUpdateRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public IFormFile Avatar { get; set; }

        [Required]
        public string[] Roles { get; set; }
    }
}