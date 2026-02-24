using Microsoft.AspNetCore.Identity;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.ApplicationRoles.Interfaces
{
    public interface IApplicationRoleRepository
    {
        Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken = default);
        Task<ApplicationRole> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApplicationRole> FindByNameAsync(string normalizedName, Guid apiResourceId, CancellationToken cancellationToken = default);
        Task<ApplicationRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationRole>> GetAllAsync();
        IEnumerable<Guid> ScanAllRoles();
    }
}