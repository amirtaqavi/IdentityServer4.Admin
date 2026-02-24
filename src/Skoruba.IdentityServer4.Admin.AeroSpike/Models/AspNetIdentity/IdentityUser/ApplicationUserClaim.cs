using System;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.IdentityUser
{
    public class ApplicationUserClaim
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool IsDeleted { get; set; }
    }
}
