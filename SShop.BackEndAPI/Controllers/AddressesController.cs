using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.System.Addresses;
using SShop.Utilities.Constants.Systems;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Addresses;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer, Admin")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;

        public AddressesController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet("all")]
        public async Task<IActionResult> RetrieveAll([FromQuery] AddressGetPagingRequest request)
        {
            var addresses = await _addressRepository.RetrieveAll(request);
            if (addresses == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get address list"));
            return Ok(CustomAPIResponse<PagedResult<AddressViewModel>>.Success(addresses, StatusCodes.Status200OK));
        }

        [HttpGet("{addressId}")]
        public async Task<IActionResult> RetrieveById(int addressId)
        {
            var address = await _addressRepository.RetrieveById(addressId);

            if (address == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this address"));
            return Ok(CustomAPIResponse<AddressViewModel>.Success(address, StatusCodes.Status200OK));
        }

        [HttpGet("address/{userId}")]
        public async Task<IActionResult> RetrieveByUserId(string userId)
        {
            var addresses = await _addressRepository.GetAddressByUserId(userId);

            if (addresses == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found address for this user"));
            return Ok(CustomAPIResponse<PagedResult<AddressViewModel>>.Success(addresses, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] AddressCreateRequest request)
        {
            var addressId = await _addressRepository.Create(request);

            if (addressId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this address"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] AddressUpdateRequest request)
        {
            var count = await _addressRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this address"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{addressId}")]
        public async Task<IActionResult> Delete(int addressId)
        {
            var count = await _addressRepository.Delete(addressId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this address"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}