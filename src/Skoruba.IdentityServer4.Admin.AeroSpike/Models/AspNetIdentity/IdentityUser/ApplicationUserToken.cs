using System;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.IdentityUser
{
    public class ApplicationUserToken
    {
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid UserId { get; set; }
    }
}
