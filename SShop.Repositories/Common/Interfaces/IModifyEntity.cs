using System.Threading.Tasks;

namespace SShop.Repositories.Common.Interfaces
{
    public interface IModifyEntity<CreateRequest, UpdateRequest, EntityId>
    {
        Task<int> Create(CreateRequest request);

        Task<int> Update(UpdateRequest request);

        Task<int> Delete(EntityId id);
    }
}