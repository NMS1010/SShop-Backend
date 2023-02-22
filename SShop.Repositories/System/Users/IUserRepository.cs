using SShop.ViewModels.Common;
using SShop.ViewModels.System.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SShop.Repositories.System.Users
{
    public interface IUserRepository
    {
        Task<string> Authenticate(LoginRequest request);

        Task<string> AuthenticateWithGoogle(string email, string loginProvider, string providerKey);

        Task<(bool, string)> Register(RegisterRequest request);

        Task<bool> VerifyToken(string email, string token);

        Task<bool> ForgotPassword(string email, string host);

        Task<bool> VerifyForgotPasswordToken(string email, string token, string password);

        Task<PagedResult<UserViewModel>> RetrieveAll(UserGetPagingRequest request);

        Task<UserViewModel> RetrieveById(string userId);

        Task<(bool, string)> Update(UserUpdateRequest request);

        Task<int> Delete(string userId);

        Task<List<string>> CheckNewUser(UserCheckNewRequest request);

        Task<List<string>> CheckEditUser(UserCheckEditRequest request);

        Task<bool> CheckEmail(string email);
    }
}