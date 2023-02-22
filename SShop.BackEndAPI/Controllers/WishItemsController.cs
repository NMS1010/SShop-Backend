using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.WishItems;
using SShop.ViewModels.Catalog.Wishtems;
using SShop.ViewModels.Common;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Customer")]
    public class WishItemsController : ControllerBase
    {
        private readonly IWishItemRepository _wishItemServices;

        public WishItemsController(IWishItemRepository wishItemServices)
        {
            _wishItemServices = wishItemServices;
        }

        [HttpPost("all")]
        public async Task<IActionResult> RetrieveAll([FromForm] string userId)
        {
            var wishItems = await _wishItemServices.RetrieveWishByUserId(userId);

            if (wishItems == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot get user's wish list"));
            return Ok(CustomAPIResponse<PagedResult<WishItemViewModel>>.Success(wishItems, StatusCodes.Status200OK));
        }

        [HttpGet("{wishItemId}")]
        public async Task<IActionResult> RetrieveById(int wishItemId)
        {
            var wishItem = await _wishItemServices.RetrieveById(wishItemId);

            if (wishItem == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot get this wish item"));
            return Ok(CustomAPIResponse<WishItemViewModel>.Success(wishItem, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] WishItemCreateRequest request)
        {
            var statusRes = await _wishItemServices.AddProductToWish(request);

            return Ok(CustomAPIResponse<string>.Success(statusRes, StatusCodes.Status200OK));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] WishItemUpdateRequest request)
        {
            var count = await _wishItemServices.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot update this wish item"));
            return Ok(CustomAPIResponse<WishItemViewModel>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{wishItemId}")]
        public async Task<IActionResult> Delete(int wishItemId)
        {
            var count = await _wishItemServices.Delete(wishItemId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot delete this wish item"));
            return Ok(CustomAPIResponse<WishItemViewModel>.Success(StatusCodes.Status200OK));
        }
    }
}