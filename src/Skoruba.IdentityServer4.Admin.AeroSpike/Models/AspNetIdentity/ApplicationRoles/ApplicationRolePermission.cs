using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xanis.Infra.Message;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.ApplicationRoles
{
    public class ApplicationRolePermission : IBinarySerializable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ApiResourceId { get; set; }
        public string Category { get; set; }

        public byte[] GetBytes()
        {
            return new BinaryStreamSerializer(new byte[GetSize()])
                .With(Id)
                .With(ApiResourceId)
                .With(Name)
                .With(Category)
                .Serialize();
        }

        public int GetSize()
        {
            return new BinaryStreamSizeCalculator()
                .With(Id)
                .With(ApiResourceId)
                .With(Name)
                .With(Category)
                .Calculate();
        }

        public void SetBytes(byte[] bytes, ref int startIndex)
        {
            var deserilzer = new BinaryStreamDeserializer(bytes, startIndex);
            deserilzer
                .With(id => Id = id)
                .With(apiResourceId => ApiResourceId = apiResourceId)
                .With(name => Name = name)
                .Deserialize();

        }


    }
    public static class ApplicationRolePermissionExtensions
    {
        public static List<byte[]> ToBytesList(this IEnumerable<ApplicationRolePermission> rolePermissions)
        {
            var rolePermissionsBytes = new List<byte[]>();
            foreach (var rolePermission in rolePermissions)
                rolePermissionsBytes.Add(rolePermission.GetBytes());

            return rolePermissionsBytes;

        }
        public static ISet<ApplicationRolePermission> ToApplicationRolePermission(this IEnumerable<byte[]> rolePermissionsBytes)
        {
            var rolePermissions = new HashSet<ApplicationRolePermission>();
            try
            {



                foreach (var rolePermissionBytes in rolePermissionsBytes)
                {
                    var rolePermission = new ApplicationRolePermission();

                    var startIndex = 0;

                    rolePermission.SetBytes(rolePermissionBytes, ref startIndex);

                    rolePermissions.Add(rolePermission);
                }

                return rolePermissions;
            }
            catch (Exception)
            {

                return rolePermissions;
            }
        }
    }
}
