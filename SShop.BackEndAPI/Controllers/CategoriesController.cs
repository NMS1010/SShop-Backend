using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.Catalog.Categories;
using SShop.ViewModels.Catalog.Categories;
using SShop.ViewModels.Common;
using System.Threading.Tasks;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveAll([FromQuery] CategoryGetPagingRequest request)
        {
            var categories = await _categoryRepository.RetrieveAll(request);
            if (categories == null)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get categories list"));
            return Ok(CustomAPIResponse<PagedResult<CategoryViewModel>>.Success(categories, StatusCodes.Status200OK));
        }

        [HttpGet("all/parent-categories")]
        [AllowAnonymous]
        public async Task<IActionResult> RetrieveParentCategories()
        {
            var categories = await _categoryRepository.GetParentCategory();
            if (categories == null)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get parent categories list"));
            return Ok(CustomAPIResponse<PagedResult<CategoryViewModel>>.Success(categories, StatusCodes.Status200OK));
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> RetrieveById(int categoryId)
        {
            var category = await _categoryRepository.RetrieveById(categoryId);
            if (category == null)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot find this caterogy"));
            return Ok(CustomAPIResponse<CategoryViewModel>.Success(category, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return Ok(ModelState);
            var categoryId = await _categoryRepository.Create(request);
            if (categoryId <= 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this caterogy"));
            //var category = await _categoryRepository.RetrieveById(categoryId);

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return Ok(ModelState);
            var count = await _categoryRepository.Update(request);
            if (count <= 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this caterogy"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{categoryId}")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            int records = await _categoryRepository.Delete(categoryId);
            if (records <= 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this caterogy"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}