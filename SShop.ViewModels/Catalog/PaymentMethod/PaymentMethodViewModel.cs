using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.PaymentMethod
{
    public class PaymentMethodViewModel : PaymentMethodRequest
    {
        public int PaymentMethodId { get; set; }
        public string Image { get; set; }
    }
}