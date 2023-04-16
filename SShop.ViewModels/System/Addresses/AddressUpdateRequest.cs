using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.System.Addresses
{
    public class AddressUpdateRequest : AddressRequest
    {
        [Required]
        public int AddressId { get; set; }

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public int DistrictId { get; set; }
    }
}