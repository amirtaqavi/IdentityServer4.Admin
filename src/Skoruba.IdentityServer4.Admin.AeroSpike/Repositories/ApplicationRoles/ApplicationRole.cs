namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.ApplicationRoles
{
    // Add profile data for application roles by adding properties to the ApplicationRole class
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string DisplayName { get; set; }
        public Guid ApiResourceId { get; set; }
        public string ApiResourceName { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid ModifierId { get; set; }
        public string ModifierName { get; set; }
        public DateTime ModificationDate { get; set; }
        public ISet<ApplicationRolePermission> rolePermissions { get; set; }


    }

}