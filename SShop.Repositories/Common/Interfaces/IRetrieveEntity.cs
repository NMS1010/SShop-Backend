using SShop.ViewModels.Common;

namespace SShop.Repositories.Common.Interfaces
{
    public interface IRetrieveEntity<ReturnType, Entity, EntityId>
    {
        Task<PagedResult<ReturnType>> RetrieveAll(Entity entity);

        Task<ReturnType> RetrieveById(EntityId entity);
    }
}