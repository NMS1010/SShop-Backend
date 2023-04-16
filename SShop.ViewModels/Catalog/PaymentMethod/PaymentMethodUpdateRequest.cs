using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.PaymentMethod
{
    public class PaymentMethodUpdateRequest : PaymentMethodRequest
    {
        [Required]
        public int PaymentMethodId { get; set; }

        [Required]
        public IFormFile PaymentImage { get; set; }
    }
}