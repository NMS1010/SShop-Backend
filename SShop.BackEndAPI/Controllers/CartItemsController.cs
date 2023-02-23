using SShop.ViewModels.Catalog.CartItems;
using SShop.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.CartItems;
using System.Threading.Tasks;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer,Admin")]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemsController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        [HttpPost("all")]
        public async Task<IActionResult> RetrieveAll([FromForm] CartItemGetPagingRequest request)
        {
            var cartItems = await _cartItemRepository.RetrieveCartByUserId(request.UserId);
            if (cartItems == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get cart item list"));
            return Ok(CustomAPIResponse<PagedResult<CartItemViewModel>>.Success(cartItems, StatusCodes.Status200OK));
        }

        [HttpGet("{cartItemId}")]
        public async Task<IActionResult> RetrieveById(int cartItemId)
        {
            var cartItem = await _cartItemRepository.RetrieveById(cartItemId);
            if (cartItem == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get cart item"));
            return Ok(CustomAPIResponse<CartItemViewModel>.Success(cartItem, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromForm] CartItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var responseStatus = await _cartItemRepository.AddProductToCart(request);

            return Ok(CustomAPIResponse<string>.Success(responseStatus, StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] CartItemUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var count = await _cartItemRepository.Update(request);
            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update quantity"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{cartItemId}")]
        public async Task<IActionResult> Delete(int cartItemId)
        {
            int records = await _cartItemRepository.Delete(cartItemId);
            if (records <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this product from cart"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}