namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.ApplicationRoles.Interfaces
{
    public interface IApplicationRoleClaimRepository
    {
        Task AddClaimAsync(ApplicationRole role, ApplicationRoleClaim claim, CancellationToken cancellationToken = default);
        Task RemoveClaimAsync(ApplicationRoleClaim claim, CancellationToken cancellationToken = default);
        Task<IEnumerable<ApplicationRoleClaim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default);
    }
}
