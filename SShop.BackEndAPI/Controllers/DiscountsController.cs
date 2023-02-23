using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.Discounts;
using SShop.ViewModels.Catalog.Discounts;
using SShop.ViewModels.Common;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountsController(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveAll([FromQuery] DiscountGetPagingRequest request)
        {
            var discounts = await _discountRepository.RetrieveAll(request);

            if (discounts == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get discount list"));
            return Ok(CustomAPIResponse<PagedResult<DiscountViewModel>>.Success(discounts, StatusCodes.Status200OK));
        }

        [HttpGet("{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveById(int discountId)
        {
            var discount = await _discountRepository.RetrieveById(discountId);

            if (discount == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this discount"));
            return Ok(CustomAPIResponse<DiscountViewModel>.Success(discount, StatusCodes.Status200OK));
        }

        [HttpGet("apply/{discountCode}")]
        [Authorize]
        public async Task<IActionResult> ApplyDiscount(string discountCode)
        {
            var state = await _discountRepository.ApllyDiscount(discountCode);
            return Ok(CustomAPIResponse<string>.Success(state, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] DiscountCreateRequest request)
        {
            var discountId = await _discountRepository.Create(request);

            if (discountId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this discount"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] DiscountUpdateRequest request)
        {
            var count = await _discountRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this discount"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{discountId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int discountId)
        {
            var count = await _discountRepository.Delete(discountId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this discount"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}