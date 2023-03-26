using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.System.Addresses
{
    public class AddressViewModel : AddressRequest
    {
        public int AddressId { get; set; }
        public int ProvinceId { get; set; }
        public int WardId { get; set; }
        public int DistrictId { get; set; }
    }
}