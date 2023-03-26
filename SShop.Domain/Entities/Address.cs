using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class Address
    {
        public string UserId { get; set; }
        public int AddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ProvinceId { get; set; }
        public int WardId { get; set; }
        public int DistrictId { get; set; }
        public string Phone { get; set; }
        public string SpecificAddress { get; set; }
        public bool IsDefault { get; set; }
        public Province Province { get; set; }
        public Ward Ward { get; set; }
        public District District { get; set; }
        public AppUser User { get; set; }
        public HashSet<Order> Orders { get; set; }
    }
}