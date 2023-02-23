using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.Orders;
using SShop.ViewModels.Catalog.Orders;
using SShop.ViewModels.Common;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveAll([FromQuery] OrderGetPagingRequest request)
        {
            var orders = await _orderRepository.RetrieveAll(request);

            if (orders == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get order list"));
            return Ok(CustomAPIResponse<PagedResult<OrderViewModel>>.Success(orders, StatusCodes.Status200OK));
        }

        [HttpGet("statictis")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveOrderOverviewStatictis()
        {
            var statictis = await _orderRepository.GetOverviewStatictis();

            if (statictis == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get orders statictis overview"));
            return Ok(CustomAPIResponse<OrderOverviewViewModel>.Success(statictis, StatusCodes.Status200OK));
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> RetrieveById(int orderId)
        {
            var order = await _orderRepository.RetrieveById(orderId);

            if (order == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this order"));
            return Ok(CustomAPIResponse<OrderViewModel>.Success(order, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] OrderCreateRequest request)
        {
            var orderId = await _orderRepository.Create(request);

            if (orderId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this order"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] OrderUpdateRequest request)
        {
            var count = await _orderRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this order"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}