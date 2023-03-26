﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Entities
{
    public class DeliveryMethod
    {
        public int DeliveryMethodId { get; set; }
        public int DeliveryMethodName { get; set; }
        public decimal Price { get; set; }
        public HashSet<Order> Orders { get; set; }
    }
}