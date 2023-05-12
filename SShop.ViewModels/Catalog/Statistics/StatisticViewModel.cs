using SShop.ViewModels.System.Users;

namespace SShop.ViewModels.Catalog.Statistics
{
    public class StatisticViewModel
    {
        public long TotalCanceled { get; set; }
        public long TotalPending { get; set; }
        public long TotalCompleted { get; set; }
        public long TotalReady { get; set; }
        public long TotalDelivering { get; set; }

        public List<UserViewModel> TopTenUser { get; set; }

        public long TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public long TotalProduct { get; set; }
        public long TotalUsers { get; set; }
    }
}