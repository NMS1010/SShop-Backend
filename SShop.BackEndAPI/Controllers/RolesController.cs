using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.System.Roles;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Roles;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleServices;

        public RolesController(IRoleRepository roleServices)
        {
            _roleServices = roleServices;
        }

        [HttpGet("all")]
        public async Task<IActionResult> RetrieveAll([FromQuery] RoleGetPagingRequest request)
        {
            var roles = await _roleServices.RetrieveAll(request);
            if (roles == null)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot get role list"));
            return Ok(CustomAPIResponse<PagedResult<RoleViewModel>>.Success(roles, StatusCodes.Status200OK));
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> RetrieveById(string roleId)
        {
            var role = await _roleServices.RetrieveById(roleId);

            if (role == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot found this role"));
            return Ok(CustomAPIResponse<RoleViewModel>.Success(role, StatusCodes.Status200OK));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromForm] RoleCreateRequest request)
        {
            var roleId = await _roleServices.Create(request);

            if (roleId <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot create this role"));

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] RoleUpdateRequest request)
        {
            var count = await _roleServices.Update(request);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot update this role"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> Delete(string roleId)
        {
            var count = await _roleServices.Delete(roleId);

            if (count <= 0)
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this role"));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}