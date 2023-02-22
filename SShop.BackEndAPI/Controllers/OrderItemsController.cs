using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.OrderItems;
using SShop.ViewModels.Catalog.OrderItems;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemService;

        public OrderItemsController(IOrderItemRepository orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveAll([FromQuery] OrderItemGetPagingRequest request)
        {
            var categories = await _orderItemService.RetrieveAll(request);
            if (categories == null)
                return BadRequest();
            return Ok(categories);
        }

        [HttpGet("{orderItemId}")]
        public async Task<IActionResult> RetrieveById(int orderItemId)
        {
            var orderItem = await _orderItemService.RetrieveById(orderItemId);
            if (orderItem == null)
                return BadRequest();
            return Ok(orderItem);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] OrderItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var orderItemId = await _orderItemService.Create(request);
            if (orderItemId <= 0)
                return BadRequest();
            var orderItem = await _orderItemService.RetrieveById(orderItemId);

            return CreatedAtAction(nameof(RetrieveById), new { orderItemId }, orderItem);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] OrderItemUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var count = await _orderItemService.Update(request);
            if (count <= 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("delete/{orderItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int orderItemId)
        {
            int records = await _orderItemService.Delete(orderItemId);
            if (records <= 0)
                return BadRequest();
            return Ok();
        }
    }
}