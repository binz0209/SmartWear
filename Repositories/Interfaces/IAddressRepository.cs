using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address> GetAddressByIdAsync(Guid id);
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(Guid id);
    }
}
