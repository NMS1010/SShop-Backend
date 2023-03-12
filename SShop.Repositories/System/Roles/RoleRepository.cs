using SShop.Domain.EF;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SShop.Repositories.System.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<int> Create(RoleCreateRequest request)
        {
            try
            {
                var role = new IdentityRole()
                {
                    Name = request.RoleName
                };
                await _roleManager.CreateAsync(role);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> Delete(string id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                await _roleManager.DeleteAsync(role);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public RoleViewModel GetRoleViewModel(IdentityRole role)
        {
            return new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
        }

        public async Task<PagedResult<RoleViewModel>> RetrieveAll(RoleGetPagingRequest request)
        {
            try
            {
                var query = await _context.Roles
                .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.Name.Contains(request.Keyword))
                        .ToList();
                }
                var data = query
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetRoleViewModel(x)).ToList();

                return new PagedResult<RoleViewModel>
                {
                    TotalItem = query.Count,
                    Items = data
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<RoleViewModel> RetrieveById(string id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                return GetRoleViewModel(role);
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> Update(RoleUpdateRequest request)
        {
            try
            {
                var role = await _context.Roles.FindAsync(request.RoleId);
                role.Name = request.RoleName;
                await _roleManager.UpdateAsync(role);
                return await _context.SaveChangesAsync();
            }
            catch
            {
                return -1;
            }
        }
    }
}