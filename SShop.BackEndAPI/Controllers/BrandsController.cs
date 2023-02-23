using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.Brands;
using SShop.ViewModels.Catalog.Brands;
using SShop.ViewModels.Common;
using System.Threading.Tasks;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;

        public BrandsController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveAll([FromQuery] BrandGetPagingRequest request)
        {
            var brands = await _brandRepository.RetrieveAll(request);
            if (brands == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get brand list"));
            return Ok(CustomAPIResponse<PagedResult<BrandViewModel>>.Success(brands, StatusCodes.Status200OK));
        }

        [HttpGet("{brandId}")]
        public async Task<IActionResult> RetrieveById(int brandId)
        {
            var brand = await _brandRepository.RetrieveById(brandId);

            if (brand == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this brand"));
            return Ok(CustomAPIResponse<BrandViewModel>.Success(brand, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] BrandCreateRequest request)
        {
            var brandId = await _brandRepository.Create(request);

            if (brandId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this brand"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] BrandUpdateRequest request)
        {
            var count = await _brandRepository.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this brand"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{brandId}")]
        public async Task<IActionResult> Delete(int brandId)
        {
            var count = await _brandRepository.Delete(brandId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this brand"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}