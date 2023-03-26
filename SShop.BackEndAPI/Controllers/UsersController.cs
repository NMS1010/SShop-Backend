using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
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
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resToken = await _userRepository.Authenticate(request);
            return Ok(CustomAPIResponse<TokenViewModel>.Success(resToken, StatusCodes.Status200OK));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromForm] TokenViewModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resToken = await _userRepository.RefreshToken(request);
            return Ok(CustomAPIResponse<TokenViewModel>.Success(resToken, StatusCodes.Status200OK));
        }

        [HttpPost("revoke-token/{userId}")]
        public async Task<IActionResult> RevokeToken(string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _userRepository.RevokeToken(userId);
            return Ok(CustomAPIResponse<string>.Success("Revoke token for this user successfully", StatusCodes.Status200OK));
        }

        [HttpPost("revoke-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevokeAllToken()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _userRepository.RevokeAllToken();
            return Ok(CustomAPIResponse<string>.Success("Revoke token for all user successfully", StatusCodes.Status200OK));
        }

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromForm] string email, [FromForm] string loginProvider, [FromForm] string providerKey)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resToken = await _userRepository.AuthenticateWithGoogle(email, loginProvider, providerKey);
            if (string.IsNullOrEmpty(resToken))
            {
                return BadRequest(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status400BadRequest, "error"));
            }
            return Ok(CustomAPIResponse<string>.Success(resToken, StatusCodes.Status200OK));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _userRepository.Register(request);

            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status201Created));
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrieveAll([FromQuery] UserGetPagingRequest request)
        {
            var res = await _userRepository.RetrieveAll(request);
            if (res.Items?.Count == 0)
                return NotFound(CustomAPIResponse<PagedResult<NoContentAPIResponse>>.Fail(StatusCodes.Status404NotFound, "Cannot get user list"));
            return Ok(CustomAPIResponse<PagedResult<UserViewModel>>.Success(res, StatusCodes.Status200OK));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> RetrieveById(string userId)
        {
            var res = await _userRepository.RetrieveById(userId);
            if (res == null)
                return NotFound(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status404NotFound, "Cannot find this user"));
            return Ok(CustomAPIResponse<UserViewModel>.Success(res, StatusCodes.Status200OK));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequest request)
        {
            (var res, var status) = await _userRepository.Update(request);
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
            var count = await _userRepository.Delete(userId);
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
            var res = await _userRepository.VerifyToken(email, token);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("confirm-error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("confirm-success", StatusCodes.Status200OK));
        }

        [HttpGet("check-email")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail([FromQuery] string email)
        {
            var res = await _userRepository.CheckEmail(email);
            if (!res)
                return Ok(CustomAPIResponse<bool>.Success(false, StatusCodes.Status200OK));
            return Ok(CustomAPIResponse<bool>.Success(true, StatusCodes.Status200OK));
        }

        [HttpGet("check-username")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUsername([FromQuery] string username)
        {
            var res = await _userRepository.CheckUsername(username);
            if (!res)
                return Ok(CustomAPIResponse<bool>.Success(false, StatusCodes.Status200OK));
            return Ok(CustomAPIResponse<bool>.Success(true, StatusCodes.Status200OK));
        }

        [HttpGet("check-phone")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckPhone([FromQuery] string phone)
        {
            var res = await _userRepository.CheckPhone(phone);
            if (!res)
                return Ok(CustomAPIResponse<bool>.Success(false, StatusCodes.Status200OK));
            return Ok(CustomAPIResponse<bool>.Success(true, StatusCodes.Status200OK));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromForm] string email, [FromForm] string host)
        {
            var res = await _userRepository.ForgotPassword(email, host);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("success", StatusCodes.Status200OK));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromForm] string email, [FromForm] string token, [FromForm] string password)
        {
            var res = await _userRepository.VerifyForgotPasswordToken(email, token, password);
            if (!res)
                return Ok(CustomAPIResponse<string>.Success("error", StatusCodes.Status400BadRequest));
            return Ok(CustomAPIResponse<string>.Success("success", StatusCodes.Status200OK));
        }

        [HttpPost("check-add")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckNewUser(UserCheckNewRequest request)
        {
            var res = await _userRepository.CheckNewUser(request);
            if (res.Count > 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, res));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }

        [HttpPost("check-edit")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEditUser(UserCheckEditRequest request)
        {
            var res = await _userRepository.CheckEditUser(request);
            if (res.Count > 0)
                return Ok(CustomAPIResponse<NoContentAPIResponse>.Fail(StatusCodes.Status200OK, res));
            return Ok(CustomAPIResponse<NoContentAPIResponse>.Success(StatusCodes.Status200OK));
        }
    }
}