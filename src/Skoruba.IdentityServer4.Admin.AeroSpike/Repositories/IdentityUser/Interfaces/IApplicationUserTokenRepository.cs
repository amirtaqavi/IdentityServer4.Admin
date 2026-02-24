using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser.Interfaces
{
    public interface IApplicationUserTokenRepository
    {
        Task<ApplicationUserToken> FindTokenAsync(Guid userId, string loginProvider, string name, CancellationToken cancellationToken);
        Task AddUserTokenAsync(ApplicationUserToken token);
        Task RemoveUserTokenAsync(ApplicationUserToken token);
    }
}
