using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser.Interfaces
{
    public interface IApplicationUserRepository
    {
        Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApplicationUser> FindByIdAsync(string userId, List<int> userType = null, CancellationToken cancellationToken = default);
        Task<ApplicationUser> FindByNameAsync(string normalizedUserName, List<int> userType = null, CancellationToken cancellationToken = default);
        Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, List<int> userType = null, CancellationToken cancellationToken = default);
        Task<List<ApplicationUser>> GetAllAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
        Task<ApplicationUser> FindByNationalCodeAsync(string nationalCode, List<int> userType = null, CancellationToken cancellationToken = default);
        Task<ApplicationUser> FindByCellPhoneAsync(string cellPhone, List<int> userType = null, CancellationToken cancellationToken = default);
        Task<ApplicationUser> GetByUsernameOrEmailOrNationalCodeAsync(ApplicationUser user, List<int> userType = null);
        Task<ApplicationUser> FindByPersonageIdAsync(string personageId, List<int> userType = null, CancellationToken cancellationToken = default);
        IEnumerable<ApplicationUser> GetAll();
        Task<List<ApplicationUser>> FindAllByPersonageIdAsync(string personageId, List<int> userType = null, CancellationToken cancellationToken = default);
    }
}
