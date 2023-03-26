using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.PaymentMethod
{
    public class PaymentMethodRequest
    {
        [Required]
        public string PaymentMethodName { get; set; }
    }
}