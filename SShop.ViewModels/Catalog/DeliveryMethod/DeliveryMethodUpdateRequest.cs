using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.DeliveryMethod
{
    public class DeliveryMethodIUpdateRequest : DeliveryMethodRequest
    {
        [Required]
        public int DeliveryMethodId { get; set; }
    }
}