using System.ComponentModel.DataAnnotations;

namespace SShop.ViewModels.System.Roles
{
    public class RoleCreateRequest
    {
        [Required]
        public string RoleName { get; set; }
    }
}