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
    public class WishsController : ControllerBase
    {
        private readonly IWishItemRepository _wishItemRepository;

        public WishsController(IWishItemRepository wishItemRepository)
        {
            _wishItemRepository = wishItemRepository;
        }

        [HttpPost("all")]
        public async Task<IActionResult> RetrieveAll([FromForm] string userId)
        {
            var wishItems = await _wishItemRepository.RetrieveWishByUserId(userId);

            if (wishItems == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot get user's wish list"));
            return Ok(CustomAPIResponse<PagedResult<WishItemViewModel>>.Success(wishItems, StatusCodes.Status200OK));
        }

        [HttpGet("{wishItemId}")]
        public async Task<IActionResult> RetrieveById(int wishItemId)
        {
            var wishItem = await _wishItemRepository.RetrieveById(wishItemId);

            if (wishItem == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot get this wish item"));
            return Ok(CustomAPIResponse<WishItemViewModel>.Success(wishItem, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] WishItemCreateRequest request)
        {
            var res = await _wishItemRepository.AddProductToWish(request);

            return Ok(CustomAPIResponse<object>.Success(res, StatusCodes.Status200OK));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] WishItemUpdateRequest request)
        {
            var count = await _wishItemRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot update this wish item"));
            return Ok(CustomAPIResponse<WishItemViewModel>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{wishItemId}")]
        public async Task<IActionResult> Delete(int wishItemId)
        {
            var currentWishAmount = await _wishItemRepository.Delete(wishItemId);

            return Ok(CustomAPIResponse<object>.Success(new { CurrentWishAmount = currentWishAmount }, StatusCodes.Status200OK));
        }

        [HttpDelete("delete/all/{userId}")]
        public async Task<IActionResult> DeleteAll(string userId)
        {
            var count = await _wishItemRepository.DeleteAll(userId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "cannot delete this wish item"));
            return Ok(CustomAPIResponse<WishItemViewModel>.Success(StatusCodes.Status200OK));
        }
    }
}