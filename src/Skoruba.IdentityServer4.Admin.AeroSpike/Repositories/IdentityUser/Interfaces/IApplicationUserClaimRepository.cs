using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser.Interfaces
{
    public interface IApplicationUserClaimRepository
    {
        Task<List<ApplicationUserClaim>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<ApplicationUserClaim>> GetClaimsAsync(string type, string value, CancellationToken cancellationToken = default);
        Task AddClaimsAsync(IEnumerable<ApplicationUserClaim> claims, CancellationToken cancellationToken = default);
        Task ReplaceClaimAsync(ApplicationUserClaim claim, ApplicationUserClaim newClaim, CancellationToken cancellationToken = default);
        Task RemoveClaimsAsync(Guid userId, IEnumerable<ApplicationUserClaim> claims, CancellationToken cancellationToken = default);

    }
}
