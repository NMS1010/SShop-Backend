using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class Ward
    {
        public int WardId { get; set; }
        public int WardCode { get; set; }
        public string WardName { get; set; }
        public HashSet<Address> Addresses { get; set; }
    }
}