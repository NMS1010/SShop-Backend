using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Utilities.Constants.Orders;
using SShop.ViewModels.Catalog.Orders;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using SShop.Repositories.Catalog.OrderItems;
using SShop.Services.MailJet;
using SShop.ViewModels.System.Addresses;
using SShop.Repositories.System.Addresses;
using SShop.ViewModels.Catalog.Statistics;
using SShop.Repositories.System.Users;
using System.Reflection;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using SShop.Utilities.Constants.Users;
using SShop.ViewModels.System.Users;

namespace SShop.Repositories.Catalog.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMailJetServices _mailJetServices;

        public OrderRepository(AppDbContext context, IOrderItemRepository orderItemRepository, IMailJetServices mailJetServices
            , IAddressRepository addressRepository)
        {
            _context = context;
            _orderItemRepository = orderItemRepository;
            _mailJetServices = mailJetServices;
            _addressRepository = addressRepository;
        }

        public async Task<int> Create(OrderCreateRequest request)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var orderState = await _context.OrderStates
                    .Where(x => x.OrderStateName == "Pending").FirstOrDefaultAsync() ?? throw new Exception("Error while handling action");
                var order = new Order()
                {
                    UserId = request.UserId,
                    DiscountId = request.DiscountId ?? null,
                    TotalItemPrice = request.TotalItemPrice,
                    TotalPrice = request.Shipping + request.TotalItemPrice,
                    AddressId = request.AddressId,
                    DateCreated = DateTime.Now,
                    DateDone = null,
                    OrderStateId = orderState.OrderStateId,
                    PaymentMethodId = request.PaymentMethodId,
                    DeliveryMethodId = request.DeliveryMethodId
                };
                var paymentMethod = await _context.PaymentMethods
                    .Where(x => x.PaymentMethodName == "Paypal" && x.PaymentMethodId == request.PaymentMethodId)
                    .FirstOrDefaultAsync();
                if (paymentMethod != null)
                {
                    order.DatePaid = DateTime.Now;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                var user = await _context.Users
                    .Where(x => x.Id == request.UserId)
                    .Include(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync() ?? throw new Exception("Error while handling action");
                if (user.CartItems.Where(x => x.Status == 1).ToList().Count <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Please select your product to order");
                }
                foreach (var cartItem in user.CartItems)
                {
                    var orderItem = new OrderItem()
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Order = order,
                        Product = cartItem.Product,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price,
                        TotalPrice = cartItem.Quantity * cartItem.Product.Price,
                    };
                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    product.Quantity -= cartItem.Quantity;
                    _context.Products.Update(product);
                    _context.OrderItems.Add(orderItem);
                    _context.CartItems.Remove(cartItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                bool res = await _mailJetServices.SendMail(user.FirstName + " " + user.LastName, user.Email,
              "<h2>Chào " + user.FirstName + " " + user.LastName + " </h2>, <h3>FurSshop cảm ơn vì đã tin tưởng mua sản phẩm, đơn hàng sẽ nhanh chóng đến tay của bạn.<br />Bạn có thể xem chi tiết đơn hàng trong mục Đơn hàng của tôi. </h3><h4>Xin chân thành cảm ơn bạn !!! Rất vui được phục vụ.</h4>",
              "Đơn xác nhận đặt hàng");
                if (!res)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error while handling action");
                }
                return order.OrderId;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
        }

        public async Task<int> Delete(int orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                    return -1;
                _context.Orders.Remove(order);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public OrderViewModel GetOrderViewModel(Order order)
        {
            return new OrderViewModel()
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                UserFullName = order.User.FirstName + " " + order.User.LastName,
                UserPhone = order.User.PhoneNumber,
                DiscountId = order?.DiscountId,
                DiscountCode = order.Discount?.DiscountCode,
                DiscountValue = order.Discount?.DiscountValue,
                TotalItemPrice = order.TotalItemPrice,
                TotalPrice = order.TotalPrice,
                DateCreated = order.DateCreated,
                DateDone = order.DateDone,
                DatePaid = order.DatePaid,
                DeliveryMethod = new ViewModels.Catalog.DeliveryMethod.DeliveryMethodViewModel()
                {
                    DeliveryMethodId = order.DeliveryMethodId,
                    DeliveryMethodName = order.DeliveryMethod.DeliveryMethodName,
                    DeliveryMethodPrice = order.DeliveryMethod.Price,
                    Image = order.DeliveryMethod?.Image,
                },
                OrderStateId = order.OrderStateId,
                OrderStateName = order.OrderState.OrderStateName,
                PaymentMethod = new ViewModels.Catalog.PaymentMethod.PaymentMethodViewModel()
                {
                    PaymentMethodId = order.PaymentMethodId,
                    PaymentMethodName = order.PaymentMethod.PaymentMethodName,
                    Image = order.PaymentMethod.Image
                },
                TotalItem = order.OrderItems.Count,
                AddressId = order.AddressId
            };
        }

        public async Task<PagedResult<OrderViewModel>> RetrieveAll(OrderGetPagingRequest request)
        {
            try
            {
                var query = await _context.Orders
                    .Include(x => x.Discount)
                    .Include(x => x.OrderItems)
                    .Include(x => x.User)
                    .Include(x => x.Discount)
                    .Include(x => x.Address)
                    .Include(x => x.PaymentMethod)
                    .Include(x => x.OrderState)
                    .Include(x => x.DeliveryMethod)
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.Address.SpecificAddress.Contains(request.Keyword))
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetOrderViewModel(x)).ToList();
                foreach (var d in data)
                {
                    d.OrderItems = await _orderItemRepository.RetrieveByOrderId(d.OrderId);
                    d.Address = await _addressRepository.RetrieveById(d.AddressId);
                }
                return new PagedResult<OrderViewModel>
                {
                    TotalItem = query.Count,
                    Items = data
                };
            }
            catch
            {
                throw new Exception("Failed to get order list");
            }
        }

        public async Task<OrderViewModel> RetrieveById(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Where(x => x.OrderId == orderId)
                    .Include(x => x.Discount)
                    .Include(x => x.OrderItems)
                    .Include(x => x.User)
                    .Include(x => x.Discount)
                    .Include(x => x.Address)
                    .Include(x => x.PaymentMethod)
                    .Include(x => x.OrderState)
                    .Include(x => x.DeliveryMethod)
                    .FirstOrDefaultAsync();
                if (order == null)
                    return null;
                var res = GetOrderViewModel(order);
                res.Address = await _addressRepository.RetrieveById(order.AddressId);
                res.OrderItems = await _orderItemRepository.RetrieveByOrderId(order.OrderId);
                return res;
            }
            catch
            {
                throw new Exception("Failed to get order");
            }
        }

        public async Task<int> Update(OrderUpdateRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .Where(x => x.OrderId == request.OrderId)
                    .Include(x => x.OrderState)
                    .Include(x => x.PaymentMethod)
                    .FirstOrDefaultAsync()
                    ?? throw new KeyNotFoundException("Cannot find this order");
                var orderState = await _context.OrderStates
                    .FindAsync(request.OrderStateId)
                    ?? throw new KeyNotFoundException("Cannot find this state"); ;
                if (order.OrderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.DELIVERED])
                    throw new Exception("This order has been deliveried");
                order.OrderStateId = request.OrderStateId;
                if (orderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.DELIVERED])
                {
                    var d = DateTime.Now;
                    order.DateDone = d;
                    if (order.PaymentMethod.PaymentMethodName == "COD")
                    {
                        order.DatePaid = d;
                    }
                }

                _context.Orders.Update(order);

                var res = await _context.SaveChangesAsync();
                if (res < 1)
                    throw new Exception("Failed to update order state");
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private UserViewModel GetUserViewModel(AppUser user)
        {
            return new UserViewModel()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                UserName = user.UserName,
                Dob = user.DateOfBirth.ToString("yyyy-MM-dd"),
                Gender = user.Gender,
                Avatar = user.Avatar,
                DateCreated = user.DateCreated.ToString(),
                DateUpdated = user.DateUpdated.ToString(),
                Status = user.Status,
                TotalCartItem = user.CartItems.Count,
                TotalWishItem = user.WishItems.Count,
                TotalOrders = user.Orders.Count,
                TotalBought = user.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity)),
                TotalCost = user.Orders.Sum(o => o.TotalPrice),
                StatusCode = USER_STATUS.UserStatus[user.Status],
            };
        }

        public async Task<StatisticViewModel> GetOverviewStatictis()
        {
            var orders = await _context.Orders.Include(x => x.OrderState).ToListAsync();
            var products = await _context.Products.ToListAsync();
            var users = await _context.Users
                .Include(x => x.Orders)
                .ThenInclude(x => x.OrderItems)
                .Include(x => x.CartItems)
                .Include(x => x.WishItems)
                .ToListAsync();
            var totalProduct = products.Select(x => x.Quantity).Sum();
            var topTen = users.OrderByDescending(x => x.Orders.Select(o => o.TotalPrice).Sum()).Take(10);
            StatisticViewModel statistic = new()
            {
                TotalPending = orders.Where(x => x.OrderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.PENDING]).Count(),
                TotalCanceled = orders.Where(x => x.OrderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.CANCELED]).Count(),
                TotalReady = orders.Where(x => x.OrderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.READY_TO_SHIP]).Count(),
                TotalCompleted = orders.Where(x => x.OrderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.DELIVERED]).Count(),
                TotalDelivering = orders.Where(x => x.OrderState.OrderStateName == ORDER_STATUS.OrderStatus[ORDER_STATUS.ON_THE_WAY]).Count(),
                TotalOrders = orders.Count,
                TotalRevenue = orders.Select(x => x.TotalPrice).Sum(),
                TotalProduct = totalProduct,
                TotalUsers = users.Count,
                TopTenUser = topTen.Select(x => GetUserViewModel(x)).ToList()
            };

            return statistic;
        }

        public async Task<PagedResult<OrderViewModel>> RetrieveByUserId(OrderGetPagingRequest request)
        {
            try
            {
                var query = await _context.Orders
                    .Include(x => x.Discount)
                    .Include(x => x.OrderItems)
                    .Include(x => x.User)
                    .Include(x => x.Discount)
                    .Include(x => x.Address)
                    .Include(x => x.PaymentMethod)
                    .Include(x => x.OrderState)
                    .Include(x => x.DeliveryMethod)
                    .Where(x => x.UserId == request.UserId)
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.Address.SpecificAddress.Contains(request.Keyword))
                        .ToList();
                }
                if (request.OrderStateId != 0)
                {
                    query = query
                        .Where(x => x.OrderStateId == request.OrderStateId)
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetOrderViewModel(x)).ToList();
                foreach (var d in data)
                {
                    d.OrderItems = await _orderItemRepository.RetrieveByOrderId(d.OrderId);
                    d.Address = await _addressRepository.RetrieveById(d.AddressId);
                }
                return new PagedResult<OrderViewModel>
                {
                    TotalItem = query.Count,
                    Items = data
                };
            }
            catch
            {
                throw new Exception("Failed to get order list");
            }
        }

        private decimal GetMonthlyRevenue(List<Order> orderInYear, int month)
        {
            return orderInYear
                .Where(x => x.DatePaid.HasValue
                    && x.DatePaid.Value.Month == month)
                .Select(x => x.TotalPrice)
                .Sum();
        }

        public async Task<YearlyRevenueViewModel> GetYearlyRevenue(int year)
        {
            try
            {
                var orderInYear = await _context.Orders
                    .Where(x => x.DatePaid.HasValue && x.DatePaid.Value.Year == year)
                    .ToListAsync();
                YearlyRevenueViewModel yearlyRevenue = new();
                //{
                //    JanTotal = GetMonthlyRevenue(orderInYear, 1),
                //    FebTotal = GetMonthlyRevenue(orderInYear, 2),
                //    MarTotal = GetMonthlyRevenue(orderInYear, 3),
                //    AprTotal = GetMonthlyRevenue(orderInYear, 4),
                //    MayTotal = GetMonthlyRevenue(orderInYear, 5),
                //    JunTotal = GetMonthlyRevenue(orderInYear, 6),
                //    JulTotal = GetMonthlyRevenue(orderInYear, 7),
                //    AugTotal = GetMonthlyRevenue(orderInYear, 8),
                //    SepTotal = GetMonthlyRevenue(orderInYear, 9),
                //    OctTotal = GetMonthlyRevenue(orderInYear, 10),
                //    NovTotal = GetMonthlyRevenue(orderInYear, 11),
                //    DecTotal = GetMonthlyRevenue(orderInYear, 12)
                //};
                int count = 1;
                foreach (PropertyInfo propertyInfo in yearlyRevenue.GetType().GetProperties())
                {
                    propertyInfo.SetValue(yearlyRevenue, GetMonthlyRevenue(orderInYear, count));
                    count++;
                }
                return yearlyRevenue;
            }
            catch
            {
                throw new Exception("Failed to get statistic");
            }
        }

        private static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        private decimal GeDayOfWeekRevenue(List<Order> orderInWeek, DayOfWeek dayOfWeek)
        {
            return orderInWeek
                .Where(x => x.DatePaid.HasValue
                    && x.DatePaid.Value.DayOfWeek == dayOfWeek)
                .Select(x => x.TotalPrice)
                .Sum();
        }

        public async Task<WeeklyRevenueViewModel> GetWeeklyRevenue(int year, int month, int day)
        {
            DateTime dt = new DateTime(year, month, day);
            DateTime startDateWeek = StartOfWeek(dt, DayOfWeek.Monday);
            try
            {
                var orderInWeek = await _context.Orders
                    .Where(x => x.DatePaid.HasValue
                            && x.DatePaid.Value >= startDateWeek && x.DatePaid < startDateWeek.AddDays(7))
                    .ToListAsync();
                WeeklyRevenueViewModel weeklyRevenue = new WeeklyRevenueViewModel()
                {
                    MonTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Monday),
                    TueTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Tuesday),
                    WedTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Wednesday),
                    ThurTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Thursday),
                    FriTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Friday),
                    SatTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Saturday),
                    SunTotal = GeDayOfWeekRevenue(orderInWeek, DayOfWeek.Sunday)
                };
                return weeklyRevenue;
            }
            catch
            {
                throw new Exception("Failed to get statistic");
            }
        }
    }
}