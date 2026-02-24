using System;
using Xanis.Infra.Message;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.IdentityUser
{
    public class ApplicationUserLogin
    {
        public string LoginProvider { get; set; }
        public string LoginKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public Guid UserId { get; set; }
    }
}

