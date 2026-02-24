namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.ApplicationRoles
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