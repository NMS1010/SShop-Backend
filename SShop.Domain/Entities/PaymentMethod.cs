using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public HashSet<Order> Orders { get; set; }
        public string Image { get; set; }
    }
}