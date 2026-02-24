using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser.Interfaces
{
    public interface IApplicationUserLoginRepository
    {
        Task AddLoginAsync(Guid userId, ApplicationUserLogin login, CancellationToken cancellationToken = default);
        Task RemoveLoginAsync(Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken = default);
        Task<List<ApplicationUserLogin>> GetLoginsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<ApplicationUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default);
    }
}
