using SShop.Repositories.Common.Interfaces;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Addresses;

namespace SShop.Repositories.System.Addresses
{
    public interface IAddressRepository : IModifyEntity<AddressCreateRequest, AddressUpdateRequest, int>,
        IRetrieveEntity<AddressViewModel, AddressGetPagingRequest, int>
    {
        Task<PagedResult<AddressViewModel>> GetAddressByUserId(string userId);
    }
}