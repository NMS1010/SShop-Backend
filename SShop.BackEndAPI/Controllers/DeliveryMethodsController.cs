using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.DeliveryMethod;
using SShop.ViewModels.Catalog.DeliveryMethod;
using SShop.ViewModels.Common;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DeliveryMethodsController : ControllerBase
    {
        private readonly IDeliveryMethodRepository _deliveryMethodRepository;

        public DeliveryMethodsController(IDeliveryMethodRepository deliveryMethodRepository)
        {
            _deliveryMethodRepository = deliveryMethodRepository;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveAll([FromQuery] DeliveryMethodGetPagingRequest request)
        {
            var deliveryMethods = await _deliveryMethodRepository.RetrieveAll(request);
            if (deliveryMethods == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get delivery method list"));
            return Ok(CustomAPIResponse<PagedResult<DeliveryMethodViewModel>>.Success(deliveryMethods, StatusCodes.Status200OK));
        }

        [HttpGet("{deliveryMethodId}")]
        public async Task<IActionResult> RetrieveById(int deliveryMethodId)
        {
            var deliveryMethod = await _deliveryMethodRepository.RetrieveById(deliveryMethodId);

            if (deliveryMethod == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this delivery method"));
            return Ok(CustomAPIResponse<DeliveryMethodViewModel>.Success(deliveryMethod, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] DeliveryMethodCreateRequest request)
        {
            var deliveryMethodId = await _deliveryMethodRepository.Create(request);

            if (deliveryMethodId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this delivery method"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] DeliveryMethodUpdateRequest request)
        {
            var count = await _deliveryMethodRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this delivery method"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{deliveryMethodId}")]
        public async Task<IActionResult> Delete(int deliveryMethodId)
        {
            var count = await _deliveryMethodRepository.Delete(deliveryMethodId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this delivery method"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}