using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.System.Users
{
    public class GoogleLoginRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string LoginProvider { get; set; }

        [Required]
        public string ProviderKey { get; set; }
    }
}