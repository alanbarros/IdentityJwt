using System;
using IdentityJwt.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityJwt.Repository
{
    public class RoleRepository : IRepository<Role>
    {
        private const int success = 1;
        private const int fail = 0;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public int Add(Role entity)
        {
            if (!_roleManager.RoleExistsAsync(entity.Name).Result)
            {
                var resultado = _roleManager.CreateAsync(entity).Result;
                if (!resultado.Succeeded)
                {
                    throw new Exception(
                        $"Erro durante a criação da role {entity.Name}.");
                }
                return success;
            }
            return fail;
        }
    }
}