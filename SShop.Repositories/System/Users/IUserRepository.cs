using SShop.Domain.Entities;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SShop.Repositories.System.Users
{
    public interface IUserRepository
    {
        Task<TokenViewModel> Authenticate(LoginRequest request);

        Task<TokenViewModel> RefreshToken(TokenViewModel request);

        Task RevokeToken(string userId);

        Task RevokeAllToken();

        Task<TokenViewModel> AuthenticateWithGoogle(string email, string loginProvider, string providerKey);

        Task<bool> Register(RegisterRequest request);

        Task<bool> VerifyToken(string email, string token, string host);

        Task<bool> ForgotPassword(string email, string host);

        Task<bool> VerifyForgotPasswordToken(string email, string token, string password);

        Task<PagedResult<UserViewModel>> RetrieveAll(UserGetPagingRequest request);

        Task<UserViewModel> RetrieveById(string userId);

        Task<(bool, string)> Update(UserUpdateRequest request);

        Task<int> Delete(string userId);

        Task<List<string>> CheckNewUser(UserCheckNewRequest request);

        Task<List<string>> CheckEditUser(UserCheckEditRequest request);

        Task<bool> CheckEmail(string email);

        Task<bool> CheckPhone(string phone);

        Task<bool> CheckUsername(string username);

        Task<int> AdminUpdateUser(AdminUserUpdateRequest request);
    }
}