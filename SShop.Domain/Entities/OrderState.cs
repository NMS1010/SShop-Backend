using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class OrderState
    {
        public int OrderStateId { get; set; }
        public string OrderStateName { get; set; }
        public HashSet<Order> Orders { get; set; }
    }
}