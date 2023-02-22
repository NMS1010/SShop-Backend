using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SShop.Repositories.System.Users;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Users;

namespace SShop.BackEndAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Customer")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userService;

        public UsersController(IUserRepository userService)
        {
            _userService = userService;
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resToken = await _userService.Authenticate(request);
            if (string.IsNullOrEmpty(resToken))
            {
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, "Username/password is incorrect"));
            }
            if (resToken == "banned")
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, "Your account has been banned"));
            if (resToken == "unconfirm")
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, "Your account hasn't been confirmed"));
            return Ok(CustomAPIResponse<string>.Success(resToken, StatusCodes.Status200OK));
        }

        [Route("google-login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromForm] string email, [FromForm] string loginProvider, [FromForm] string providerKey)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resToken = await _userService.AuthenticateWithGoogle(email, loginProvider, providerKey);
            if (string.IsNullOrEmpty(resToken))
            {
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "error"));
            }
            return Ok(CustomAPIResponse<string>.Success(resToken, StatusCodes.Status200OK));
        }

        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            (var res, var status) = await _userService.Register(request);
            if (!res)
            {
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, status));
            }
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveAll([FromQuery] UserGetPagingRequest request)
        {
            var res = await _userService.RetrieveAll(request);
            if (res.Items?.Count == 0)
                return NotFound(CustomAPIResponse<PagedResult<NoContentAPIResponse>>.Fail(StatusCodes.Status404NotFound, "Cannot get user list"));
            return Ok(CustomAPIResponse<PagedResult<UserViewModel>>.Success(res, StatusCodes.Status200OK));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> RetrieveById(string userId)
        {
            var res = await _userService.RetrieveById(userId);
            if (res == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot find this user"));
            return Ok(CustomAPIResponse<UserViewModel>.Success(res, StatusCodes.Status200OK));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequest request)
        {
            (var res, var status) = await _userService.Update(request);
            if (!res)
            {
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, status));
            }
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpDelete("delete/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string userId)
        {
            var count = await _userService.Delete(userId);
            if (count <= 0)
            {
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "Cannot delete this user"));
            }
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpGet("register-confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirm([FromQuery] string email, [FromQuery] string token)
        {
            var res = await _userService.VerifyToken(email, token);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("confirm-error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("confirm-success", StatusCodes.Status200OK));
        }

        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            var res = await _userService.CheckEmail(email);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("success", StatusCodes.Status200OK));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromForm] string email, [FromForm] string host)
        {
            var res = await _userService.ForgotPassword(email, host);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("success", StatusCodes.Status200OK));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromForm] string email, [FromForm] string token, [FromForm] string password)
        {
            var res = await _userService.VerifyForgotPasswordToken(email, token, password);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("success", StatusCodes.Status200OK));
        }

        [HttpPost("check-add")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckNewUser(UserCheckNewRequest request)
        {
            var res = await _userService.CheckNewUser(request);
            if (res.Count > 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, res));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpPost("check-edit")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEditUser(UserCheckEditRequest request)
        {
            var res = await _userService.CheckEditUser(request);
            if (res.Count > 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, res));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}