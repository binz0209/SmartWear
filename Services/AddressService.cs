using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _addressRepository.GetAllAddressesAsync();
        }

        public async Task<Address> GetAddressByIdAsync(Guid id)
        {
            return await _addressRepository.GetAddressByIdAsync(id);
        }

        public async Task AddAddressAsync(Address address)
        {
            await _addressRepository.AddAddressAsync(address);
        }

        public async Task UpdateAddressAsync(Address address)
        {
            await _addressRepository.UpdateAddressAsync(address);
        }

        public async Task DeleteAddressAsync(Guid id)
        {
            await _addressRepository.DeleteAddressAsync(id);
        }
    }
}
