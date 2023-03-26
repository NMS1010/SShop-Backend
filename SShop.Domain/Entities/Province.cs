using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class Province
    {
        public int ProvinceId { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public HashSet<Address> Addresses { get; set; }
    }
}