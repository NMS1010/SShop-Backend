using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.ViewModels.Catalog.Statistics
{
    public class WeeklyRevenueViewModel
    {
        public decimal MonTotal { get; set; }
        public decimal TueTotal { get; set; }
        public decimal WedTotal { get; set; }
        public decimal ThurTotal { get; set; }
        public decimal FriTotal { get; set; }
        public decimal SatTotal { get; set; }
        public decimal SunTotal { get; set; }
    }
}