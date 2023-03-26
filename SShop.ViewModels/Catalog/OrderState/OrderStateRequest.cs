using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.OrderState
{
    public class OrderStateRequest
    {
        [Required]
        public string OrderStateName { get; set; }
    }
}