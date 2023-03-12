using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.System.Roles
{
    public class RoleUpdateRequest
    {
        [Required]
        public string RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}