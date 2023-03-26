using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.System.Addresses
{
    public class AddressRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string SpecificAddress { get; set; }

        [Required]
        public string ProvinceName { get; set; }

        [Required]
        public int ProvinceCode { get; set; }

        [Required]
        public string DistrictName { get; set; }

        [Required]
        public int DistrictCode { get; set; }

        [Required]
        public string WardName { get; set; }

        [Required]
        public int WardCode { get; set; }

        [Required]
        public bool IsDefault { get; set; }
    }
}