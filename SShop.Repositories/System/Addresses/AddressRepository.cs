using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;
using SShop.Domain.EF;
using SShop.Domain.Entities;
using SShop.ViewModels.Common;
using SShop.ViewModels.System.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Repositories.System.Addresses
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(AddressCreateRequest request)
        {
            try
            {
                var address = new Address()
                {
                    SpecificAddress = request.SpecificAddress,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Phone = request.Phone,
                    Province = new Province()
                    {
                        ProvinceCode = request.ProvinceCode,
                        ProvinceName = request.ProvinceName,
                    },
                    District = new District()
                    {
                        DistrictCode = request.DistrictCode,
                        DistrictName = request.DistrictName,
                    },
                    Ward = new Ward()
                    {
                        WardCode = request.WardCode,
                        WardName = request.WardName,
                    },
                    UserId = request.UserId,
                    IsDefault = request.IsDefault,
                };
                _context.Addresses.Add(address);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> Delete(int id)
        {
            try
            {
                var address = await _context.Addresses.FindAsync(id);
                if (address == null)
                {
                    return -1;
                }
                _context.Addresses.Remove(address);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private AddressViewModel GetAddressViewModel(Address address)
        {
            return new AddressViewModel()
            {
                SpecificAddress = address.SpecificAddress,
                DistrictCode = address.District.DistrictCode,
                DistrictId = address.District.DistrictId,
                DistrictName = address.District.DistrictName,
                ProvinceCode = address.Province.ProvinceCode,
                ProvinceId = address.Province.ProvinceId,
                ProvinceName = address.Province.ProvinceName,
                WardCode = address.Ward.WardCode,
                WardId = address.Ward.WardId,
                WardName = address.Ward.WardName,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Phone = address.Phone,
                UserId = address.UserId,
                IsDefault = address.IsDefault
            };
        }

        public async Task<PagedResult<AddressViewModel>> GetAddressByUserId(string userId)
        {
            try
            {
                var query = await _context.Addresses
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.Ward)
                    .ToListAsync();
                var data = query.Select(x => GetAddressViewModel(x)).ToList();

                return new PagedResult<AddressViewModel>
                {
                    Items = data,
                    TotalItem = query.Count
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<PagedResult<AddressViewModel>> RetrieveAll(AddressGetPagingRequest request)
        {
            try
            {
                var query = await _context.Addresses
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.Ward)
                    .ToListAsync();
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query
                        .Where(x => x.SpecificAddress.Contains(request.Keyword))
                        .ToList();
                }
                var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => GetAddressViewModel(x)).ToList();

                return new PagedResult<AddressViewModel>
                {
                    Items = data,
                    TotalItem = query.Count
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AddressViewModel> RetrieveById(int addressId)
        {
            try
            {
                var address = await _context.Addresses
                    .Where(x => x.AddressId == addressId)
                    .Include(x => x.Province)
                    .Include(x => x.District)
                    .Include(x => x.Ward)
                    .FirstOrDefaultAsync();
                if (address == null)
                {
                    return null;
                }
                return GetAddressViewModel(address);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> Update(AddressUpdateRequest request)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var address = await _context.Addresses.FindAsync(request.AddressId);
                if (address == null)
                {
                    return -1;
                }
                address.Province = new Province()
                {
                    ProvinceCode = request.ProvinceCode,
                    ProvinceName = request.ProvinceName,
                };
                address.District = new District()
                {
                    DistrictCode = request.DistrictCode,
                    DistrictName = request.DistrictName,
                };
                address.Ward = new Ward()
                {
                    WardCode = request.WardCode,
                    WardName = request.WardName,
                };
                address.FirstName = request.FirstName;
                address.LastName = request.LastName;
                address.SpecificAddress = request.SpecificAddress;
                address.Phone = request.Phone;
                address.IsDefault = request.IsDefault;
                _context.Addresses.Update(address);

                var province = await _context.Provinces.FindAsync(request.ProvinceId);
                province.Addresses.Remove(address);

                var district = await _context.Districts.FindAsync(request.DistrictId);
                district.Addresses.Remove(address);

                var ward = await _context.Wards.FindAsync(request.WardId);
                ward.Addresses.Remove(address);

                int res = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return res;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return -1;
            }
        }
    }
}