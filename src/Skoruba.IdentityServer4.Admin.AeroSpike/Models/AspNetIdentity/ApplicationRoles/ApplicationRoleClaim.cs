using System;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.ApplicationRoles
{
    public class ApplicationRoleClaim
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool IsDeleted { get; set; }
    }
}
