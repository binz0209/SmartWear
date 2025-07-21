using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<Role> GetRoleByIdAsync(Guid id)
        {
            return await _roleRepository.GetRoleByIdAsync(id);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _roleRepository.AddRoleAsync(role);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            await _roleRepository.UpdateRoleAsync(role);
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            await _roleRepository.DeleteRoleAsync(id);
        }
    }
}
