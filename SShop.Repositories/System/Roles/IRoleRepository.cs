using SShop.ViewModels.System.Roles;
using SShop.Repositories.Common.Interfaces;

namespace SShop.Repositories.System.Roles
{
    public interface IRoleRepository : IModifyEntity<RoleCreateRequest, RoleUpdateRequest, string>,
        IRetrieveEntity<RoleViewModel, RoleGetPagingRequest, string>
    {
    }
}