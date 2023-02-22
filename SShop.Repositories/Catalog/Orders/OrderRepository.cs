using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Utilities.Constants.Orders;
using SShop.ViewModels.Catalog.Orders;
using SShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using SShop.Repositories.Catalog.OrderItems;
using SShop.Services.MailJet;

namespace SShop.Repositories.Catalog.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IOrderItemRepository _orderItemServices;
        private readonly IMailJetServices _mailJetServices;

        public OrderRepository(AppDbContext context, IOrderItemRepository orderItemServices, IMailJetServices mailJetServices)
        {
            _context = context;
            _orderItemServices = orderItemServices;
            _mailJetServices = mailJetServices;
        }

        public async Task<int> Create(OrderCreateRequest request)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var order = new Order()
                {
                    UserId = request.UserId,
                    DiscountId = request.DiscountId ?? null,
                    Shipping = request.Shipping,
                    TotalItemPrice = request.TotalItemPrice,
                    TotalPrice = request.Shipping + request.TotalItemPrice,
                    Address = request.Address,
                    DateCreated = DateTime.Now,
                    Status = request.Status,
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Payment = request.Payment
                };
                if (request.Payment == ORDER_PAYMENT.PAYPAL)
                {
                    order.DateDone = DateTime.Now;
                }
                else
                {
                    order.DateDone = null;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                var user = await _context.Users
                    .Where(x => x.Id == request.UserId)
                    .Include(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();
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
                    product.Quantity -= 1;
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
                    return -1;
                }
                return order.OrderId;
            }
            catch
            {
                return -1;
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

        private static string GenerateOrderStatusClass(int x)
        {
            string s = "";
            switch (x)
            {
                case ORDER_STATUS.PENDING:
                    s = "badge badge-secondary";
                    break;

                case ORDER_STATUS.READY_TO_SHIP:
                    s = "badge badge-warning";
                    break;

                case ORDER_STATUS.ON_THE_WAY:
                    s = "badge badge-info";
                    break;

                case ORDER_STATUS.DELIVERED:
                    s = "badge badge-success";
                    break;

                case ORDER_STATUS.CANCELED:
                    s = "badge badge-danger";
                    break;

                case ORDER_STATUS.RETURNED:
                    s = "badge badge-dark";
                    break;

                default:
                    s = "";
                    break;
            }
            return s;
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
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.Address.Contains(request.Keyword))
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new OrderViewModel()
                    {
                        OrderId = x.OrderId,
                        UserId = x.UserId,
                        UserFullName = x.User.FirstName + x.User.LastName,
                        UserAddress = x.User.Address,
                        UserPhone = x.User.PhoneNumber,
                        DiscountId = x?.DiscountId,
                        DiscountCode = x.Discount?.DiscountCode,
                        DiscountValue = x.Discount?.DiscountValue,
                        Shipping = x.Shipping,
                        TotalItemPrice = x.TotalItemPrice,
                        TotalPrice = x.TotalPrice,
                        Address = x.Address,
                        DateCreated = x.DateCreated,
                        DateDone = x.DateDone,
                        Status = x.Status,
                        StatusClass = GenerateOrderStatusClass(x.Status),
                        Name = x.Name,
                        Email = x.Email,
                        Phone = x.Phone,
                        Payment = x.Payment,
                        PaymentMethod = ORDER_PAYMENT.OrderPayment[x.Payment],
                        StatusCode = ORDER_STATUS.OrderStatus[x.Status],
                        TotalItem = x.OrderItems.Count
                    }).ToList();
                foreach (var d in data)
                {
                    d.OrderItems = await _orderItemServices.RetrieveByOrderId(d.OrderId);
                }
                return new PagedResult<OrderViewModel>
                {
                    TotalItem = query.Count,
                    Items = data
                };
            }
            catch
            {
                return null;
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
                    .FirstOrDefaultAsync();
                if (order == null)
                    return null;
                return new OrderViewModel()
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    UserFullName = order.User.UserName,
                    UserAddress = order.User.Address,
                    UserPhone = order.User.PhoneNumber,
                    DiscountId = order?.DiscountId,
                    DiscountCode = order.Discount?.DiscountCode,
                    DiscountValue = order.Discount?.DiscountValue,
                    Shipping = order.Shipping,
                    TotalItemPrice = order.TotalItemPrice,
                    TotalPrice = order.TotalPrice,
                    Address = order.Address,
                    DateCreated = order.DateCreated,
                    DateDone = order.DateDone,
                    Status = order.Status,
                    StatusClass = GenerateOrderStatusClass(order.Status),
                    Name = order.Name,
                    Email = order.Email,
                    Phone = order.Phone,
                    Payment = order.Payment,
                    PaymentMethod = ORDER_PAYMENT.OrderPayment[order.Payment],
                    StatusCode = ORDER_STATUS.OrderStatus[order.Status],
                    TotalItem = order.OrderItems.Count,
                    OrderItems = await _orderItemServices.RetrieveByOrderId(order.OrderId)
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> Update(OrderUpdateRequest request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(request.OrderId);
                if (order == null)
                    return -1;
                if (order.Status == ORDER_STATUS.DELIVERED && request.Status != ORDER_STATUS.RETURNED)
                    return -1;
                order.Status = request.Status;
                if (request.Status == ORDER_STATUS.DELIVERED && order.Payment == ORDER_PAYMENT.COD)
                {
                    order.Payment = ORDER_PAYMENT.PAYPAL;
                    order.DateDone = DateTime.Now;
                }

                _context.Orders.Update(order);

                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }

        public async Task<OrderOverviewViewModel> GetOverviewStatictis()
        {
            var orders = await _context.Orders.Select(x => x.Status).ToListAsync();

            OrderOverviewViewModel orderOverview = new OrderOverviewViewModel()
            {
                TotalPending = orders.Where(x => x == ORDER_STATUS.PENDING).Count(),
                TotalReturned = orders.Where(x => x == ORDER_STATUS.RETURNED).Count(),
                TotalCanceled = orders.Where(x => x == ORDER_STATUS.CANCELED).Count(),
                TotalReady = orders.Where(x => x == ORDER_STATUS.READY_TO_SHIP).Count(),
                TotalCompleted = orders.Where(x => x == ORDER_STATUS.DELIVERED).Count(),
                TotalDelivering = orders.Where(x => x == ORDER_STATUS.ON_THE_WAY).Count()
            };

            return orderOverview;
        }
    }
}