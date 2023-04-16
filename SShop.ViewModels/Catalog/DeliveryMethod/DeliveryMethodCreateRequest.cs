using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.DeliveryMethod
{
    public class DeliveryMethodCreateRequest : DeliveryMethodRequest
    {
        [Required]
        public IFormFile DeliveryImage { get; set; }
    }
}