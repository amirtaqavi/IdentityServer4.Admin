using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.Extensions.Options;
using Xanis.DependancyInjection.Extenstions.Aerospike.Model;
using Xanis.Idp.Models.AspNetIdentity.IdentityUser;
using Xanis.Idp.Repository;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser
{
    public class ApplicationUserRoleRepository : IApplicationUserRoleRepository, IAerospikeSchemaProvider
    {
        private readonly string Namespace = "";
        private readonly IAsyncClient _client;


        public ApplicationUserRoleRepository(IAsyncClient client, IOptions<AerospikeSettings> settings)
        {
            _client = client;
            Namespace = settings.Value.Namespace;
        }

        public async Task AddAsync(Guid userId, Guid roleId)
        {
            var bins = FillBins(userId, roleId);
            await _client.Add(new WritePolicy(), CancellationToken.None, GetKey(userId, roleId), bins.ToArray());

        }

        public Task<List<Guid>> GetUserRolesAsync(Guid userId)
        {
            var bins = GetBinsName();

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = bins
            };

            var queryPolicy = new QueryPolicy();
            queryPolicy.filterExp = Exp.Build(Exp.EQ(Exp.StringBin(UserId_Bin), Exp.Val(userId.ToString())));

            var result = new List<ApplicationUserRole>();

            _client.Query(queryPolicy, stmt, (key, record) =>
            {
                var login = ToUserRole(record);

                result.Add(login);
            });

            return Task.FromResult(result.Select(x => x.RoleId).ToList());
        }


        public Task<List<Guid>> GetUsersInRoleAsync(Guid roleId)
        {
            var bins = GetBinsName();

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = bins
            };

            var queryPolicy = new QueryPolicy();
            queryPolicy.filterExp = Exp.Build(Exp.EQ(Exp.StringBin(RoleId_Bin), Exp.Val(roleId.ToString())));

            var result = new List<ApplicationUserRole>();

            _client.Query(queryPolicy, stmt, (key, record) =>
            {
                var login = ToUserRole(record);

                result.Add(login);
            });

            return Task.FromResult(result.Select(x => x.UserId).ToList());
        }
        public async Task RemoveAsync(Guid userId, Guid roleId)
        {
            await _client.Delete(new WritePolicy(), CancellationToken.None, GetKey(userId, roleId));
        }

        public IEnumerable<Guid> ScanAllUsers()
        {
            var list = new List<Guid>();
            try
            {

                // Set up the scan policy
                ScanPolicy scanPolicy = new ScanPolicy();
                scanPolicy.concurrentNodes = true; // Scanning nodes in parallel
                scanPolicy.includeBinData = true;  // Return all bin data

                List<Record> records = new List<Record>();
                // Perform the scan
                _client.ScanAll(scanPolicy, Namespace, Set_Name, (Key key, Record record) =>
                {
                    records.Add(record);


                });

                foreach (var record in records)
                {
                    try
                    {
                        var userRole = ToUserRole(record);
                        if (userRole != null)
                            if (!list.Contains(userRole.UserId))
                                list.Add(userRole.UserId);
                    }
                    catch (Exception ex)
                    {

                    }
                }


            }
            catch (AerospikeException ex)
            {
                Console.WriteLine($"Aerospike error: {ex.Message}");
            }
            return list;
        }
        private Key GetKey(Guid userId, Guid roleId) => new Key(Namespace, Set_Name, GetKeyValue(userId, roleId));
        string GetKeyValue(Guid userId, Guid roleId) => $"{userId.ToString()}:{roleId.ToString()}";
        ApplicationUserRole ToUserRole(Record record) => record == null ? default : ToObject(record);
        string[] GetBinsName()
        {
            return new[]{
                Id_Bin,
                UserId_Bin,
                RoleId_Bin
            };
        }

        BinList FillBins(Guid userId, Guid roleId)
        {
            var binList = new BinList();
            binList.Add(Id_Bin, GetKeyValue(userId, roleId));
            binList.Add(RoleId_Bin, roleId.ToString());
            binList.Add(UserId_Bin, userId.ToString());
            return binList;
        }

        ApplicationUserRole ToObject(Record record)
        {
            return new ApplicationUserRole
            {
                RoleId = Guid.Parse(record.GetString(RoleId_Bin)),
                UserId = Guid.Parse(record.GetString(UserId_Bin)),
            };
        }
        public List<string> GetBinNames() => GetBinsName().ToList();

        public string GetSetName() => Set_Name;

        public string GetNamespace() => Namespace;

        public List<(string IndexName, string BinName, IndexType Type)> GetIndexes()
        {
            return new List<(string IndexName, string BinName, IndexType Type)>();
        }




        private const string Set_Name = "user_role";
        #region Bins
        private const string Id_Bin = "id";
        private const string UserId_Bin = "userid";
        private const string RoleId_Bin = "roleid";

        #endregion
    }
}
