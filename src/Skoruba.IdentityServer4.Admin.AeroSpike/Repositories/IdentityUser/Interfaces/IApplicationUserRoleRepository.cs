using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser.Interfaces
{
    public interface IApplicationUserRoleRepository
    {
        Task AddAsync(Guid userId, Guid roleId);
        Task RemoveAsync(Guid userId, Guid roleId);
        Task<List<Guid>> GetUserRolesAsync(Guid userId);
        Task<List<Guid>> GetUsersInRoleAsync(Guid roleId);
        IEnumerable<Guid> ScanAllUsers();
    }
}
