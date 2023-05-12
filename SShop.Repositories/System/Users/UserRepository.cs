using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.Repositories.Catalog.Orders;
using SShop.Utilities.Constants.Users;
using SShop.ViewModels.Catalog.Orders;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Roles;
using SShop.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SShop.Services.FileStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SShop.Services.MailJet;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Logging;
using SShop.Utilities.Constants.Systems;
using PayPal.Api;
using SShop.ViewModels.System.Addresses;
using System.ComponentModel.DataAnnotations;

namespace SShop.Repositories.System.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorage;
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepositories;
        private readonly IMailJetServices _mailJetServices;

        public UserRepository(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IFileStorageService fileStorage,
            RoleManager<IdentityRole> roleManager, AppDbContext context, IOrderRepository orderRepositories,
            IMailJetServices mailJetServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _roleManager = roleManager;
            _context = context;
            _orderRepositories = orderRepositories;
            _mailJetServices = mailJetServices;
        }

        private async Task<string> WriteJWT(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName + user.LastName),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                _configuration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal ValidateExpiredJWT(string jwt)
        {
            IdentityModelEventSource.ShowPII = true;

            TokenValidationParameters validationParameters = new()
            {
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration["Tokens:Issuer"],
                ValidIssuer = _configuration["Tokens:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]))
            };

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);
            if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;
            return principal;
        }

        public async Task<TokenViewModel> RefreshToken(TokenViewModel request)
        {
            var userPrincipal = ValidateExpiredJWT(request.AccessToken);
            if (userPrincipal is null)
            {
                throw new SecurityTokenException("Invalid access token");
            }
            var userName = userPrincipal.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiredTime <= DateTime.Now)
            {
                throw new SecurityTokenException("Invalid access token or refresh token");
            }
            var newAccessToken = await WriteJWT(user);
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new TokenViewModel { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }

        public async Task<TokenViewModel> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                throw new KeyNotFoundException("Username/password is incorrect");
            var res = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: true);
            if (res.IsLockedOut)
            {
                throw new AccessViolationException("Your account has been lockout, unlock in " + user.LockoutEnd);
            }
            if (!res.Succeeded)
                throw new KeyNotFoundException("Username/password is incorrect");
            if (user.Status == USER_STATUS.IN_ACTIVE)
                throw new AccessViolationException("Your account has been banned");
            if (!user.EmailConfirmed)
                throw new AccessViolationException("Your account hasn't been confirmed");

            string accessToken = await WriteJWT(user);
            string refreshToken = GenerateRefreshToken();
            DateTime refreshTokenExpiredTime = DateTime.Now.AddDays(7);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiredTime = refreshTokenExpiredTime;
            var isSuccess = await _userManager.UpdateAsync(user);
            if (!isSuccess.Succeeded)
                throw new Exception("Cannot login, please contact administrator");
            return new TokenViewModel { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var user = new AppUser()
                {
                    DateOfBirth = request.Dob,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    PhoneNumber = request.PhoneNumber,
                    Gender = request.Gender,
                    Status = USER_STATUS.ACTIVE,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Avatar = await _fileStorage.SaveFile(request.Avatar)
                };
                var res = await _userManager.CreateAsync(user, request.Password);

                if (res.Succeeded)
                {
                    List<string> roles = new()
                    {
                        SystemConstants.UserRoles.CUSTOMER_ROLE
                    };
                    await _userManager.AddToRolesAsync(user, roles);
                    if (!string.IsNullOrEmpty(request.LoginProvider))
                    {
                        await _userManager.AddLoginAsync(user, new UserLoginInfo(request.LoginProvider, request.ProviderKey, request.LoginProvider));
                    }
                    if (!string.IsNullOrEmpty(request.Host))
                    {
                        bool isSend = await SendConfirmToken(user, request.Host);
                        if (!isSend)
                        {
                            await transaction.RollbackAsync();
                            throw new Exception("Cannot send mail");
                        }
                    }
                    await transaction.CommitAsync();
                    return true;
                }

                string error = "";
                res.Errors.ToList().ForEach(x => error += (x.Description + "/n"));
                await transaction.RollbackAsync();
                throw new Exception(error);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
        }

        public async Task<bool> SendConfirmToken(AppUser user, string host)
        {
            string confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            confirmToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));
            string confirmUrl = host + $"/register-confirm?token={confirmToken}&email={user.Email}";
            string content = "<h2>Chào " + user.FirstName + " " + user.LastName + ", </h2><h3>Link xác nhận đăng ký cho tài khoản <em>" + user.UserName + "</em>: <a href = \"" + confirmUrl + "\">Confirm account</a> </h3>" + "<h4>Bạn vui lòng xác nhận ngay khi thấy tin nhắn này. Xin cảm ơn!!!</h4>";
            string title = "Xác nhận đăng ký tài khoản";
            bool isSend = await _mailJetServices.SendMail($"{user.FirstName} {user.LastName}", user.Email, content, title);
            return isSend;
        }

        public async Task<(bool, string)> Update(UserUpdateRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    return (false, "not found");
                string avatar = user.Avatar;

                user.FirstName = request.FirstName;
                user.DateOfBirth = request.Dob;
                user.Email = request.Email;
                user.LastName = request.LastName;
                user.UserName = request.UserName;
                user.PhoneNumber = request.PhoneNumber;
                user.Gender = request.Gender;
                user.Status = request.Status;
                user.DateUpdated = DateTime.Now;
                if (request.ConfirmPassword != null)
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.ConfirmPassword);
                if (request.Avatar != null)
                {
                    user.Avatar = await _fileStorage.SaveFile(request.Avatar);
                }
                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    if (request.Avatar != null)
                        await _fileStorage.DeleteFile(avatar);
                    if (request.Roles == null || request.Roles[0] == "null")
                        return (true, "successfull");
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    List<string> roles = new List<string>();
                    foreach (var roleId in JsonConvert.DeserializeObject<string[]>(request.Roles[0]))
                    {
                        var role = await _context.Roles.FindAsync(roleId);
                        if (role == null)
                        {
                            role = await _context.Roles.Where(x => x.Name.ToLower() == roleId.ToLower()).FirstOrDefaultAsync();
                        }
                        roles.Add(role.Name);
                    }
                    await _userManager.AddToRolesAsync(user, roles);

                    return (true, "successfull");
                }
                string error = "";
                res.Errors.ToList().ForEach(x => error += (x.Description + "/n"));
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public UserViewModel GetUserViewModel(AppUser user)
        {
            return new UserViewModel()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                UserName = user.UserName,
                Dob = user.DateOfBirth.ToString("yyyy-MM-dd"),
                Gender = user.Gender,
                Avatar = user.Avatar,
                DateCreated = user.DateCreated.ToString(),
                DateUpdated = user.DateUpdated.ToString(),
                Status = user.Status,
                TotalCartItem = user.CartItems.Count,
                TotalWishItem = user.WishItems.Count,
                TotalOrders = user.Orders.Count,
                TotalBought = user.Orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity)),
                TotalCost = user.Orders.Sum(o => o.TotalPrice),
                StatusCode = USER_STATUS.UserStatus[user.Status],
            };
        }

        public async Task<PagedResult<UserViewModel>> RetrieveAll(UserGetPagingRequest request)
        {
            try
            {
                var users = await _userManager.Users
                    .Include(u => u.CartItems)
                    .Include(u => u.WishItems)
                    .Include(u => u.Addresses)
                    .Include(u => u.Orders)
                    .ThenInclude(u => u.OrderItems)
                    .ToListAsync();
                string keyWord = request.Keyword;
                if (!string.IsNullOrEmpty(keyWord))
                {
                    users = (List<AppUser>)users.Where(a =>
                        a.UserName.Contains(keyWord) ||
                        a.PhoneNumber.Contains(keyWord)
                    );
                }

                int totalRow = users.Count;

                var dt = users
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetUserViewModel(x)).ToList();

                foreach (var x in dt)
                {
                    var roleIds = (await _context.UserRoles.Where(u => u.UserId == x.UserId).Select(k => k.RoleId).ToListAsync());
                    var roles = new List<RoleViewModel>();
                    foreach (string rId in roleIds)
                    {
                        var role = await _context.Roles.FindAsync(rId);

                        roles.Add(new RoleViewModel()
                        {
                            RoleId = role.Id,
                            RoleName = role.Name,
                        });
                    }
                    x.RoleIds = roleIds;
                    x.Roles = roles;
                }

                return new PagedResult<UserViewModel>()
                {
                    TotalItem = totalRow,
                    Items = dt
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserViewModel> RetrieveById(string userId)
        {
            try
            {
                var x = await _userManager.Users
                    .Where(x => x.Id == userId)
                    .Include(u => u.WishItems)
                    .Include(u => u.CartItems)
                    .Include(u => u.WishItems)
                    .Include(u => u.Orders)
                    .ThenInclude(u => u.OrderItems)
                    .FirstOrDefaultAsync();
                if (x == null)
                    return null;
                var user = GetUserViewModel(x);
                var roles = await _userManager.GetRolesAsync(x);
                user.RoleIds = (await _context.UserRoles.Where(u => u.UserId == x.Id).Select(k => k.RoleId).ToListAsync());
                user.Roles = new List<RoleViewModel>();
                foreach (string rId in user.RoleIds)
                {
                    var role = await _context.Roles.FindAsync(rId);

                    user.Roles.Add(new RoleViewModel()
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                    });
                }
                user.Orders = new PagedResult<OrderViewModel>();
                List<OrderViewModel> orders = new List<OrderViewModel>();
                foreach (var order in x.Orders)
                {
                    orders.Add(await _orderRepositories.RetrieveById(order.OrderId));
                }
                user.Orders.Items = orders;
                user.Orders.TotalItem = orders.Count;
                return user;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> Delete(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return -1;
                user.Status = USER_STATUS.IN_ACTIVE;
                await _userManager.UpdateAsync(user);

                return 1;
            }
            catch { return -1; }
        }

        public async Task<List<string>> CheckNewUser(UserCheckNewRequest request)
        {
            List<string> exist = new List<string>();
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                exist.Add("username");
            }
            user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                exist.Add("email");
            }

            user = await _context.Users.Where(x => x.PhoneNumber == request.Phone).FirstOrDefaultAsync();
            if (user != null)
            {
                exist.Add("phone");
            }

            return exist;
        }

        public async Task<List<string>> CheckEditUser(UserCheckEditRequest request)
        {
            List<string> exist = new List<string>();
            var user = await _context.Users.Where(x => x.UserName == request.UserName && x.Id != request.UserId).FirstOrDefaultAsync();
            if (user != null)
            {
                exist.Add("username");
            }
            user = await _context.Users.Where(x => x.Email == request.Email && x.Id != request.UserId).FirstOrDefaultAsync();
            if (user != null)
            {
                exist.Add("email");
            }

            user = await _context.Users.Where(x => x.PhoneNumber == request.Phone && x.Id != request.UserId).FirstOrDefaultAsync();
            if (user != null)
            {
                exist.Add("phone");
            }
            user = await _context.Users.Where(x => x.Id == request.UserId).FirstOrDefaultAsync();
            if (user != null && request.Password != null)
            {
                var res = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if (res == PasswordVerificationResult.Failed && request.IsChangePassword)
                    exist.Add("password");
            }
            return exist;
        }

        public async Task<TokenViewModel> AuthenticateWithGoogle(string email, string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new KeyNotFoundException("Cannot find your account");
            var res = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, false);
            if (!res.Succeeded)
                throw new AccessViolationException("Cannot login with your Google account");
            if (user.Status == USER_STATUS.IN_ACTIVE)
                throw new AccessViolationException("Your account has been banned");
            if (!user.EmailConfirmed)
                throw new AccessViolationException("Your account hasn't been confirm");
            string accessToken = await WriteJWT(user);
            string refreshToken = GenerateRefreshToken();
            DateTime refreshTokenExpiredTime = DateTime.Now.AddDays(7);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiredTime = refreshTokenExpiredTime;
            var isSuccess = await _userManager.UpdateAsync(user);
            if (!isSuccess.Succeeded)
                throw new Exception("Cannot login, please contact administrator");
            return new TokenViewModel { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<bool> VerifyToken(string email, string token, string host)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded) return true;
            await SendConfirmToken(user, host);
            return false;
        }

        public async Task<bool> CheckEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return !(user == null);
        }

        public async Task<bool> ForgotPassword(string email, string host)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string confirmUrl = host + $"/reset-password?token={token}&email={user.Email}";
            string content = "<h2>Chào " + user.FirstName + " " + user.LastName + ", </h2><h3>Link xác nhận đặt lại mật khẩu cho tài khoản <em>" + user.UserName + "</em>: <a href = \"" + confirmUrl + "\">Change password for account</a> </h3>" + "<h4>Bạn vui lòng xác nhận ngay khi thấy tin nhắn này. Xin cảm ơn!!!</h4>";
            string title = "FurSshop - Xác nhận đặt lại mật khẩu";

            return await _mailJetServices.SendMail($"{user.FirstName} {user.LastName}", user.Email, content, title);
        }

        public async Task<bool> VerifyForgotPasswordToken(string email, string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
                return true;
            return false;
        }

        public async Task RevokeToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task RevokeAllToken()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<bool> CheckPhone(string phone)
        {
            var user = await _userManager.Users.Where(u => u.PhoneNumber == phone).FirstOrDefaultAsync();
            return !(user == null);
        }

        public async Task<bool> CheckUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return !(user == null);
        }

        public async Task<int> AdminUpdateUser(AdminUserUpdateRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId) ?? throw new KeyNotFoundException("Cannot find this user");
                user.Status = request.Status;
                var res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    if (request.Roles == null || request.Roles[0] == "null")
                        throw new ValidationException("Cannot read user's role");
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    List<string> roles = new List<string>();
                    foreach (var roleId in JsonConvert.DeserializeObject<string[]>(request.Roles[0]))
                    {
                        var role = await _context.Roles.FindAsync(roleId);
                        if (role == null)
                        {
                            role = await _context.Roles.Where(x => x.Name.ToLower() == roleId.ToLower()).FirstOrDefaultAsync();
                        }
                        roles.Add(role.Name);
                    }
                    await _userManager.AddToRolesAsync(user, roles);

                    return 1;
                }
                throw new Exception("Cannot update user");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}