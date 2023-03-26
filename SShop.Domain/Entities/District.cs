using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class District
    {
        public int DistrictId { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public HashSet<Address> Addresses { get; set; }
    }
}