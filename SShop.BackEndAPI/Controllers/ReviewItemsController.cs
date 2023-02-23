using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.ReviewItems;
using SShop.ViewModels.Catalog.ReviewItems;
using SShop.ViewModels.Common;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewItemsController : ControllerBase
    {
        private readonly IReviewItemRepository _reviewRepository;

        public ReviewItemsController(IReviewItemRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveAll([FromQuery] ReviewItemGetPagingRequest request)
        {
            var reviews = await _reviewRepository.RetrieveAll(request);

            if (reviews == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get review list"));
            return Ok(CustomAPIResponse<PagedResult<ReviewItemViewModel>>.Success(reviews, StatusCodes.Status200OK));
        }

        [HttpPut("status/change/{reviewItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int reviewItemId)
        {
            var count = await _reviewRepository.ChangeReviewStatus(reviewItemId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot change this review status"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpGet("{reviewItemId}")]
        public async Task<IActionResult> RetrieveById(int reviewItemId)
        {
            var review = await _reviewRepository.RetrieveById(reviewItemId);

            if (review == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this review"));
            return Ok(CustomAPIResponse<ReviewItemViewModel>.Success(review, StatusCodes.Status200OK));
        }

        [HttpPost("get-by-user")]
        public async Task<IActionResult> RetrieveReviewsByUser([FromForm] string userId, [FromForm] int productId)
        {
            var reviews = await _reviewRepository.RetrieveReviewsByUser(userId, productId);

            if (reviews == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found reviews for this productId"));
            return Ok(CustomAPIResponse<PagedResult<ReviewItemViewModel>>.Success(reviews, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] ReviewItemCreateRequest request)
        {
            var reviewId = await _reviewRepository.Create(request);

            if (reviewId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this review"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] ReviewItemUpdateRequest request)
        {
            var count = await _reviewRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this review"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{reviewItemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int reviewItemId)
        {
            var count = await _reviewRepository.ChangeReviewStatus(reviewItemId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this review"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}