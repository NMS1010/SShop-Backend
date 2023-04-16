using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.ProductImages;
using SShop.Repositories.Catalog.Products;
using SShop.ViewModels.Catalog.ProductImages;
using SShop.ViewModels.Catalog.Products;
using SShop.ViewModels.Common;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;

        public ProductsController(IProductRepository productRepository, IProductImageRepository productImageRepository)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveAllPaging([FromQuery] ProductGetPagingRequest request)
        {
            var domainName = HttpContext.Request.GetDisplayUrl();
            var products = await _productRepository.RetrieveAll(request);
            if (products == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get product list"));
            return Ok(CustomAPIResponse<PagedResult<ProductViewModel>>.Success(products, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int productId = await _productRepository.Create(request);
            if (productId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this product"));

            //var product = await _productRepository.RetrieveById(productId);
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveById(int productId)
        {
            var product = await _productRepository.RetrieveById(productId);
            if (product == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this product"));
            return Ok(CustomAPIResponse<ProductViewModel>.Success(product, StatusCodes.Status200OK));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int records = await _productRepository.Update(request);
            if (records <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this product"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int productId)
        {
            int records = await _productRepository.Delete(productId);
            if (records <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this product"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        ////Product Images
        [HttpGet("{productId}/images/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveImageByProductId(int productId)
        {
            var productImages = await _productImageRepository.RetrieveAll(new ProductImageGetPagingRequest() { ProductId = productId });
            if (productImages == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot get images of this product "));
            return Ok(CustomAPIResponse<PagedResult<ProductImageViewModel>>.Success(productImages, StatusCodes.Status200OK));
        }

        [HttpGet("images/{productImageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveImageById(int productImageId)
        {
            var productImage = await _productImageRepository.RetrieveById(productImageId);
            if (productImage == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this product image"));
            return Ok(CustomAPIResponse<ProductImageViewModel>.Success(productImage, StatusCodes.Status200OK));
        }

        [HttpPost("images/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateImages([FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int count = await _productImageRepository.Create(request);
            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create images for this product"));
            var pagingRequest = new ProductImageGetPagingRequest() { ProductId = request.ProductId };
            var productImages = await _productImageRepository.RetrieveAll(pagingRequest);
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPost("image/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSingleImage([FromForm] ProductImageCreateSingleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int productImgId = await _productImageRepository.CreateSingleImage(request);
            if (productImgId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create sub image for this product"));
            var productImage = await _productImageRepository.RetrieveById(productImgId);
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("images/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateImage([FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int records = await _productImageRepository.Update(request);
            if (records <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this product image"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("images/delete/{imageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int records = await _productImageRepository.Delete(imageId);
            if (records == 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this product image"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}