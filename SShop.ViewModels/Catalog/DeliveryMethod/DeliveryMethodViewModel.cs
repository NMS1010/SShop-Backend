using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.DeliveryMethod
{
    public class DeliveryMethodViewModel : DeliveryMethodRequest
    {
        public int DeliveryMethodId { get; set; }
        public string Image { get; set; }
    }
}