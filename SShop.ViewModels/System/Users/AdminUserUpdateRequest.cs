using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.System.Users
{
    public class AdminUserUpdateRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string[] Roles { get; set; }

        [Required]
        public int Status { get; set; }
    }
}